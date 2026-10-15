using System.Collections.Generic; // 为了使用 List
using UnityEngine;
using UnityEngine.Playables;

public class MyLightAsset : PlayableAsset
{
    public List<LightProperties> properties; // 用于存储所有需要调整的属性

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<MyLightBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();

        behaviour.properties = properties;

        return playable;
    }
}