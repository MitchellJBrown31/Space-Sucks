using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    

    public Direction direction;

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") RoomManager.Instance.LoadNextRoom(direction == Direction.North ? new Vector3(0, 0, 1) : direction == Direction.East ? new Vector3(1, 0, 0) : direction == Direction.South ? new Vector3(0, 0, -1) : new Vector3(-1, 0, 0));
        //this is a REALLY long line that assumes you're most likely to go through the top door, if not then the right, then bottom, then left. switch directions to most -> least likely order to optimize
    }
}


