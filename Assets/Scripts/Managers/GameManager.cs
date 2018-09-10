using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public enum GameStage {
        idle, busy, execMoves, unitsMoving, gamePaused
    }

    public static GameManager instance;

    private List<MovingObject> movers;
    private List<Beholder> beholders;

    public GameStage stage;
    public List<Collider2D> impassableList;
    public List<Collider2D> groundList;

    public bool isDragMode;    // MirrorLine drawing mode stored in gameManager

    // Use this for initialization
    void Awake () {

        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        movers = new List<MovingObject>();
        beholders = new List<Beholder>();

        GameManager.instance.stage = GameManager.GameStage.idle;

        isDragMode = true;
    }
	
	// Update is called once per frame
	void Update () {
        if (stage.Equals(GameStage.execMoves)) {
            moveEnemies();
        }
    }

    public void LineDrawToggle() {
        isDragMode = !isDragMode;
    }

    public void winGame() {
        GameManager.instance.stage = GameStage.gamePaused;
        Canvas winGUI = WinCanvas.instance.gameObject.GetComponent<Canvas>();
        winGUI.enabled = true;
    }

    public void nextLevel() {
        Canvas winGUI = WinCanvas.instance.gameObject.GetComponent<Canvas>();
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        winGUI.enabled = false;
        sceneIndex = sceneIndex % (SceneManager.sceneCountInBuildSettings - 1) + 1;   // main scene not counted
        SceneManager.LoadScene(sceneIndex);
    }

    public void openMainMenu() {
        Time.timeScale = 1;
        Destroy(GameObject.Find("HudCanvas(Clone)"));
        Destroy(GameObject.Find("PauseCanvas(Clone)"));
        Destroy(GameObject.Find("MirrorLine(Clone)"));
        Destroy(GameObject.Find("Trajectory(Clone)"));
        Destroy(GameObject.Find("GameOverCanvas(Clone)"));
        Destroy(GameObject.Find("WinCanvas(Clone)"));
        SceneManager.LoadScene(0);
    }

    public void goToLevel(int level) {
        SceneManager.LoadScene(level);
    }

    public void RestartLevel() {
        Time.timeScale = 1;
        GameOverCanvas.instance.gameObject.GetComponent<Canvas>().enabled = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void AddMover(MovingObject mover) {
        movers.Add(mover);
    }

    public void AddBeholder(Beholder beh) {
        beholders.Add(beh);
    }

    private void moveEnemies() {
        stage = GameStage.unitsMoving;
        for (int i = 0; i < movers.Count; i++) {
            movers[i].Move();
        }
    }

    public bool setBeholderSights(Vector2 lookAt) {
        bool seen = false;
        foreach (Beholder b in beholders) {
            if (b.changeSight(lookAt)) {
                seen = true;
                break;
            }
        }
        return seen;
    }

    public void gamePause() {        
        if (GameManager.instance.stage == GameManager.GameStage.idle) {
            Canvas pauseGUI = PauseCanvas.instance.gameObject.GetComponent<Canvas>();
            Time.timeScale = 0;
            GameManager.instance.stage = GameManager.GameStage.gamePaused;
            pauseGUI.enabled = true;

            bool temp = instance.isDragMode;
            GameObject.Find("PauseDrawToggle").GetComponent<Toggle>().isOn = temp;
            instance.isDragMode = temp;
        }
        
    }

    public void gameResume() {
        if (GameManager.instance.stage == GameManager.GameStage.gamePaused) {
            Canvas pauseGUI = PauseCanvas.instance.gameObject.GetComponent<Canvas>();
            Time.timeScale = 1;
            GameManager.instance.stage = GameManager.GameStage.idle;
            pauseGUI.enabled = false;
        }
    }

    public void gameOver() {
        GameManager.instance.stage = GameStage.gamePaused;
        Time.timeScale = 0;
        GameObject.Find("PauseCanvas(Clone)").GetComponent<Canvas>().enabled = false;
        GameObject.Find("GameOverCanvas(Clone)").GetComponent<Canvas>().enabled = true;
    }

    // Scene changing stuff
    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        movers.Clear();
        beholders.Clear();
        impassableList.Clear();
        groundList.Clear();
        GameManager.instance.stage = GameStage.idle;

        if (scene.buildIndex != 0) {
            MirrorLine.instance.setDrawBehavior(isDragMode);
            GameObject button = GameObject.Find("MoveAcceptButton");
            button.GetComponent<Image>().enabled = !GameManager.instance.isDragMode;
            button.GetComponent<Button>().enabled = !GameManager.instance.isDragMode;
        }
    }
}
