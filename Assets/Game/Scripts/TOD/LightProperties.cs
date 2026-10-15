using UnityEngine;

[System.Serializable]
public class LightProperties
{
    public enum PropertyType
    {
        Float,
        Color
    }

    public PropertyType type;
    public string propertyName;

    public AnimationCurve curve;
    public Gradient gradient;
}