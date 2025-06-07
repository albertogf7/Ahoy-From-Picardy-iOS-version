using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TrainTracksBehaviour : MonoBehaviour
{
    [SerializeField]
    private PlayableDirector[] tracksTimelines; // Array of timelines for each track
    [SerializeField]
    private BoxCollider[] pillarColliders; // Renamed for consistency (no underscore)
    private int laidTracks = 0; // Counter for the number of tracks laid
    [SerializeField]
    private PlayableDirector transformationTimeline; 
    private bool allTracksSet = false; 
    [SerializeField]
    private float loopAtTime; 
    [SerializeField]
    private GameObject trainSoundObject;
    private bool pillarsColliderEnabled = false;

    // Optimization: Cache the number of tracks
    private int totalTracks;

    private void Awake() // Changed from Start to Awake for earlier initialization
    {
        // Optimization: Initialize totalTracks in Awake
        totalTracks = tracksTimelines.Length;
        trainSoundObject.SetActive(false);

        // Optimization: Disable pillar colliders at the start
        SetPillarCollidersEnabled(false);
    }

    public void CheckLoop()
    {
        if (!allTracksSet)
        {
            transformationTimeline.time = loopAtTime;
        }
    }

    // Method to set a track based on the pillar ID
    public void SetTrack(int trackId)
    {
        // More concise check for valid trackId
        if (trackId >= 0 && trackId < totalTracks)
        {
            tracksTimelines[trackId].Play();
            laidTracks++;
            CheckTracks();
        }
        else
        {
            Debug.LogError($"Invalid trackId: {trackId}"); // Improved error message
        }
    }

    // Check if all tracks have been laid
    private void CheckTracks()
    {
        // More direct comparison with totalTracks
        if (laidTracks == totalTracks && !allTracksSet)
        {
            Debug.Log("All Tracks Set");
            allTracksSet = true;
            trainSoundObject.SetActive(true);
        }
        else if (laidTracks < totalTracks)
        {
            Debug.Log("Must lay more tracks");
        }
    }

    public void EnablePillarCollider()
    {
        SetPillarCollidersEnabled(true);
    }

    // Helper method to enable/disable pillar colliders
    private void SetPillarCollidersEnabled(bool enabled)
    {
        if (pillarsColliderEnabled != enabled)
        {
            foreach (var collider in pillarColliders)
            {
                collider.enabled = enabled;
            }
            pillarsColliderEnabled = enabled;
        }
    }
}
