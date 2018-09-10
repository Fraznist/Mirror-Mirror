using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartEnemy : MovingObject {
    public int moveStride;

    public override void Move() {
        GameObject player = GameObject.Find("Player");
        Vector2 target = player.GetComponent<Rigidbody2D>().position;   // where a smartenemy is trying to move to

        //float xDiff = target.x - rb2D.position.x;
        //float yDiff = target.y - rb2D.position.y;

        //bool xSuccess = moveInX(xDiff);
        //bool ySuccess = moveInY(yDiff);

        //if (xSuccess && ySuccess) {
        //    if (Mathf.Abs(xDiff) > Mathf.Abs(yDiff)) {
        //        if 
        //    }
        //}

        Vector2 direction = target - rb2D.position;

        RaycastHit2D ray = Physics2D.Raycast(rb2D.position, direction, moveStride, LayerMask.GetMask("Impassable"));
        if (ray.collider == null) {
            StartCoroutine(SmoothMovement(Vector2.ClampMagnitude(direction, moveStride)));
        }
    }

    public bool moveInX(float xDiff) {
        Vector2 move;
        if (xDiff > 0) move = Vector2.right;
        else move = Vector2.left;

        RaycastHit2D ray = Physics2D.Raycast(rb2D.position, move, 1, LayerMask.GetMask("Impassable"));
        if (ray.collider == null) 
            return true;
        else return false;
    }

    public bool moveInY(float yDiff) {
        Vector2 move;
        if (yDiff > 0) move = Vector2.right;
        else move = Vector2.left;

        RaycastHit2D ray = Physics2D.Raycast(rb2D.position, move, 1, LayerMask.GetMask("Impassable"));
        if (ray.collider == null) 
            return true;
        else return false;
    }
}
