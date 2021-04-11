using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeBehavior : MonoBehaviour
{
    public Vector2 calculateMove(Vector2 agentPos, Vector2 playerPos, Rigidbody2D body, float maxSpeed){
        Vector2 desiredVelocity = -(playerPos - agentPos);
        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        Vector2 steering = desiredVelocity - body.velocity;
        return steering;
        // steering /= maxSpeed;
         
        // body.velocity = Vector2.ClampMagnitude(body.velocity + steering, maxSpeed);
        // Debug.Log(body.velocity);
    }

}