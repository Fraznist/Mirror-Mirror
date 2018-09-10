using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseCanvas : MonoBehaviour {
    public static PauseCanvas instance;

    // Use this for initialization
    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void LineDrawToggle() {
        GameManager.instance.LineDrawToggle();
        MirrorLine.instance.setDrawBehavior(GameManager.instance.isDragMode);
        GameObject button = GameObject.Find("MoveAcceptButton");
        button.GetComponent<Image>().enabled = !GameManager.instance.isDragMode;
        button.GetComponent<Button>().enabled = !GameManager.instance.isDragMode;
    }
}
