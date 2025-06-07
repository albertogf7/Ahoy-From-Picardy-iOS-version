using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarButton : MonoBehaviour
{
    [SerializeField]
    private int pillarID; // The ID of this pillar
    [SerializeField]
    private TrainTracksBehaviour tracksScript; // Reference to the TrainTracksBehaviour script
    private Collider thisCollider;

    private void Start()
    {
        thisCollider = GetComponent<Collider>(); // Get the Collider component attached to this GameObject
    }

    public void CallTracks()
    {
        // Directly call the SetTrack method with the pillarID
        tracksScript.SetTrack(pillarID);

        // Disable the collider to prevent repeated interactions
        thisCollider.enabled = false;
    }
}
