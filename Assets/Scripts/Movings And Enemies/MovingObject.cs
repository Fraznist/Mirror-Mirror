using UnityEngine;
using System.Collections.Generic;
using System.Collections;

//The abstract keyword enables you to create classes and class members that are incomplete and must be implemented in a derived class.
//This class 
public abstract class MovingObject : MonoBehaviour {
    public float moveTime = 0.1f;           //Time it will take object to move, in seconds.

    protected Rigidbody2D rb2D;             //The Rigidbody2D component attached to this object.
    protected SpriteRenderer sprite;        //The sprite renderer to modify sorting layer during runtime
    protected float inverseMoveTime;        //Used to make movement more efficient.


    //Protected, virtual functions can be overridden by inheriting classes.
    protected virtual void Start() {
        //Get a component reference to this object's Rigidbody2D
        GameManager.instance.AddMover(this);

        rb2D = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        transform.position = new Vector3(transform.position.x, transform.position.y, -3);
        setSortingLayer();

        //By storing the reciprocal of the move time we can use it by multiplying instead of dividing, this is more efficient.
        inverseMoveTime = 1f / moveTime;
    }

    public abstract void Move();

    //Co-routine for moving units from one space to next, takes a parameter move to specify how much in which direction to move to.
    protected IEnumerator SmoothMovement(Vector2 move) {
        //Calculate the remaining distance to move based on the square magnitude of the difference between current position and end parameter. 
        //Square magnitude is used instead of magnitude because it's computationally cheaper.

        float sqrRemainingDistance = move.sqrMagnitude;
        Vector2 end = rb2D.position + move;

        //While that distance is greater than a very small amount (Epsilon, almost zero):
        while (sqrRemainingDistance > float.Epsilon) {
            //Find a new position proportionally closer to the end, based on the moveTime
            Vector2 newPostion = Vector2.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);

            //Call MovePosition on attached Rigidbody2D and move it to the calculated position.
            rb2D.MovePosition(newPostion);
            setSortingLayer();

            //Recalculate the remaining distance after moving.
            sqrRemainingDistance = (rb2D.position - end).sqrMagnitude;

            //Return and loop until sqrRemainingDistance is close enough to zero to end the function
            yield return null;
        }
    }
    
    protected virtual void setSortingLayer() {
        sprite.sortingOrder = Mathf.RoundToInt(rb2D.position.y * 100f) * -1;
    }
}

