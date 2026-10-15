using System.Collections.Generic; // 为了使用 List
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.Universal;

public class MyLightBehaviour : PlayableBehaviour
{
    public List<LightProperties> properties;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        Light2D light = playerData as Light2D;
        if (light == null) return;


        float t = (float)(playable.GetTime() / playable.GetDuration()); // 缩放到0-1的范围

        foreach (var property in properties) // 遍历所有属性
        {
            if (property.propertyName == "Intensity") // 如果是float类型
            {
                float value = property.curve.Evaluate(t);
                //light.SetFloat(property.propertyName, value);
                light.intensity = value;
            }
            if (property.propertyName == "Color") // 如果是color类型
            {
                Color color = property.gradient.Evaluate(t);
                light.color = color;
            }
        }
    }
}