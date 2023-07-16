using System.Collections;
using System.Threading;
using UnityEngine;

public class PlayerBody : MonoBehaviour
{

    [SerializeField]
    private bool freeCam = false;


    public static PlayerBody localPlayer = null;

    // Camera stuff
    private Camera camera;

    [Header("Attributes")]
    [SerializeField]
    private GameObject baseBody;
    [SerializeField]
    private GameObject head, legs, spine0, spine1, spine2;
    public Vector3 regOffset;

    [SerializeField]
    private CharacterController controller;
    //[SerializeField]
    //private PlayerController playerControls;
    //[SerializeField]
    //private PlayerDebugger debugger;
    // Movement
    [SerializeField]
    private float accel = 5f, maxSpeed = 3f, sprintAccel = 10f, sprintMaxSpeed = 6f, crouchAccel = 5f, crouchMaxSpeed = 5.5f, jumpHeight = 7f;
    private bool grounded;
    public bool IsGrounded { get { return grounded; } }
    public bool Sprinting { get { return sprinting; } }
    public Vector3 Velocity { get { return velocity; } }
    private bool parkouring = false; // will be used to remove movement while doing a parkour move that uses a coroutine
    public bool Parkouring { get { return parkouring; } }

    [HideInInspector]
    public bool vault;
    [HideInInspector]
    public int muscleUp;

    public bool freeze;


    #region Physics
    private Vector3 velocity;
    private bool useGravity = true;
    #endregion

    #region Input
    [HideInInspector]
    public float mouseXInput, mouseYInput;
    [HideInInspector]
    public bool forwardBool, left, back, right, jump, jumpH, sprint, crouchH, crouchT, fire, ads, reload, primaryWeapon, secondaryWeapon, interact;
    [SerializeField]
    private bool hasJump = false;
    private float justJumped = 0f;
    private float sensitivityMultiplier = 1.5f;
    private bool paused;

    private void Scan()
    {
        mouseXInput = Input.GetAxis("Mouse X");
        mouseYInput = Input.GetAxis("Mouse Y");

        forwardBool = Input.GetKey(KeyCode.W);
        left = Input.GetKey(KeyCode.A);
        back = Input.GetKey(KeyCode.S);
        right = Input.GetKey(KeyCode.D);
        jump = Input.GetKeyDown(KeyCode.Space);
        jumpH = Input.GetKey(KeyCode.Y);
        sprint = true;
        crouchH = Input.GetKey(KeyCode.LeftControl);
        crouchT = false;
        interact = Input.GetKey(KeyCode.E);
    }

    #endregion

    #region Sensors
    [Header("Sensor")]
    [SerializeField]
    private Transform feet, waist, shoulders, highest, tooHigh, physicalHips;
    [Header("Sensor Attribute")]
    [SerializeField]
    private float rayOut;
    #endregion

   
    
    




    private void Start()
    {
        camera = Camera.main;
       
            //camera binding
            //camera.transform.SetParent(head.transform);
            camera.transform.position = head.transform.position;
            camera.transform.rotation = head.transform.rotation;

            //TESTING ONLY
            Cursor.lockState = CursorLockMode.Locked;

    }

    private void Update()
    {
        Scan();

        GJLook();

        GJGrounded();
        Move();

        
    }

    private float rotX = 0;

    void GJLook()
    {
        transform.Rotate(new Vector3(0, mouseXInput * sensitivityMultiplier, 0));

        camera.transform.Rotate(new Vector3(mouseYInput * sensitivityMultiplier * -1, 0, 0));
        //if (camera.transform.rotation.eulerAngles.x > 80) camera.transform.rotation = Quaternion.Euler(80, 0, 0);
        //if (camera.transform.rotation.eulerAngles.x < -80) camera.transform.rotation = Quaternion.Euler(-80, 0, 0);
        Quaternion rot = camera.transform.localRotation;
        if (rot.x > 0.6)
            rot.x = 0.6f;
        else if (rot.x < -0.6)
            rot.x = -0.6f;

        camera.transform.localRotation = rot;

    }

    //public Transform CameraTarget { get { return head.transform; } }

    /*
    private void LateUpdate()
    {
        // we dont use this (yet)

        if (freeCam) return;
        spine0.transform.Rotate(Vector3.right, rotX / 3f);
        spine1.transform.Rotate(Vector3.right, rotX / 3f);
        spine2.transform.Rotate(Vector3.right, rotX / 3f);

            camera.transform.position = head.transform.position;
            camera.transform.rotation = head.transform.rotation;
            camera.transform.Rotate(Vector3.right, -26);
        
    }
    */

