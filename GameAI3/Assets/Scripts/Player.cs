using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour{
    public Pathfinding pf;
    List<WorldTile> movementPoints = new List<WorldTile>();
    List<WorldTile> path = new List<WorldTile>(); 
    
    Vector2 target;
    Vector2 desiredVelocity;
    Vector2 steering;

    private Rigidbody2D body;
    public float speed = 50;
    public float rotationSpeed;

    private int index = 0;
    private bool movementDone = true;

    void Start(){
        body = GetComponent<Rigidbody2D>();
    }

    void Update(){
        if(Input.GetButtonDown("Fire1")){
            //Make sure our movement is all reset
            movementPoints.Clear();
            index = 0;
            movementDone = false;

            //Finding the path - this trims the whole pathfinding algorithm down to nodes
            //These nodes are based on when the direction the entity must go changes
            path = pf.FindPath(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            for(int i = 0; i < path.Count; i++){          
                if(i + 1 <= path.Count - 1 && path[i].direction != path[i + 1].direction){
                    movementPoints.Add(path[i]);
                }
            }
            //Add in last position
            movementPoints.Add(path[path.Count - 1]);

            target = new Vector2(movementPoints[index].cellX, movementPoints[index].cellY);
        }
    }

    void FixedUpdate(){
        if(movementDone == true)
            return;

        desiredVelocity = target - (Vector2)transform.position;
        desiredVelocity = desiredVelocity.normalized * speed;

        steering = desiredVelocity - body.velocity;
        steering = Vector2.ClampMagnitude(steering, speed);
        steering /= 15f;
         
        body.velocity = Vector2.ClampMagnitude(body.velocity + steering, speed);
        transform.up = body.velocity.normalized;

        if(Vector3.Distance(transform.position, new Vector3(movementPoints[index].cellX, movementPoints[index].cellY)) < 0.1f){
            index++;
            if(index == movementPoints.Count){
                movementDone = true;
                body.velocity = Vector2.zero;
                index = 0;
                return;
            }
            target = new Vector2(movementPoints[index].cellX, movementPoints[index].cellY);
        }

        Debug.DrawRay(transform.position, body.velocity.normalized * 2, Color.green);
        Debug.DrawRay(transform.position, desiredVelocity.normalized * 2, Color.magenta);
        for(int i = 0; i < movementPoints.Count - 1; i++){
            Debug.DrawLine(new Vector2(movementPoints[i].cellX, movementPoints[i].cellY), new Vector2(movementPoints[i + 1].cellX, movementPoints[i + 1].cellY));
        }
        Debug.DrawLine(transform.position, target, Color.blue);
    }
}