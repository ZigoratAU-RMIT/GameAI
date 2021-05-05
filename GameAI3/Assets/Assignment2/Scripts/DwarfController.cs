using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DwarfController : MonoBehaviour
{

    private int speed = 5;
    private Rigidbody2D body;
    private Vector2 steering;
    private GameObject target;
    public Transform goal;
    public Seek seek;

    void Start(){
        body = GetComponent<Rigidbody2D>();
    }

    void Update(){
        steering = seek.Movement(goal.position, transform.position, body.velocity, speed);

        body.velocity = Vector2.ClampMagnitude(body.velocity + steering, speed);
        transform.up = body.velocity.normalized;
    }

    private void OnTriggerEnter2D(Collider2D col){
        if(col.tag == "Goal"){
            Destroy(this.gameObject);
        }

        if(col.tag == "River"){
            speed = 2;
        }
    }

    private void OnTriggerExit2D(Collider2D col){
        if(col.tag == "River"){
            speed = 5;
        }
    }
}