    #region Movement
    [SerializeField]
    private bool wasHoldingCrouch = false, sprinting = false, wasGrounded = true;


    private void Move()
    {
        Debug.DrawLine(waist.position + new Vector3(0, -0.12f, 0), waist.position + waist.forward, Color.green);

        // Moved Grounded to Update() // thank you for telling me lmao

        if (grounded) hasJump = true;

        sprinting = forwardBool;
        

        Vector3 forward = transform.forward * ((forwardBool ? 1 : 0) - (back ? 1 : 0));
        Vector3 strafeRight = transform.right * ((right ? 1 : 0) - (left ? 1 : 0));

        //if (grounded) Debug.Log("grounded");
        //else Debug.Log("un");

        DoWallRun();
        if (sliding)
        {
            Slide();

            if ((!crouchH && wasHoldingCrouch) || (crouchT)) Uncrouch();
        }
        else if (grounded)
        {
            velocity = (forward + strafeRight).normalized * (sprinting ? sprintAccel : accel); // does it make sense to call a couritine every frame to lerp velocity from velocity to max,
                                                                                               // or maybe to do reverse friction (I'll add reverse friction - MJB)
            if (!crouching)
            {
                if (crouchH || crouchT) Crouch();
            }
            else if ((!crouchH && wasHoldingCrouch) || (crouchT)) Uncrouch();

            //baseBody.transform.forward = camera.transform.forward-regOffset;
            //transform.forward = baseBody.transform.forward;
            //baseBody.transform.localRotation = Quaternion.identity;

            //if (jump && sprinting && !parkouring) CheckVault();
        }
        else if (!wallRunning && !parkouring) //mid-air
        {
            var aDot = Vector3.Dot(Vector3.Project(velocity, (forward + strafeRight).normalized), forward);
            aDot /= Mathf.Abs(aDot);
            var bDot = Vector3.Dot(Vector3.Project(velocity, (forward + strafeRight).normalized), strafeRight);
            bDot /= Mathf.Abs(bDot);
            if (Vector3.Project(velocity, (forward + strafeRight).normalized).magnitude * (float.IsNaN(aDot) ? 1 : aDot) * (float.IsNaN(bDot) ? 1 : bDot) < accel)
            {
                velocity += ((forward + strafeRight).normalized * accel * Time.deltaTime);
            }

            if (!crouching)
            {
                if (crouchH || crouchT) Crouch();
            }
            else if ((!crouchH && wasHoldingCrouch) || (crouchT)) Uncrouch();

            //if (jumpH && velocity.y < 2) CheckMuscleUp();

            //regOffset = new Vector3(baseBody.transform.forward.x, camera.transform.forward.y, baseBody.transform.forward.z);
            //camera.transform.forward = regOffset;

        }
        if (useGravity) velocity += (Physics.gravity * Time.deltaTime);// + ((grounded && !sliding) ? Physics.gravity * 1f : Vector3.zero);

        //Jumping
        if (jump && grounded && hasJump && !parkouring) Jump();
        else if (grounded && !jump) hasJump = true;

        MovePlayer(velocity);

        //sprint and crouch fidelity
        if (crouchH) wasHoldingCrouch = true; // a comparison that determines if you JUST released crouchH
        if (!forwardBool) sprinting = false; // stop sprinting if you release w
        if (grounded) wasGrounded = true;
        else wasGrounded = false;

        //if (grounded && !wasGrounded) StartCoroutine(Landing()); // not for this version
    }

    [SerializeField]
    private float maxVelocity = 12;
    private void MovePlayer(Vector3 velocity)
    {
        if (velocity.magnitude > maxVelocity) velocity = velocity.normalized * maxVelocity;
        var flags = controller.Move(velocity * Time.deltaTime);
    }

    private RaycastHit groundInfo;
    public float dist = 0.1f, slopeThreshold = 0.66f, slopeDot;
    private bool wasOnSlope = false;
    private Vector3 groundNormal;

    private void GJGrounded()
    {
        RaycastHit info;

        justJumped-=Time.deltaTime;

        if (justJumped > 0)
        {
            grounded = false;
            return;
        }

        grounded = Physics.Raycast(feet.transform.position + new Vector3(0, 0.2f, 0), -transform.up, out info, 0.3f, ground);

        //Debug.Log(info.collider?.name);

        //Debug.Log("player is " + (!grounded ? "not " : "") + "grounded");
    }

