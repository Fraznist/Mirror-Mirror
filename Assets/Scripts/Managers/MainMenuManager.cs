using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {

    private Canvas mainCanvas;
    private Canvas levelSelectCanvas;
    private Canvas settingsCanvas;

    //public GameObject eventSystem;

    // Use this for initialization
    void Start () {
        mainCanvas = GameObject.Find("MainCanvas").GetComponent<Canvas>();
        levelSelectCanvas = GameObject.Find("LevelSelectCanvas").GetComponent<Canvas>();
        settingsCanvas = GameObject.Find("SettingsCanvas").GetComponent<Canvas>();

        //if (EventSystemSingleton.instance == null)
        //    Instantiate(eventSystem);
    }
	
	public void openLevelCanvas() {
        levelSelectCanvas.enabled = true;
        mainCanvas.enabled = false;
    }

    public void openSettingsCanvas() {
        settingsCanvas.enabled = true;
        mainCanvas.enabled = false;

        bool temp = GameManager.instance.isDragMode;
        GameObject.Find("LineDrawToggle").GetComponent<Toggle>().isOn = temp;
        GameManager.instance.isDragMode = temp;
    }

    public void openMainCanvas() {
        levelSelectCanvas.enabled = false;
        settingsCanvas.enabled = false;
        mainCanvas.enabled = true;
    }

    public void openLevel(int level) {
        GameManager.instance.goToLevel(level);
    }

    public void LineDrawToggle() {
        GameManager.instance.LineDrawToggle();
    }
}
