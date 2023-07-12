using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class PoolData
{
    [Title("Pool Data")] 
    [AssetList]
    public GameObject origin;

    [HideInInspector] public Transform parent;

    [Space(20f)]
    [ShowIf("@origin != null")]
    public List<GameObject> poolingObjects = new List<GameObject>();
}

public class PoolManager : Singleton<PoolManager>
{
    protected override bool IsDontDestroying => true;
    
    [Searchable]
    [DictionaryDrawerSettings(KeyLabel = "Name", ValueLabel = "Pool Data")]
    [SerializeField]
    private Dictionary<string, PoolData> pools = new Dictionary<string, PoolData>();

    protected override void OnCreated()
    {
        foreach (var pool in pools)
        {
            var trans = new GameObject(pool.Key).transform;
            trans.SetParent(transform);
            pool.Value.parent = trans;
        }
    }

    protected override void OnReset()
    {
        DisableAllObjects();
    }

    public void AddPoolData(string poolName, GameObject origin)
    {
        var trans = new GameObject(poolName).transform;
        trans.SetParent(transform);

        var poolData = new PoolData()
        {
            origin = origin,
            parent = trans
        };
        pools.Add(poolName, poolData);
    }

    public void DisableAllObjects()
    {
        foreach (var pool in pools.Values)
            foreach (var obj in pool.poolingObjects)
                obj.gameObject.SetActive(false);
    }

    public GameObject Init(string origin, bool isParent = true)
    {
        if (string.IsNullOrEmpty(origin) || !pools.ContainsKey(origin))
        {
            Debug.LogAssertion("Cannot Found Pooling : " + origin);
            return null;
        }

        var poolData = pools[origin];
        GameObject copy;
        if (poolData.poolingObjects.Count > 0)
        {
            if (poolData.poolingObjects.FindAll(obj => !obj.activeSelf).Count > 0)
            {
                copy = poolData.poolingObjects.Find(obj => !obj.activeSelf);
                copy.SetActive(true);
                return copy;
            }
        }

        if (isParent)
            copy = Instantiate(poolData.origin, poolData.parent);
        else
            copy = Instantiate(poolData.origin);
        pools[origin].poolingObjects.Add(copy);
        copy.SetActive(true);
        return copy;
    }
}