    private void CheckGrounded()
    {

        //grounded = false;
        //Collider[] colliders = Physics.OverlapSphere(feet.transform.position + new Vector3(0, -jumpHeight / 240, 0) + new Vector3(0, 0.3f, 0), 0.3f, ground);

        //grounded = (colliders.Length > 0);

        if (!crouching) grounded = Physics.SphereCast(feet.transform.position + new Vector3(0, 0.3f, 0),
            0.3f, -transform.up, out groundInfo, jumpHeight / 360, ground);
        else grounded = Physics.SphereCast(feet.transform.position + new Vector3(0, 0.5f, 0),
            0.3f, -transform.up, out groundInfo, jumpHeight / 360, ground);


        if (grounded) groundNormal = SphereCastNormal(groundInfo, -transform.up);

        slopeDot = Vector3.Dot(groundNormal, Vector3.up);
        if (grounded)
        {
            if (slopeDot == 0)
            { }
            else if (slopeDot < slopeThreshold) //must be on a slope greater than acceptable
            {
                grounded = false;
                if (!wasOnSlope)
                {

                    wasOnSlope = true;
                }
            }
        }

        if (grounded != wasGrounded) Debug.Log("Is grounded " + grounded);

    }

    public static Vector3 SphereCastNormal(RaycastHit hit, Vector3 dir)
    {
        if (hit.collider is MeshCollider)
        {
            var collider = hit.collider as MeshCollider;
            var mesh = collider.sharedMesh;
            var tris = mesh.triangles;
            var verts = mesh.vertices;

            var v0 = verts[tris[hit.triangleIndex * 3]];
            var v1 = verts[tris[hit.triangleIndex * 3 + 1]];
            var v2 = verts[tris[hit.triangleIndex * 3 + 2]];

            var n = Vector3.Cross(v1 - v0, v2 - v1).normalized;

            return hit.transform.TransformDirection(n);
        }
        else
        {
            RaycastHit result;
            hit.collider.Raycast(new Ray(hit.point - dir * 0.01f, dir), out result, 0.011f);
            return result.normal;
        }
    }

    void Jump()
    {
        justJumped = 0.2f;
        grounded = false;
        velocity = new Vector3(velocity.x, jumpHeight, velocity.z);
    }
    
    private void Look()
    {
        float oldX = rotX;
        #region Parkour Look
        if (wallRunning || parkouring)
        {
            float mouseX = mouseXInput * sensitivityMultiplier;
            float mouseY = mouseYInput * sensitivityMultiplier;

            baseBody.transform.Rotate(new Vector3(0, mouseX, 0));

            if (baseBody.transform.localEulerAngles.y > 90 && baseBody.transform.localEulerAngles.y < 270)
            {
                if (baseBody.transform.localEulerAngles.y > 180)
                    baseBody.transform.localEulerAngles = new Vector3(0, 270, 0);
                else
                    baseBody.transform.localEulerAngles = new Vector3(0, 90, 0);
            }

            //head.transform.Rotate(new Vector3(-mouseY, 0, 0));

            rotX -= mouseY;
            if (rotX <= -85) rotX = -85;
            else if (rotX >= 85) rotX = 85;

            Quaternion rot = head.transform.localRotation; // make it
            if (rot.x > 0.7071068) // on every frame that your head is too far right
                rot.x = 0.7071068f; //you cannot turn it right, and we will adjust it left
            else if (rot.x < -0.7071068)
                rot.x = -0.7071068f;
            head.transform.localRotation = rot;
        }
        #endregion
        #region Normal Look
        else
        {
            float mouseX = mouseXInput * sensitivityMultiplier;
            float mouseY = mouseYInput * sensitivityMultiplier;

            transform.Rotate(new Vector3(0, mouseX, 0));
            //head.transform.Rotate(new Vector3(-mouseY, 0, 0));

            rotX -= mouseY;
            if (rotX <= -85) rotX = -85;
            else if (rotX >= 85) rotX = 85;

            Quaternion rot = head.transform.localRotation;
            if (rot.x > 0.7071068)
                rot.x = 0.7071068f;
            else if (rot.x < -0.7071068)
                rot.x = -0.7071068f;
            head.transform.localRotation = rot;
        }
        #endregion
    }
    #endregion

