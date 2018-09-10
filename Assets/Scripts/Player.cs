using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class Player : MonoBehaviour {
    private float animTime = 1f;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private SpriteRenderer sprite;
    private Animator animator;
    private LineRenderer direction;
    private Text moveCount;
    private int moves;

	// Use this for initialization
	void Start () {
        moves = 0;
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        direction = GetComponent<LineRenderer>();
        direction.sortingLayerName = "TopLayer";
        transform.position = new Vector3(transform.position.x, transform.position.y, -3);
        setSortingLayer();
        moveCount = GameObject.Find("MoveText").GetComponent<Text>();
        moveCount.text = "Moves: " + moves;
    }

    private IEnumerator Teleport(Vector2 coord) {
        GameManager.instance.stage = GameManager.GameStage.busy;
        boxCollider.enabled = false;
        moves++;
        moveCount.text = "Moves: " + moves;
        yield return new WaitForSeconds(animTime * 0.5f);
        GameManager.instance.stage = GameManager.GameStage.execMoves;   // Player made a valid move, switch game stage to move every moveable object.
        rb2D.MovePosition(coord);
        yield return null;
        bool playerVisible = GameManager.instance.setBeholderSights(coord);
        setSortingLayer();
        yield return new WaitForSeconds(animTime * 0.5f);
        boxCollider.enabled = true;
        if (playerVisible || !groundEnMasse(rb2D.position)) 
            Death();
        GameManager.instance.stage = GameManager.GameStage.idle;
    }

    public void finishedMirror() {
        Vector2 toTeleport;
        
        if (testTeleport(out toTeleport))
            StartCoroutine(Teleport(toTeleport));

        Trajectory.instance.resetTrajectory();

        direction.SetPosition(0, Vector2.zero);
        direction.SetPosition(1, Vector2.zero);
    }

    // Test if current trajectory is valid for a teleport, and calculate where to teleport
    public bool testTeleport(out Vector2 teleportLoc) {
        Vector2 mirrorImpact;
        Vector2 currentPos = rb2D.position;
        teleportLoc = Vector2.zero;

        Trajectory.instance.resetTrajectory();

        if (!testMirrorAndPlayer(out mirrorImpact))     // trajectory doesnt go through mirror, no teleport
            return false;

        teleportLoc = 2 * mirrorImpact - currentPos; // calculate new position of player, should the teleport occur

        direction.SetPosition(0, rb2D.position);
        direction.SetPosition(1, Vector2.MoveTowards(rb2D.position, teleportLoc, 1f));

        Trajectory.instance.addEdge(currentPos);

        if (!recursiveTestTrajectory(currentPos, teleportLoc, out teleportLoc, null))
            return false;

        if (!collidingEnMasse(teleportLoc))   // validDestination is true if the player will not collide with any impassables after teleportation.
            return true;
        else {
            Trajectory.instance.setValidity(false);
            return false;
        }
    }

    // Cast a ray between current position and "would be" destination in the case of no collisions. Test collisions with impassables
    // Abort teleportation if ray collides with a wall. 
    // Send a ray from collision point to newly calculated "would be" destination, upon collision with a mirror
    public bool recursiveTestTrajectory(Vector2 rayStart, Vector2 rayEnd, out Vector2 teleportLoc, Collider2D lastCollider) {
        // the start location of the ray is on the mirrors boxCollider, we need to disable the mirrors collider temporarily.
        if (lastCollider != null) lastCollider.enabled = false; 
        RaycastHit2D hit = Physics2D.Linecast(rayStart, rayEnd, LayerMask.GetMask("Impassable"));
        if (lastCollider != null) lastCollider.enabled = true;

        teleportLoc = rayEnd;

        if (hit.collider == null) {
            Trajectory.instance.addEdge(teleportLoc);    // add last edge of trail line to the point of teleportation
            return true;
        }
        else {
            if (hit.collider.CompareTag("Wall")) { // Hit wall, invalid teleport trajectory
                Trajectory.instance.addEdge(hit.point);  // End the trajectory at wall collision point
                Trajectory.instance.setValidity(false);
                return false;
            }
            else {      // Hit mirror, find reflected teleport location, cast ray from mirror hit location.
                teleportLoc = newTrajectoryDestination(hit.collider.bounds, hit.point, teleportLoc);
                Trajectory.instance.addEdge(hit.point);
                return recursiveTestTrajectory(hit.point, teleportLoc, out teleportLoc, hit.collider);
            }
        }
    }

    // Cast a ray between current player position and the reflection of the position according to drawn mirror
    // Does the ray actually pass through the mirror, return false if no.
    public bool testMirrorAndPlayer(out Vector2 hitPoint) {
        EdgeCollider2D edge = MirrorLine.instance.getEdge();

        Vector2 mirrorVector = edge.points[1] - edge.points[0];
        Vector2[] rayDirection = new Vector2[2];

        rayDirection[0] = new Vector2(-mirrorVector.y, mirrorVector.x);
        rayDirection[1] = new Vector2(mirrorVector.y, -mirrorVector.x);

        for (int i = 0; i < 2; i++) {
            RaycastHit2D hit = Physics2D.Raycast(rb2D.position, rayDirection[i], Mathf.Infinity, LayerMask.GetMask("Mirror"));
            if (hit.collider != null) {
                hitPoint = hit.point;
                return true;
            }
        }
        hitPoint = Vector2.zero;
        return false;
    }

    private Vector2 newTrajectoryDestination(Bounds mirrorBox, Vector2 colPoint, Vector2 oldDest) {

        if (Mathf.Abs(colPoint.x - mirrorBox.max.x) < 0.0005f || Mathf.Abs(colPoint.x - mirrorBox.min.x) < 0.0005f) {
            // Ray collided the mirror from left or right, take reflection of destination by vertical axis x = colpoint.x
            return new Vector2(2 * colPoint.x - oldDest.x, oldDest.y);
        }
        else {
            // Ray collided the mirror from above or below, take reflection of destination by horizontal axis y = colpoint.y
            return new Vector2(oldDest.x, 2 * colPoint.y - oldDest.y);
        }
    }

    private bool isCollidingPleb(Vector2 center, Bounds bound) {
        // returns true if a collider with the size of the players boxcollider centered at arg center, collides with the bound
        double ex = boxCollider.size.x * 0.5, ey = boxCollider.size.y * 0.5;
        if (center.x < bound.max.x + ex && center.x > bound.min.x - ex && center.y < bound.max.y + ey && center.y > bound.min.y - ey)
            return true;

        else return false;
    }

    private bool collidingEnMasse(Vector2 center) {
        List<Collider2D> impassableArray = GameManager.instance.impassableList;
        for (int i = 0; i < impassableArray.Count; i++) {
            if (isCollidingPleb(center, impassableArray[i].bounds)) return true;
        }
        return false;
    }

    private bool groundEnMasse(Vector2 center) {
        List<Collider2D> groundArray = GameManager.instance.groundList;
        for (int i = 0; i < groundArray.Count; i++) {
            if (isCollidingPleb(center, groundArray[i].bounds)) return true;
        }
        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        
        if (collision.CompareTag("Finish")) {
            Invoke("NextLevel", 1f);
        }
        else if (collision.CompareTag("Enemy")) {
            Death();
        }
    }

    private void setSortingLayer() {
        sprite.sortingOrder = Mathf.RoundToInt(rb2D.position.y * 100f) * -1;
    }

    private void NextLevel() {
        GameManager.instance.winGame();
    }

    private void Restart() {        
        GameManager.instance.gameOver();
    }

    private void Death() {
        Debug.Log("I DA!");
        animator.SetTrigger("death");
        Invoke("Restart", 1f);
    }
}
