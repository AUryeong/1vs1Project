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

    public static Vector3 Beizer(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        var ab = Vector3.Lerp(a, b, t);
        var bc = Vector3.Lerp(b, c, t);

        var abbc = Vector3.Lerp(ab, bc, t);
        return abbc;
    }
}