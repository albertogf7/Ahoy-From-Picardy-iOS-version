using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

[TrackClipType(typeof(HapticClip))]
[TrackBindingType(typeof(HapticFeedback))]
public class HapticTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<HapticTrackMixer>.Create(graph, inputCount);
    }
}