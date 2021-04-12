using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : MonoBehaviour{
    //Define states
    private enum States{
        wander,
        seek,
        arrive,
        flee
    }
    
    int state = (int)States.wander;
    private int speed = 5;

    private Rigidbody2D body;

    private Vector2 circleCentre;
    private Vector2 displacement;
    private Vector2 wanderForce;
    private Vector2 steering;

    public LayerMask targetMask;

    public List<Transform> visibleTargets = new List<Transform>();

    public int viewAngle = 180;

    void Start(){
        body = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate(){
        switch(state){
            case (int)States.wander:
                //Movement
                circleCentre = body.velocity;
                circleCentre = circleCentre.normalized * 2;

                displacement = new Vector2(0, -1);
                displacement = displacement.normalized * 2;

                int heading = Random.Range(0, 360);

                displacement.x = Mathf.Cos(heading);
                displacement.y = Mathf.Sin(heading);

                wanderForce = circleCentre + displacement;

                steering = wanderForce - body.velocity;
                steering = Vector2.ClampMagnitude(steering, speed);
                steering /= 15f;

                body.velocity = Vector2.ClampMagnitude(body.velocity + steering, speed);
                transform.up = body.velocity.normalized;

                //Finding target
                visibleTargets.Clear();
                Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, 10, targetMask);

                for(int i = 0; i < targetsInViewRadius.Length; i++){
                    Transform target = targetsInViewRadius[i].transform;
                    Vector2 dirToTarget = (target.position - transform.position).normalized;
                    if(Vector2.Angle(transform.up, dirToTarget) < viewAngle / 2){
                        float dstToTarget = Vector2.Distance(transform.position, target.position);
                        //If line draw form object to target is not interrupted by wall, add target to list of visible targets
                        //if(!Physics2D.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                            visibleTargets.Add(target);
                            
                    }
                }

                Debug.DrawRay(transform.position, displacement, Color.green);
                Debug.DrawRay(transform.position, circleCentre, Color.magenta);
                break;
            case (int)States.seek:
                break;
            case (int)States.arrive:
                break;
            case (int)States.flee:
                break;
            default:
                state = (int)States.wander;
                break;
            }
    }
}
