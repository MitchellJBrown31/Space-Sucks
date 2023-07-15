using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField]
    private PlayerBody body;
    [SerializeField]
    private GameObject movementSetTest;
    [SerializeField]
    private GameObject m16Magazine, akMagazine, m16MagazineLeft, akMagazineLeft;

    [SerializeField]
    private Animator anim;

    [SerializeField]
    private float x, z, deadzone = 0.05f;
    private bool wasGrounded = false, wasCrouched = false, isReloading = false;
    private bool freeToMove;

    [SerializeField]
    private bool test;

    // Update is called once per frame
    void Update()
    {
        if (!test)
        {
            freeToMove = false;

            //if (body.Parkouring) ParkourUpdate();
            if (body.WallRunning)
            {
                anim.SetBool("Idle", false);

                if (body.WallRunLeft)
                {
                    anim.SetBool("WallrunLeft", true);
                    anim.SetBool("WallrunRight", false);
                }
                else
                {
                    anim.SetBool("WallrunLeft", false);
                    anim.SetBool("WallrunRight", true);
                }
            }
            else if (body.Sliding)
            {
                anim.SetBool("Idle", false);

                anim.SetBool("Slide", true);
            }
            else
            {
                if(body.sprint) anim.SetBool("Running", true);

                //#region Regular Input
                //anim.SetBool("slide", false);
                //anim.SetBool("wallrunLeft", false);
                //anim.SetBool("wallrunRight", false);

                //freeToMove = true;

                //x = 0;
                //z = 0;

                if (body.jump && wasGrounded) StartCoroutine(Jump());
                else anim.SetBool("Jump", false); //if the jump anim was called last frame, not again

                ////if (body.IsGrounded)
                ////{
                ////    wasGrounded = true;

                ////    if (ctrl.Controls.forward) z = 1;
                ////    if (ctrl.Controls.right) x = 1;

                ////    if (ctrl.Controls.back) z--;
                ////    if (ctrl.Controls.left) x--;
                ////}
                ////else
                ////{
                ////    wasGrounded = false;
                ////}

                //if (body.Sprinting && z > 0) anim.SetFloat("Z", z + 1);
                //else anim.SetFloat("Z", z);

                //anim.SetFloat("X", x);

                //if (z <= deadzone && x <= deadzone)
                //{
                //    if (z >= 0 - deadzone && x >= 0 - deadzone) anim.SetBool("idle", true);
                //    else anim.SetBool("idle", false);
                //}
                //else anim.SetBool("idle", false);

                //#endregion

                //#region Crouch

                //if (body.Crouching)
                //{
                //    anim.SetBool("idle", false);

                //    anim.SetBool("crouch", true);

                //    wasCrouched = true;
                //}
                //else
                //{
                //    anim.SetBool("crouch", false);
                //    if (wasCrouched)
                //    {
                //        StartCoroutine(Uncrouch());
                //    }

                //}

                //#endregion

                //#region Reload

                //if (body.GlockReload && !isReloading)
                //{
                //    StartCoroutine(GlockReload());
                //}
                //else if (body.M16Reload && !isReloading)
                //{
                //    StartCoroutine(M16Reload());
                //    StartCoroutine(M16MagReload());
                //}
                //else if (body.AkReload && !isReloading)
                //{
                //    StartCoroutine(AkReload());
                //    StartCoroutine(AkMagReload());
                //}

                //#endregion

                //#region Shoot

                //if (body.GlockShoot)
                //{
                //    anim.SetBool("GlockShoot", true);
                //}
                //else
                //{
                //    anim.SetBool("GlockShoot", false);
                //}
                //if (body.M16Shoot)
                //{
                //    anim.SetBool("M16Shoot", true);
                //}
                //else
                //{
                //    anim.SetBool("M16Shoot", false);
                //}
                //if (body.AkShoot)
                //{
                //    anim.SetBool("AkShoot", true);
                //}
                //else
                //{
                //    anim.SetBool("AkShoot", false);
                //}

                //#endregion
            }
            anim.SetBool("free", freeToMove);
        }
    }

    void ParkourUpdate()
    {

        if (body.vault) StartCoroutine(VaultAnim());



        if (body.muscleUp != 0) StartCoroutine(MuscleUp());
    }

    private IEnumerator VaultAnim()
    {
        anim.SetBool("vault", true);

        //anim.applyRootMotion=true;
        yield return new WaitForSeconds(0.8f);
        //anim.applyRootMotion = false;

        anim.SetBool("vault", false);
        //yield return new WaitForSeconds(0.1f);
        //movementSetTest.transform.localPosition = new Vector3(0, 0, 0); //yes??
    }

    private IEnumerator Jump()
    {
        anim.SetBool("jump", true);

        //anim.applyRootMotion=true;
        yield return new WaitForSeconds(0.4667f);
        //anim.applyRootMotion = false;

        anim.SetBool("jump", false);
    }

    private IEnumerator Uncrouch()
    {
        wasCrouched = false;
        anim.SetBool("unCrouch", true);

        yield return new WaitForSeconds(0.3667f);

        anim.SetBool("unCrouch", false);
    }

    private IEnumerator MuscleUp()
    {
        int m = body.muscleUp;
        anim.SetInteger("muscleUp", m);
        if (m == 3)
        {
            yield return new WaitForSeconds(1.57f);
        }
        else if (m == 2)
        {
            yield return new WaitForSeconds(1.17f);
        }
        else if (m == 1)
        {
            yield return new WaitForSeconds(.6f);
        }

        anim.SetInteger("muscleUp", 0);

        yield return null;
    }

    //#region WeaponAnimationCoroutine
    //private IEnumerator GlockReload()
    //{

    //    anim.SetBool("GlockReload", true);
    //    isReloading = true;

    //    yield return new WaitForSeconds(1.14f);

    //    anim.SetBool("GlockReload", false);
    //    isReloading = false;
    //    body.GlockReload = false;
    //}

    //private IEnumerator AkReload()
    //{

    //    anim.SetBool("AkReload", true);
    //    isReloading = true;

    //    yield return new WaitForSeconds(1.14f);

    //    anim.SetBool("AkReload", false);
    //    isReloading = false;
    //    body.AkReload = false;
    //}

    //private IEnumerator M16Reload()
    //{

    //    anim.SetBool("M16Reload", true);
    //    isReloading = true;

    //    yield return new WaitForSeconds(1.14f);

    //    anim.SetBool("M16Reload", false);
    //    isReloading = false;
    //    body.M16Reload = false;
    //}

    //private IEnumerator M16MagReload()
    //{

    //    yield return new WaitForSeconds(0.055f);

    //    m16Magazine.SetActive(false);
    //    m16MagazineLeft.SetActive(true);

    //    yield return new WaitForSeconds(2.02f);

    //    m16Magazine.SetActive(true);
    //    m16MagazineLeft.SetActive(false);
    //}
    //private IEnumerator AkMagReload()
    //{

    //    yield return new WaitForSeconds(0.51f);

    //    akMagazine.SetActive(false);
    //    akMagazineLeft.SetActive(true);

    //    yield return new WaitForSeconds(1.52f);

    //    akMagazine.SetActive(true);
    //    akMagazineLeft.SetActive(false);
    //}
    //#endregion

}