using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static Color FadeChange(this Color color, float fade)
    {
        return new Color(color.r, color.g, color.b, fade);
    } 
    public static T SelectOne<T>(this List<T> tList)
    {
        return tList[Random.Range(0, tList.Count)];
    }
}