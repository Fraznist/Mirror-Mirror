using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beholder : MonoBehaviour {
    // The beholder is an immobile enemy, that will look at the direction of the player. If it can see the player directly
    // (ingame mirrors dont work for beholder), the player dies.
    private GameObject player;
    private LineRenderer line;

	// Use this for initialization
	void Start () {
        GameManager.instance.AddBeholder(this);
        line = GetComponent<LineRenderer>();
        player = GameObject.FindWithTag("Player");

        line.SetPosition(0, transform.position);
        changeSight(player.transform.position);
	}
	
	public bool changeSight(Vector2 lookAt) {
        // called during the players teleport coroutine
        bool playerVisible = true;
        // cast a ray between lookAt and beholder
        RaycastHit2D ray = Physics2D.Linecast(transform.position, lookAt, LayerMask.GetMask("Impassable")); 
        // if the ray doesnt hit anything, then the beholder can see the player.
        if (ray.collider != null) {
            lookAt = ray.point;
            playerVisible = false;
        }
        // set endpoint to the first impassable collider, ot the player if there is no collider inbetween.
        line.SetPosition(1, lookAt);
        return playerVisible;
    }
}