    #region Wallrun
    [Header("Wallrunning")]
    [SerializeField]
    private LayerMask wall;
    [SerializeField]
    private bool wallRunning; public bool WallRunning { get { return wallRunning; } }
    private Vector3 wallForward; //facing retains original direction you were facing when you hit the wall
    [SerializeField]
    private float ejectionSpeed = 3f, minWallSpeed;

    private bool wallLeft, wallRight;
    public bool WallRunLeft { get { return (wallLeft && wallRunning); } }
    public bool WallRunRight { get { return (wallRight && wallRunning); } }

    private void DoWallRun()
    {
        if (!grounded)
        {
            wallLeft = Physics.Raycast(transform.position, -transform.right, out RaycastHit infoLeft, controller.radius + rayOut, wall);
            wallRight = Physics.Raycast(transform.position, transform.right, out RaycastHit infoRight, controller.radius + rayOut, wall);
            //bool wallInFront = Physics.Raycast(transform.position, transform.forward, out RaycastHit infoInFront, controller.radius + rayOut, wall);

            if (Vector3.Dot(velocity, infoLeft.normal) > 0) wallLeft = false;
            if (Vector3.Dot(velocity, infoRight.normal) > 0) wallRight = false;

            //if (jump && wallInFront) //if you ran straight at a wall holding space
            //{
            //    if (Vector3.Dot(velocity, infoInFront.normal) < 0)
            //        velocity.x = 0f;
            //    velocity.z = 0f;
            //    velocity.y *= 0.1f;

            //    velocity += infoInFront.normal * ejectionSpeed;
            //    velocity += new Vector3(0, jumpHeight, 0);

            //    wallRunning = false;
            //    useGravity = true;
            //    hasJump = false;

            //    return;
            //}

            if (wallLeft && (forwardBool || wallRunning))
            {
                wallForward = Vector3.Cross(transform.up, -infoLeft.normal);

                if (!wallRunning/* && !hasJump*/)
                {
                    Debug.Log("attaching to left wall");

                    if (jump)
                        hasJump = false;

                    wallRunning = true;
                    useGravity = false;

                    //if you're not moving in the direction of the wallrun fast enough
                    if ((Vector3.Dot(velocity, wallForward) * Vector3.Project(velocity, wallForward).magnitude) < minWallSpeed) // tune min wall speed
                    {
                        Debug.Log("falling off left wall");

                        float tempY = velocity.y;
                        velocity = Vector3.Project(velocity, wallForward);
                        velocity.y = tempY / 2;

                        velocity += infoLeft.normal * ejectionSpeed;
                        velocity += new Vector3(0, jumpHeight / 1.25f, 0);
                        Debug.Log(Vector3.Project(velocity, infoLeft.normal));
                        wallRunning = false;
                        useGravity = true;
                        hasJump = false;

                        return;
                    }

                    velocity.x = 0f;
                    velocity.z = 0f;

                    velocity -= infoLeft.normal * 3f;
                    velocity += wallForward * maxSpeed * slideBoostMultiplier * slideBoostMultiplier;

                    Vector3 facing = transform.forward;
                    transform.forward = wallForward;
                    baseBody.transform.forward = facing;


                }

                if (!jump) hasJump = true;

                if (jump && hasJump) //left wall
                {
                    Debug.Log("jumping off left wall");

                    velocity = Vector3.Project(velocity, wallForward);
                    velocity += infoLeft.normal * ejectionSpeed / 3 * 2;
                    velocity += head.transform.forward * ejectionSpeed / 3;
                    velocity += new Vector3(0, jumpHeight, 0);
                    //Debug.Log(Vector3.Project(velocity, infoLeft.normal));
                    wallRunning = false;
                    useGravity = true;
                    hasJump = false;

                    /*
                    Vector3 facing = new Vector3(head.transform.forward.x, 0, head.transform.forward.z);
                    Vector3 bodyDirection = new Vector3(head.transform.rotation.eulerAngles.x, 0, 0);
                    head.transform.rotation = Quaternion.Euler(bodyDirection);
                    transform.forward = facing;
                    */

                    return;
                }

                velocity += (Physics.gravity / 2f) * Time.deltaTime;

                transform.forward = wallForward;
                //Debug.Log("LEFT WALL");
            }
            else if (wallRight && (forwardBool || wallRunning))
            {
                //Debug.Log("RIGHT WALL");
                wallForward = Vector3.Cross(transform.up, infoRight.normal);

                if (!wallRunning/* && !hasJump*/)
                {
                    if (jump)
                        hasJump = false;

                    wallRunning = true;
                    useGravity = false;

                    //if you're not moving in the direction of the wallrun fast enough
                    if ((Vector3.Dot(velocity, wallForward) * Vector3.Project(velocity, wallForward).magnitude) < minWallSpeed) // tune min wall speed
                    {
                        Debug.Log("falling off right wall");

                        float tempY = velocity.y;
                        velocity = Vector3.Project(velocity, wallForward);
                        velocity.y = tempY / 2;

                        velocity += infoRight.normal * ejectionSpeed;
                        velocity += new Vector3(0, jumpHeight / 1.25f, 0);
                        Debug.Log(Vector3.Project(velocity, infoRight.normal));
                        wallRunning = false;
                        useGravity = true;
                        hasJump = false;

                        return;
                    }

                    velocity.x = 0f;
                    velocity.z = 0f;

                    velocity -= infoRight.normal * 3f;
                    velocity += wallForward * maxSpeed * slideBoostMultiplier * slideBoostMultiplier;

                    Vector3 facing = transform.forward;
                    transform.forward = wallForward;
                    baseBody.transform.forward = facing;
                }

                if (!jump) hasJump = true;

                if (jump && hasJump)
                {
                    Debug.Log("jumping off right wall");

                    velocity = Vector3.Project(velocity, wallForward);
                    velocity += infoRight.normal * ejectionSpeed / 3 * 2;
                    velocity += head.transform.forward * ejectionSpeed / 3;
                    velocity += new Vector3(0, jumpHeight, 0);
                    wallRunning = false;
                    useGravity = true;
                    hasJump = false;

                    /*
                    Vector3 facing = new Vector3(head.transform.forward.x, 0, head.transform.forward.z);
                    Vector3 bodyDirection = new Vector3(head.transform.rotation.eulerAngles.x, 0, 0);
                    head.transform.rotation = Quaternion.Euler(bodyDirection);
                    transform.forward = facing;
                    */

                    return;
                }

                velocity += (Physics.gravity / 2f) * Time.deltaTime;

                transform.forward = wallForward;
            }
            else
            {
                wallRunning = false;
                useGravity = true;

                //transform.forward = baseBody.transform.forward;
                //baseBody.transform.localRotation = Quaternion.identity;
            }
        }
        else
        {
            wallRunning = false;
            useGravity = true;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!wallRunning)
        {
            var p = Vector3.Project(velocity, -hit.normal);
            velocity -= p;
        }
    }
    #endregion

