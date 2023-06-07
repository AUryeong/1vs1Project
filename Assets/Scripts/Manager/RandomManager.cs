using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomManager
{
    public static T SelectOne<T>(List<T> tList)
    {
        return tList[Random.Range(0, tList.Count)];
    }
    public static T SelectOne<T>(params T[] tList)
    {
        return tList[Random.Range(0, tList.Length)];
    }
}
