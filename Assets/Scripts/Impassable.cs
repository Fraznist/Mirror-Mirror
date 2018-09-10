using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impassable : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameManager.instance.impassableList.Add(GetComponent<BoxCollider2D>());
	}
}