    #region Slide / Crouch
    private bool crouching, sliding;
    public bool Crouching { get { return crouching; } }
    public bool Sliding { get { return sliding; } }
    [SerializeField]
    private float slideDecrement = 0.50f, slideBoostMultiplier = 1.5f;
    void Crouch()
    {
        crouching = true;

        Debug.Log("Crouching!!!");
        //baseBody.transform.localPosition = baseBody.transform.localPosition + new Vector3(0, -0.5f, 0);
        //legs.SetActive(false);

        controller.height = 1;
        controller.center = new Vector3(0, 0.2f, 0);
        transform.Translate(0, 0.3f, 0);

        //if sprinting Slide()
        if (sprinting) StartCoroutine(StartSlide());
        else
        {
            //actual latent crouch, unique anim, change sound package
        }
    }
    void Uncrouch()
    {
        crouching = false;
        wasHoldingCrouch = false;

        Debug.Log("Uncrouching!!!");
        //baseBody.transform.localPosition = baseBody.transform.localPosition + new Vector3(0, 0.5f, 0);
        //legs.SetActive(true);

        controller.height = 2f;
        controller.center = new Vector3(0, 0, 0);
        gameObject.transform.Translate(0, 1, 0);

        //if sliding return to sprint -> Unslide();
        if (sliding) StartCoroutine(Unslide());
    }
    private IEnumerator StartSlide()
    {
        sliding = true;

        //velocity = velocity.normalized * sprintMaxSpeed * 1.35f; //recode to ignore y

        Vector2 xzVel = new Vector2(velocity.x, velocity.z).normalized;

        xzVel = xzVel * sprintMaxSpeed * slideBoostMultiplier; //or currSpeed instead of sprintmax, whichever is lower

        velocity.x = xzVel.x;

        velocity.z = xzVel.y;

        controller.height = 1f;
        controller.center = new Vector3(0, 0.2f, 0);

        yield return null;

    }
    private void CheckGroundedSliding()
    {
        grounded = Physics.SphereCast(physicalHips.position + new Vector3(0, 0.3f, 0),
            0.3f, -transform.up, out groundInfo, jumpHeight / 60, ground);

        //gj
        grounded = Physics.Raycast(waist.transform.position + new Vector3(0, 0.1f, 0), -transform.up, 0.3f, ground);
        Debug.Log($"Grounded sliding: {grounded}");
    }
    private void Slide() // this function is only called if(sliding && grounded) so that you don't lose speed in the air; may have jumping end all slides/crouches, but being airborne won't prevent starting one
    {
        CheckGroundedSliding();
        if (grounded)
        {

            velocity = new Vector3(velocity.x - (velocity.x * slideDecrement * Time.deltaTime), velocity.y - (velocity.y * slideDecrement * Time.deltaTime), velocity.z - (velocity.z * slideDecrement * Time.deltaTime));
            if (velocity.magnitude < crouchMaxSpeed / 2)
            {
                Debug.Log("No longer sliding.. too slow.");
                velocity = Vector3.zero;
                Unslide();
            }
        }
    }
    private IEnumerator Unslide()
    {
        sliding = false;
        //head.transform.Translate(new Vector3(0, 0.3f, 0));

        controller.height = 2f;
        controller.center = new Vector3(0, 0, 0);
        gameObject.transform.Translate(0, 1, 0);

        yield return null;
    }

