using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : MonoBehaviour
{
    public Vector2 calculateMove(Vector2 agentPos, Vector2 playerPos, Rigidbody2D body, float maxSpeed){
        Vector2 desiredVelocity = playerPos - agentPos;
        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        Vector2 steering = desiredVelocity - body.velocity;
        steering = Vector2.ClampMagnitude(steering, maxSpeed);
        steering /= 15f;

        return steering;
    }

}