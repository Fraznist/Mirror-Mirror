using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MovingObject {
    public Vector2[] PatrolMovements;
    public int startIndex;
    public int zSort;

    // Use this for initialization
    protected override void Start () {
        GameManager.instance.groundList.Add(GetComponent<BoxCollider2D>());

        base.Start();

        transform.position = new Vector3(transform.position.x, transform.position.y, zSort);
    }

    public override void Move() {
        StartCoroutine(SmoothMovement(PatrolMovements[startIndex]));
        startIndex = (startIndex + 1) % PatrolMovements.Length;
    }

    protected override void setSortingLayer() {
        return;
    }

}