    #endregion

    #region Muscle Up
    [Header("Muscle Up")]
    [SerializeField]
    private LayerMask ground;
    [SerializeField]
    private float climbSpeed;
    private bool ledge = false, ledgeTooHigh = false;
    private int ledgeHeight;
    private RaycastHit ledgeInfo, ledgeInfo2, ledgeInfo3;

    private void CheckMuscleUp()
    {
        return; // we don't wanna use this anymore
        //redo, as follows:
        /*
         cast ray from highest (0.25m in front of player), straight down (layermask ground).
         if hit within (highest-playerHeight), and player is moving downward,
         send a ray of length playerHeight straight up, and if it hits nothing, there's enough
         room to stand up; BONUS if not, is there enough room to crouch up?
        */
        if (Physics.Raycast(highest.position, new Vector3(0, -1, 0), out ledgeInfo, 1.5f, ground)) // if I reach terrain before I reach my waist
        {
            if (Physics.Raycast(highest.position, new Vector3(0, -1, 0), out ledgeInfo2, 1f, ground)) // then if I reach it before my shoulders
            {
                if (Physics.Raycast(highest.position, new Vector3(0, -1, 0), out ledgeInfo3, .05f, ground)) // then if it's right where my hands are
                {
                    muscleUp = 3;
                    StartCoroutine(MuscleUp(ledgeInfo3)); // I'll climb it
                }
                else if (Physics.Raycast(highest.position + new Vector3(0, -0.95f, 0), new Vector3(0, -1, 0), out ledgeInfo, 0.05f, ground)) // if it's not, but it's right at my shoulders
                {
                    muscleUp = 2;
                    StartCoroutine(MuscleUp(ledgeInfo2)); // I'll climb it
                }

            }
            else if (Physics.Raycast(highest.position + new Vector3(0, -1.45f, 0), new Vector3(0, -1, 0), out ledgeInfo, 0.05f, ground)) // if it's not those, but it's at my waist
            {
                muscleUp = 1;
                StartCoroutine(MuscleUp(ledgeInfo)); // I'll climb it
            }

            // otherwise no climbing, I'll wait to fall into a place that the IK will look right in
        }

        /*
         * change the top to a wasTrue model, so if (you can catch the ledge with your waist && that was not true last frame) (you've fallen in line fo IK-ing)
         * 
         * nvm
         * 
         */

        // one from head.transform, out 0.5 m
        // change to not mask but ask ledgeInfo if it's ground
        // so there's nothing between you and the ground piece (LATER - MJB)
        // if yes, one from some calculated amount above head.transform, out dist+.01f where dist is the amount the first one traveled before contact

        /*
        if (Physics.Raycast(waist.transform.position, waist.transform.forward, out ledgeInfo3, controller.radius + rayOut))
        {
            ledgeHeight = 1;
            ledge = true;
        }
        if (Physics.Raycast(shoulders.transform.position, shoulders.transform.forward, out ledgeInfo2, controller.radius + rayOut))
        {
            ledgeHeight = 2;
            ledge = true;
        }
        if (Physics.Raycast(highest.transform.position, highest.transform.forward, out ledgeInfo, controller.radius + rayOut))
        {
            ledgeHeight = 3;
            ledge = true;
        }


        if (ledge)
            ledgeTooHigh = Physics.Raycast(tooHigh.transform.position + new Vector3(0, 1f, 0), tooHigh.transform.forward, 0.6f);
        */
    }
    private IEnumerator MuscleUp(RaycastHit info)
    {
        //a check upwards using info
        if (Physics.Raycast(info.point, new Vector3(0, 1, 0), controller.height) || Physics.Raycast(head.transform.position, new Vector3(0, 1, 0), ((info.point + new Vector3(0, controller.height, 0)) - (head.transform.position + new Vector3(0, 0, highest.position.z - head.transform.position.z))).magnitude))
        {   // I'm so sorry this looks gross but it does exactly what it's supposed to do and this is literally the most elegant way to do it that will remain mathematically accurate no matter what we do to the player prefab
            // This just says "well would the player's head end up in something if we let them stand up there?" || "would they climb through somthing on their way there?"
            // (AND TO ADD LATER || "could they climb straight up but moving forward would send them through a thin wall because the map was poorly made?" )

            // if so, just don't do any of the fun coroutine stuff
            Debug.Log("Can't climb this.");
            yield return null;
        }
        else // do ALL the fun coroutine stuff
        {
            //this entire area needs to be rewritten I think
            parkouring = true;

            //how high must I climb
            float progress = 0f, goal = info.point.y - (feet.transform.position.y) + .1f;

            //calculate goal
            //goal =/* difference between (top of structure+0.01f) and bottom of controller */-99f;
            //// renderer/collider    .bounds.size, then you have the middle, and the height, half height - players amount above middle (y compared to wall y)
            //goal = (ledgeCollider.transform.position.y + (ledgeCollider.bounds.size.y / 2)) - transform.position.y + 0.125f;


            //Need to find a new way to twist the legs, probably a ray that comes out just below where the hands will IK and returns the normal of what it hits

            // ALSO CHECK WHEN IT RUNS CheckMuscleUp() should be different logic in move() now

            //Vector3 facing = transform.forward;

            //if (ledgeHeight == 3)
            //{
            //    goal = 2.75f;
            //    transform.forward = -ledgeInfo.normal;
            //}
            //else if (ledgeHeight == 2)
            //{
            //    goal = 1.75f;
            //    transform.forward = -ledgeInfo2.normal;
            //}
            //else
            //{
            //    goal = 1.25f;
            //    transform.forward = -ledgeInfo3.normal;
            //}

            //baseBody.transform.forward = facing;

            Debug.Log("Goal: " + goal);

            while (progress < goal)
            {
                //continue lerping up to exactly ___ above the ledge
                //if there, parkouring=false
                //else yield return null;

                //Debug.Log("Progress: " + progress);

                //rewrite this to falsify climbing outside of the loop, and make it just precalculate and lerp
                //over a time that scales with distance
                velocity = new Vector3(0, climbSpeed, 0);
                progress += climbSpeed * Time.deltaTime;

                MovePlayer(velocity);
                yield return null;
            }

            //math to convert climbspeed into facing
            velocity = new Vector3(transform.forward.x * climbSpeed, 0, transform.forward.z);

            while (progress < goal + 0.5)
            {
                progress += climbSpeed * Time.deltaTime;

                MovePlayer(velocity);
            }
            Debug.Log("ESCAPE!");

            parkouring = false;
            ledge = false;
            ledgeTooHigh = false;
            //sprinting = false; //maybe
        }
    }

