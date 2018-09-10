using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudCanvas : MonoBehaviour {
    public static HudCanvas instance;

	// Use this for initialization
	void Awake () {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void submitMove() {
        MirrorLine.instance.acceptMove();
    }
}
