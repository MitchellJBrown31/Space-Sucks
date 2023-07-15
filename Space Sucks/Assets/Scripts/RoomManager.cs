using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{

    public Vector3 roomOffset;
    public bool flipped;

    private static RoomManager instance;
    public static RoomManager Instance
    {
        get { return instance; }
    }

    public void Awake()
    {
        instance = this;
    }

    public List<GameObject> rooms = new List<GameObject>();

    public List<GameObject> liveRooms = new List<GameObject>();

    public void LoadNextRoom()
    {
        Debug.Log("Roomicus Kabloomicus!");
        GameObject tile = Instantiate(rooms[(int)(Random.value * rooms.Count)], roomOffset, Quaternion.identity);
        if(flipped) tile.transform.Rotate(new Vector3(0, 180, 0));

        liveRooms.Add(tile);
        if (liveRooms.Count > 2)
        {
            GameObject.Destroy(liveRooms[0]);
            liveRooms.RemoveAt(0);
        }
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
