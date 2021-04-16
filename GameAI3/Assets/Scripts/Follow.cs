using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour{
    public Rigidbody2D leader;
    private Rigidbody2D body;
    Vector2 steering;
    Vector2 desiredVelocity;
    float slowingRadius = 5f;
    public int LEADER_BEHIND_DIST = 5;
    // Start is called before the first frame update
    void Start(){
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    public Vector2 Movement(){
        //if(Vector2.Distance(transform.position, leader.position) < 2f){
        //    steering = Vector2.zero;
        //    return steering;
        //}

        Vector2 targetVelocity = leader.velocity * -1;
        targetVelocity = targetVelocity.normalized * LEADER_BEHIND_DIST;
        Vector2 behind = leader.position + targetVelocity;
        desiredVelocity = behind - (Vector2)transform.position;
        float distance = desiredVelocity.magnitude;
 
        // Check the distance to detect whether the character
        // is inside the slowing area
        if (distance < slowingRadius) {
            // Inside the slowing area
            desiredVelocity = desiredVelocity.normalized * 5 * (distance / slowingRadius);
        } else {
            // Outside the slowing area.
            desiredVelocity = desiredVelocity.normalized * 5;
        }

        steering = desiredVelocity - body.velocity;
        steering = Vector2.ClampMagnitude(steering, 5);
        steering /= 15f;
         
        Debug.DrawRay(transform.position, body.velocity.normalized * 2, Color.green);
        Debug.DrawLine(body.position, behind, Color.blue);

        return steering;
    }
}
