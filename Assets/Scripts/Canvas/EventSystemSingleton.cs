using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSystemSingleton : MonoBehaviour {
    public static EventSystemSingleton instance;

    // Use this for initialization
    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
}
