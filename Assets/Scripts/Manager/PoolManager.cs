using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{

    private Dictionary<GameObject, List<GameObject>> pools = new Dictionary<GameObject, List<GameObject>>();

    public void GameEnd()
    {
        foreach (List<GameObject> pool in pools.Values)
            foreach (var obj in pool)
                obj.gameObject.SetActive(false);
    }
    public void AddPooling(GameObject origin, Transform parent)
    {
        if (!pools.ContainsKey(origin))
        {
            pools.Add(origin, new List<GameObject>());
        }
        for (int i = 0; i < parent.childCount; i++)
        {
            GameObject obj = parent.GetChild(i).gameObject;
            if (obj != origin)
            {
                pools[origin].Add(obj);
                DontDestroyOnLoad(obj);
            }
        }
    }
    public GameObject Init(GameObject origin)
    {
        if (origin != null)
        {
            GameObject copy = null;
            if (pools.ContainsKey(origin))
            {
                if (pools[origin].FindAll((GameObject x) => !x.activeSelf).Count > 0)
                {
                    copy = pools[origin].Find((GameObject x) => !x.activeSelf);
                    copy.SetActive(true);
                    return copy;
                }
            }
            else
            {
                pools.Add(origin, new List<GameObject>());
            }
            copy = Instantiate(origin);
            DontDestroyOnLoad(copy);
            pools[origin].Add(copy);
            copy.SetActive(true);
            return copy;
        }
        else
        {
            return null;
        }
    }

    public override void OnReset()
    {
        foreach (List<GameObject> objs in pools.Values)
            foreach (var obj in objs)
                obj.gameObject.SetActive(false);
    }
}
