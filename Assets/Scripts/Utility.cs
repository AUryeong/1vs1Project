using UnityEngine;

public static class Utility
{
    public static Color FadeChange(this Color color, float fade)
    {
        return new Color(color.r, color.g, color.b, fade);
    } 
}