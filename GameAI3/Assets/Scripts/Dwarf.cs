using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dwarf : MonoBehaviour{
    //Define states
    private enum States{
        wander,
        flee,
        follow,
    }

    int state = (int)States.wander;
    private int speed = 5;

    private Rigidbody2D body;
    private Vector2 steering;

    //Wander
    private Vector2 circleCentre;
    private Vector2 displacement;
    private Vector2 wanderForce;

    // Start is called before the first frame update
    void Start(){
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate(){
        switch (state){
            case (int)States.wander:
                //Movement
                circleCentre = body.velocity;
                circleCentre = circleCentre.normalized * 2;

                displacement = new Vector2(0, -1);
                displacement = displacement.normalized * 2;

                int heading = Random.Range(0, 360);

                displacement.x = Mathf.Cos(heading);
                displacement.y = Mathf.Sin(heading);

                wanderForce = circleCentre + displacement;

                steering = wanderForce - body.velocity;
                steering = Vector2.ClampMagnitude(steering, speed);
                steering /= 15f;

                break;
            case (int)States.flee:
                break;
            case (int)States.follow:
                break;
            default:
                state = (int)States.wander;
                break;
        }
        body.velocity = Vector2.ClampMagnitude(body.velocity + steering, speed);
        transform.up = body.velocity.normalized;
    }
}
