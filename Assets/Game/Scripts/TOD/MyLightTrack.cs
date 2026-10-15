using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Timeline;

[TrackClipType(typeof(MyLightAsset))] 
[TrackBindingType(typeof(Light2D))] 
public class MyLightTrack : TrackAsset
{

}