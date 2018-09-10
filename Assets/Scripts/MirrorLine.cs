using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;

public class MirrorLine : MonoBehaviour {
    // singleton class, only one mirror is needed.
    public static MirrorLine instance;
    // 2 different materials to indicate valid or invalid mirror
    public Material validMirror;
    public Material invalidMirror;

    private LineRenderer line;      //reference to LineRenderer component
    private Player playerScript;      // reference to the script component of the current player object
    private EdgeCollider2D edge;    // Store a 2D EdgeCollider for the mirror, for detection if the teleport trajectory passes through mirror

    private LineDraw drawBehavior;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        line = GetComponent<LineRenderer>();
        edge = GetComponent<EdgeCollider2D>();

        line.sortingLayerName = "TopLayer";
        reloadPlayer();

        drawBehavior = new LineDrag();
    }

    public void setDrawBehavior(bool setToDrag) {
        if (setToDrag)
            drawBehavior = new LineDrag();
        else
            drawBehavior = new LineAccept();
    }

    private void Update() {
#if UNITY_IOS
        if (Input.touchCount > 1) return;
#endif

        Vector2 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
            drawBehavior.MouseButtonDown(newPos);
        else if (Input.GetMouseButton(0) && drawBehavior.getEndPoint() != newPos)
            drawBehavior.MouseButtonPressed(newPos);
        else if (Input.GetMouseButtonUp(0))
            drawBehavior.MouseButtonUp(newPos);
    }

    public void pointsModified(Vector2 start, Vector2 end) {
        line.SetPosition(0, start);
        line.SetPosition(1, end);

        setEdgeCollider();
        setMirrorMaterial();
    }

    public void pointsDecided(Vector2 start, Vector2 end) {
        line.SetPosition(0, start);
        line.SetPosition(1, end);

        setEdgeCollider();
        playerScript.finishedMirror();

        line.SetPosition(0, Vector3.zero);
        line.SetPosition(1, Vector3.zero);
    }

    private void setEdgeCollider() {
        Vector2[] edgeEdges = new Vector2[2];

        edgeEdges[0] = (Vector2)line.GetPosition(0);
        edgeEdges[1] = (Vector2)line.GetPosition(1);

        edge.points = edgeEdges;
    }

    private void setMirrorMaterial() {
        Vector2 dummy = Vector2.zero;
        if (!playerScript.testTeleport(out dummy))
            line.sharedMaterial = invalidMirror;
        else
            line.sharedMaterial = validMirror;
    }

    public EdgeCollider2D getEdge() {
        return edge;
    }

    public LineRenderer getLine() {
        return line;
    }

    public void reloadPlayer() {
        GameObject playah = GameObject.FindWithTag("Player");
        playerScript = playah.GetComponent<Player>();
    }

    // Scene changing stuff
    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        reloadPlayer();
    }

    public void acceptMove() {
        drawBehavior.PointsDecided();
    }

    private class LineDrag : LineDraw {
        // The player simply drags a line on the screen, move is submitted when the player releases finger

        public override void PointsDecided() {
            MirrorLine.instance.pointsDecided(startPoint, endPoint);
        }

        public override void PointsModified() {
            MirrorLine.instance.pointsModified(startPoint, endPoint);
        }

        public override void MouseButtonDown(Vector2 newPos) {
            startPoint = newPos;
            endPoint = newPos;
            PointsModified();
        }

        public override void MouseButtonPressed(Vector2 newPos) {
            endPoint = newPos;
            PointsModified();
        }

        public override void MouseButtonUp(Vector2 newPos) {
            endPoint = newPos;
            PointsDecided();
        }
    }

    private class LineAccept : LineDraw {
        // The player draws a line, and can drag the start and end points of the line to maniplulate the line
        // The line doesn't disappear when the player releases his finger. The player submits the move via a HUD button

        private enum Selected { NONE, START, END }
        private Selected selected;

        private bool mirrorOnScene = false;

        public override void MouseButtonDown(Vector2 newPos) {
            if (!mirrorOnScene) {
                startPoint = newPos;
                endPoint = newPos;
                PointsModified();
                selected = Selected.END;
                mirrorOnScene = true;
            }
            else {
                setSelectedEdge(newPos);
            }
        }

        public override void MouseButtonPressed(Vector2 newPos) {
            if (selected == Selected.START) {
                startPoint = newPos;
                PointsModified();
            }
            else if (selected == Selected.END) {
                endPoint = newPos;
                PointsModified();
            }
        }

        public override void MouseButtonUp(Vector2 newPos) {
            if (selected == Selected.START) {
                startPoint = newPos;
                PointsModified();
            }
            else if (selected == Selected.END) {
                endPoint = newPos;  
                PointsModified();
            }

            if ((startPoint - endPoint).magnitude < 0.5) {
                mirrorOnScene = false;
            }

            selected = Selected.NONE;
        }

        public override void PointsDecided() {
            MirrorLine.instance.pointsDecided(startPoint, endPoint);
            mirrorOnScene = false;
        }

        public override void PointsModified() {
            MirrorLine.instance.pointsModified(startPoint, endPoint);
        }

        private void setSelectedEdge(Vector2 fingerTouch) {
            if (isCloseEnough(startPoint, fingerTouch))
                selected = Selected.START;
            else if (isCloseEnough(endPoint, fingerTouch))
                selected = Selected.END;
            else selected = Selected.NONE;
        }

        private bool isCloseEnough(Vector2 testedEdge, Vector2 fingerTouch) {
            if ((testedEdge - fingerTouch).magnitude < 0.5)
                return true;
            else return false;
        }
    }

    private abstract class LineDraw {
        // Base class for the drawing behaviors

        protected Vector2 startPoint;
        protected Vector2 endPoint;

        public virtual Vector2 getEndPoint() {
            return endPoint;
        }

        public abstract void PointsModified();

        public abstract void PointsDecided();

        public abstract void MouseButtonDown(Vector2 newPos);

        public abstract void MouseButtonPressed(Vector2 newPos);

        public abstract void MouseButtonUp(Vector2 newPos);
    }
}
