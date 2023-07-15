using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector3 netMove;

    public float speed;
    
    void Update()
    {
        if (Input.GetKey(KeyCode.W)) netMove += new Vector3(0, 0, 1.0f);
        if (Input.GetKey(KeyCode.S)) netMove += new Vector3(0, 0, -1.0f);

        if (Input.GetKey(KeyCode.A)) netMove += new Vector3(-1.0f, 0, 0);
        if (Input.GetKey(KeyCode.D)) netMove += new Vector3(1.0f, 0, 0);

        netMove.Normalize();
        netMove *= speed * Time.deltaTime;

        transform.position += netMove;

        netMove = Vector3.zero;
    }
}