    #endregion

    #region Vault
    [Header("Vaulting")]
    [SerializeField]
    private LayerMask vaultable;
    RaycastHit vaultInfo;
    public void CheckVault()
    {
        return; // we don't wanna do this anymore

        if (Physics.Raycast(waist.position + new Vector3(0, -0.35f, 0), waist.forward, out vaultInfo, 2))
        {


            Debug.Log("into the if in CheckVault()\n " + vaultInfo.collider.gameObject.layer.ToString());
            if (vaultInfo.collider.gameObject.layer.ToString().Equals("12"))
            {
                Debug.Log("Happy time");
                StartCoroutine(AlignVault());
            }
        }
        //Debug.Log("unfinished - PlayerBody.cs #region Vault ");
        //if good
        //start coroutine AlignVault
    }

    public IEnumerator AlignVault()
    {
        if (!vault)
        {

            parkouring = true;
            float count = 0;
            Debug.Log("We made it into AlignVault()");

            //DEFINITELYmaybe turn the player so that they're running the right direction (into the normal of ledgeInfo)
            Vector3 worldFacing = transform.forward;
            transform.forward = new Vector3(-vaultInfo.normal.x, 0, -vaultInfo.normal.z);
            //head.transform.forward = transform.worldToLocalMatrix * worldFacing;
            //Vector3 diff = new Vector3(worldFacing.x - transform.forward.x, head.transform.forward.y, worldFacing.z - transform.forward.z);
            //head.transform.forward = diff;

            bool reachedGoal;

            while (/*not there yet*/ true)
            {
                reachedGoal = Physics.Raycast(waist.position + new Vector3(0, -0.12f, 0), waist.forward, 0.3f);
                if (Physics.Raycast(waist.position + new Vector3(0, -0.35f, 0), waist.forward, out vaultInfo, 0.8f))
                {
                    Debug.Log("Vaulting");
                    StartCoroutine(Vault());
                    break;
                }

                yield return null;

                if (!vault)
                {
                    MovePlayer(velocity);
                    count += Time.deltaTime;
                    if (count > 1.25f)
                    {
                        Debug.Log("Something went wrong with the vault.");
                        parkouring = false;
                        break;
                    }

                }

            }

            yield return null;
        }
        yield return null;
    }

