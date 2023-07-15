using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Murder : MonoBehaviour
{
    public float duration = 1000.0f;

    private void Update()
    {
        duration -= Time.deltaTime;

        if(duration<0)
        {
            if (GameObject.FindGameObjectWithTag("Player").transform.position.y - transform.position.y > 1) Lose();
        }
        if(duration<-30)
            GameObject.Destroy(gameObject);

    }

    private void Lose()
    {
        Debug.Log("You Lose");
    }
}
