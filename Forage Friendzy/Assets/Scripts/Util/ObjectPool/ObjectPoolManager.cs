using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectPoolManager : MonoBehaviour
{
    private static ObjectPoolManager instance;

    public static ObjectPoolManager Instance
    {
        get { return instance; }
    }

    [SerializeField]
    private ObjectPool[] objectPools;

    private void Start()
    {
        if(instance == null)
            instance = this;
    }

    public GameObject GetPooledObject(PoolTypes type)
    {
        return objectPools[(int)type].GetPooledObject();
    }

    public T GetPooledObjectComponent<T>(PoolTypes type)
    {
        return GetPooledObject(type).GetComponent<T>();
    }

    public ObjectPool GetPool(PoolTypes type)
    {
        return objectPools[(int)type];
    }

    public void Reset()
    {
        for(int i = 0; i < objectPools.Length; i++)
        {
            objectPools[i].Reset();
        }
    }
}

public enum PoolTypes
{
    SFXAudioSource
}