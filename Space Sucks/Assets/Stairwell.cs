using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairwell : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;

        RoomManager.Instance.roomOffset += new Vector3(0, -20, 0);
        RoomManager.Instance.flipped = !RoomManager.Instance.flipped;
        RoomManager.Instance.LoadNextRoom();
    }
}
