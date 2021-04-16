using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek : MonoBehaviour{

    private Vector2 desiredVelocity;
    private Vector2 steering;

    public Vector2 Movement(Vector2 position, Vector2 targetPosition, Vector2 bodyVelocity, int speed){
        desiredVelocity = targetPosition - position;
        desiredVelocity = desiredVelocity.normalized * speed;

        steering = desiredVelocity - bodyVelocity;
        steering = Vector2.ClampMagnitude(steering, speed);
        steering /= 15f;

        return steering;
    }
}
