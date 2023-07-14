/*
using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{

    [SerializeField]
    private PlayerBody body;

    private int fireMode;
    private void Start()
    {
        this.body = GetComponent<PlayerBody>();
    }

    public void SetFireMode(int value)
    {
        fireMode = value;
    }

    private PlayerControls controls;
    public PlayerControls Controls { get { return controls; } }

    private void Update()
    {
        if (hasAuthority)
        {
            if (Input.GetKey(Settings.Controls.moveForward)) controls.forward = true;
            else controls.forward = false;
            if (Input.GetKey(Settings.Controls.moveLeft)) controls.left = true;
            else controls.left = false;
            if (Input.GetKey(Settings.Controls.moveBack)) controls.back = true;
            else controls.back = false;
            if (Input.GetKey(Settings.Controls.moveRight)) controls.right = true;
            else controls.right = false;
            if (Input.GetKeyDown(Settings.Controls.jump)) controls.jump = true;
            else controls.jump = false;
            if (Input.GetKey(Settings.Controls.jump)) controls.jumpH = true;
            else controls.jumpH = false;
            if (Input.GetKeyDown(Settings.Controls.toggleSprint)) controls.sprint = true; /*OLD { controls.sprint = !controls.sprint; }*/
/*
            else controls.sprint = false;
            if (Input.GetKey(Settings.Controls.crouchH)) controls.crouchH = true;
            else controls.crouchH = false;
            if (Input.GetKeyDown(Settings.Controls.crouchT)) controls.crouchT = true;
            else controls.crouchT = false;
            if (fireMode == 0)
            {
                if (Input.GetMouseButton(0)) controls.fire = true;
                else controls.fire = false;

            }
            else if (fireMode == 1)
            {
                if (Input.GetMouseButtonDown(0)) controls.fire = true;
                else controls.fire = false;
            }
            if (Input.GetMouseButton(1)) controls.ads = true;
            else controls.ads = false;
            if (Input.GetKeyDown(Settings.Controls.reload)) controls.reload = true;
            else controls.reload = false;
            if (Input.GetKeyDown(Settings.Controls.interact)) controls.interact = true;
            else controls.interact = false;
            if (Input.GetKeyDown(Settings.Controls.primaryWeapon)) controls.primaryWeapon = true;
            else controls.primaryWeapon = false;
            if (Input.GetKeyDown(Settings.Controls.secondaryWeapon)) controls.secondaryWeapon = true;
            else controls.secondaryWeapon = false;

            body.mouseXInput = Input.GetAxis("Mouse X");
            body.mouseYInput = Input.GetAxis("Mouse Y");

            body.forwardBool = controls.forward;
            body.left = controls.left;
            body.back = controls.back;
            body.right = controls.right;
            body.jump = controls.jump;
            body.jumpH = controls.jumpH;
            body.sprint = controls.sprint;
            body.crouchH = controls.crouchH;
            body.crouchT = controls.crouchT;
            body.fire = controls.fire;
            body.ads = controls.ads;
            body.reload = controls.reload;
            body.primaryWeapon = controls.primaryWeapon;
            body.secondaryWeapon = controls.secondaryWeapon;
            body.interact = controls.interact;
        }
    }

}


[System.Serializable]
public struct PlayerControls
{
    public bool forward, left, back, right, jump, jumpH, sprint, crouchH, crouchT, fire, ads, reload, interact, primaryWeapon, secondaryWeapon;
}

*/