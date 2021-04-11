using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour{
    public Rigidbody2D player;
    private Rigidbody2D body;
    Vector2 steering;
    Vector2 desiredVelocity;
    public int LEADER_BEHIND_DIST = 1;
    // Start is called before the first frame update
    void Start(){
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate(){
        if(Vector2.Distance(transform.position, player.position) < 2f){
            body.velocity = Vector2.zero;
            return;
        }

        Vector2 targetVelocity = player.velocity * -1;
        targetVelocity = targetVelocity.normalized * LEADER_BEHIND_DIST;
        Vector2 behind = player.position + targetVelocity;

        desiredVelocity = behind - (Vector2)transform.position;
        desiredVelocity = desiredVelocity.normalized * 5;

        steering = desiredVelocity - body.velocity;
        steering = Vector2.ClampMagnitude(steering, 5);
        steering /= 15f;
         
        body.velocity = Vector2.ClampMagnitude(body.velocity + steering, 5);
        transform.up = body.velocity.normalized;
        Debug.DrawRay(transform.position, body.velocity.normalized * 2, Color.green);
        Debug.DrawLine(body.position, behind, Color.blue);
    }
}
