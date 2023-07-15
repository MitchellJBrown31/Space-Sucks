using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{

    public float roomOffset;

    private static RoomManager instance;
    public static RoomManager Instance
    {
        get { return instance; }
    }

    public List<GameObject> rooms = new List<GameObject>();

    public void LoadNextRoom(Vector3 gateDir)
    {
        GameObject tile = Instantiate(rooms[(int)(Random.value * rooms.Count)], gateDir*roomOffset, Quaternion.identity);
    }

}

public enum Direction
{
    North,
    East,
    South,
    West,
    Up,
    Down
}
