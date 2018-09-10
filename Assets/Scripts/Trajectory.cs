using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory : MonoBehaviour {
    // static variable for singleton class
    public static Trajectory instance = null;

    [HideInInspector] public LineRenderer trail;
    public Material validTraj;
    public Material invalidTraj;

	// Use this for initialization
	void Awake () {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        trail = GetComponent<LineRenderer>();
        trail.sortingLayerName = "TopLayer";
    }

    public void resetTrajectory() {
        trail.positionCount = 0;
        setValidity(true);
    }

    public void addEdge(Vector2 newEdge) {
        trail.positionCount++;
        trail.SetPosition(trail.positionCount - 1, newEdge);
    }

    public void setValidity(bool valid) {
        if (valid)
            trail.sharedMaterial = validTraj;
        else
            trail.sharedMaterial = invalidTraj;
    }
}
