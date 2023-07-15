using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task : MonoBehaviour
{
    public float completion, completionThreshhold, completionValue, stability, stabilityDPS;

    private bool playerNear = false, logging = true;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("he");

        playerNear = true;
        Log("Player near task");
    }

    private void OnTriggerExit(Collider other)
    {
        playerNear = false;
        Log("Player leaving task");
    }

    private void Work()
    {
        completion += Time.deltaTime;
        if (completion > completionThreshhold)
        {
            completion = 0;
            stability += completionValue;
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.E) && playerNear) { Work(); }

        stability -=stabilityDPS*Time.deltaTime;
        if (stability < 0) Lose();
    }

    private void Lose()
    {

    }

    void Log(string msg) { if(logging) Debug.Log(msg); }
}
