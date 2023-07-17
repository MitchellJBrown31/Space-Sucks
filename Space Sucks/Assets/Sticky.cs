using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Sticky : MonoBehaviour
{
    

    public void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player") Lose(); //lose
    }

    void Lose()
    {
        SceneManager.LoadScene("Credits", LoadSceneMode.Single);
    }
}
