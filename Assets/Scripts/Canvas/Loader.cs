using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour {
    public GameObject mirrorLine;
    public GameObject trajectory;
    public GameObject hudCanvas;
    public GameObject pauseCanvas;
    public GameObject gameOverCanvas;
    public GameObject winCanvas;

    // Use this for initialization
    void Awake () {
        if (MirrorLine.instance == null)
            Instantiate(mirrorLine);
        if (Trajectory.instance == null)
            Instantiate(trajectory);
        if (HudCanvas.instance == null)
            Instantiate(hudCanvas);
        if (PauseCanvas.instance == null)
            Instantiate(pauseCanvas);
        if (GameOverCanvas.instance == null)
            Instantiate(gameOverCanvas);
        if (WinCanvas.instance == null)
            Instantiate(winCanvas);
    }
}