    public IEnumerator Vault()
    {
        vault = true;

        /*
         down 10cm in the first 6 frames (0.2s)
         over the next 7 frames up 36
         15 frames to go back down 26
         
         */
        /*
         from -62 => -28  from frame 13-17
         */

        Vector3 forwardSpeed = new Vector3(0, 0, 8.5f);


        Debug.Log("vaulting successfully");

        //I know the exact height, and that I'm the right distance, so call anim, waitForSeconds(length of anim), unparkouring?

        //MovePlayer(new Vector3(0, 1, 0) / Time.deltaTime);
        //controller.height = 0f;
        //controller.center = new Vector3(0, 0.5f, 0);

        //controller.enabled = false;

        //for (float count = 0; count < 0.2f; count += Time.deltaTime)
        //{
        //    transform.Translate(forwardSpeed * Time.deltaTime);
        //    transform.Translate(transform.worldToLocalMatrix * new Vector3(0, -0.5f, 0) * Time.deltaTime);

        //    yield return null;
        //}
        for (float count = 0; count < 0.2333333f; count += Time.deltaTime)
        {
            transform.Translate(forwardSpeed / 2 * Time.deltaTime);
            transform.Translate(transform.worldToLocalMatrix * new Vector3(0, 0.26f / 0.2333f, 0) * Time.deltaTime);

            yield return null;
        }
        for (float count = 0; count < 0.353333f; count += Time.deltaTime)
        {
            transform.Translate(forwardSpeed * Time.deltaTime);
            transform.Translate(transform.worldToLocalMatrix * new Vector3(0, 0.26f / 0.5f, 0) * Time.deltaTime);

            yield return null;
        }
        //MovePlayer(new Vector3(0, -1, 0) / Time.deltaTime);
        //controller.height = 2f;
        //controller.center = new Vector3(0, 1f, 0);

        //controller.enabled = true;

        //yield return new WaitForSeconds(.8f);

        parkouring = false;
        vault = false;
    }
    #endregion

    #region Landing
    private bool landingBoost = false;
    [Header("Landings")]
    [SerializeField]
    private float landingPeriod = 0f;

    private IEnumerator Landing()
    {
        for (float i = 0; i < landingPeriod; i += Time.deltaTime)
        {
            if (jump)
            {
                StartCoroutine(LandingBoost());
                break;
            }

            yield return null;
        }
        if (!landingBoost) StartCoroutine(BadLanding());

    }

    private IEnumerator LandingBoost()
    {
        landingBoost = true;
        landingBoost = false;

        //extra speed for a little bit
        yield return new WaitForSeconds(1f); // REPLACE


    }

    [SerializeField]
    float stumblePeriod = 0.66f;
    private IEnumerator BadLanding()
    {
        parkouring = true;

        //call stumbleAnim

        for (float i = 0; i < stumblePeriod; i += Time.deltaTime)
            yield return null;
    }
    #endregion

   


}