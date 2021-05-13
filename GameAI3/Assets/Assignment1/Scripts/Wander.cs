using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : MonoBehaviour {
    private Vector2 steering;

    //Wander
    private Vector2 circleCentre;
    private Vector2 displacement;
    private Vector2 wanderForce;

    public Vector2 Movement(Vector2 bodyVelocity, int speed){
        circleCentre = bodyVelocity;
        circleCentre = circleCentre.normalized * 2;

        displacement = new Vector2(0, -1);
        displacement = displacement.normalized * 2;

        int heading = Random.Range(0, 360);

        displacement.x = Mathf.Cos(heading);
        displacement.y = Mathf.Sin(heading);

        wanderForce = circleCentre + displacement;

        steering = wanderForce - bodyVelocity;
        steering = Vector2.ClampMagnitude(steering, speed);
        steering /= 15f;

        Debug.DrawRay(transform.position, displacement, Color.green);
        Debug.DrawRay(transform.position, circleCentre, Color.magenta);

        return steering;
    }
}
