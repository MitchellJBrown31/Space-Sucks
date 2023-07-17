using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sticky : MonoBehaviour
{
    

    public void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player") other.gameObject.GetComponent<PlayerBody>().sprintMaxSpeed *= 0.25f;
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player") other.gameObject.GetComponent<PlayerBody>().sprintMaxSpeed *= 4.0f;
    }
}
