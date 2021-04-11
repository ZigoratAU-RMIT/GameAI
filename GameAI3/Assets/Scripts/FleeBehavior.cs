using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeBehavior : MonoBehaviour
{
    public Vector2 calculateMove(Vector2 agentPos, Vector2 target, Rigidbody2D body, float maxSpeed){
        Vector2 desiredVelocity = -(target - agentPos);
        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        Vector2 steering = desiredVelocity - body.velocity;
        steering = Vector2.ClampMagnitude(steering, maxSpeed);
         
        body.velocity = Vector2.ClampMagnitude(body.velocity + steering, maxSpeed);
        return body.velocity.normalized;
    }

}