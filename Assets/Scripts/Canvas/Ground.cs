using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameManager.instance.groundList.Add(GetComponent<BoxCollider2D>());
    }
}
