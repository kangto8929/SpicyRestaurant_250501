using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [SerializeField] private List<PoolInfo> _poolInfoList;

    //싱글톤
    public static PoolManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        Init();
    }

    public void Init()
    {
        foreach (PoolInfo info in _poolInfoList)
        {
            for (int i = 0; i < info.InitCount; i++)
            {
                info.PoolQueue.Enqueue(CreateNewObject(info));
            }
        }
    }

    private GameObject CreateNewObject(PoolInfo info)
    {
        GameObject newObject = Instantiate(info.Prefab, info.Container.transform);
        newObject.SetActive(false);
        return newObject;
    }

    private PoolInfo GetPoolByType(EObjectType type)
    {
        foreach (PoolInfo info in _poolInfoList)
        {
            if (type == info.Type)
            {
                return info;
            }
        }
        return null;
    }

    //타입에 맞는 이미지를 가져와서 이미지를 변경해야한다.
    public GameObject GetObject(EObjectType type, string ingredientype)
    {
        PoolInfo info = Instance.GetPoolByType(type);
        GameObject obj = null;
        if (info.PoolQueue.Count > 0)
        {
            obj = info.PoolQueue.Dequeue();
        }
        else
        {
            obj = Instance.CreateNewObject(info);
        }

        obj.transform.position = Vector3.zero;
        obj.GetComponent<Fragment>().SetImage(ingredientype);
        obj.SetActive(true);

        return obj;
    }


    public GameObject GetObject(EObjectType type)
    {
        PoolInfo info = Instance.GetPoolByType(type);
        GameObject obj = null;
        if (info.PoolQueue.Count > 0)
        {
            obj = info.PoolQueue.Dequeue();
        }
        else
        {
            obj = Instance.CreateNewObject(info);
        }

        // Ensure the object's position is reset when retrieved from the pool
        obj.transform.position = Vector3.zero;

        obj.SetActive(true);
        return obj;
    }

    public void ReturnObject(GameObject obj, EObjectType type)
    {
        PoolInfo info = Instance.GetPoolByType(type);
        info.PoolQueue.Enqueue(obj);
        obj.SetActive(false);
    }
}