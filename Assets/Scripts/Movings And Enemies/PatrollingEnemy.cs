using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrollingEnemy : MovingObject {
    public Vector2[] PatrolMovements;
    public int startIndex;
	
	public override void Move() {
        gameObject.GetComponent<MovingObject>();
        StartCoroutine(SmoothMovement (PatrolMovements[startIndex]));
        startIndex = (startIndex + 1) % PatrolMovements.Length;
    }
}
