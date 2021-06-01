using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DwarfController : MonoBehaviour
{

    private int speed = 5;
    private Rigidbody2D body;
    private Vector2 steering;
    public Seek seek;

    void Start(){
        body = GetComponent<Rigidbody2D>();
    }

    void Update(){

        
        Vector2 targetPos = transform.position;
        if (Input.GetKey(KeyCode.A))
            targetPos -= Vector2.left;
        if (Input.GetKey(KeyCode.D))
            targetPos -= Vector2.right;
        if (Input.GetKey(KeyCode.W))
            targetPos -= Vector2.up;
        if (Input.GetKey(KeyCode.S))
            targetPos -= Vector2.down;

        steering = seek.Movement(targetPos, transform.position, body.velocity, speed);

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
