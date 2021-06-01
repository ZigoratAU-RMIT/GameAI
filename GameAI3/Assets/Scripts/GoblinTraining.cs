using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GoblinTraining : MonoBehaviour
{
    //Define states
    private enum States
    {
        wander,
        seek,
        chase,
        flee
    }

    private Rigidbody2D body;
    private Renderer rend;
    private Vector2 steering;

    public Wander wander;
    public Seek seek;

    public int state = (int)States.wander;
    private int speed = 5;
    float dstToTarget;

    public int viewAngle = 180;

    //Chase
    private Vector2 targetPosition;

    private GameObject target;
    private int index = 0;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public Tilemap obstacleMap;
    public Grid grid;

    public List<GameObject> visibleTargets = new List<GameObject>();

    void Start(){
        body = GetComponent<Rigidbody2D>();
        rend = GetComponent<Renderer>();
    }

    void FixedUpdate(){
        switch (state){
            case (int)States.wander:
                //Finding target
                visibleTargets.Clear();
                Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, 10, targetMask);

                for (int i = 0; i < targetsInViewRadius.Length; i++)
                {
                    GameObject target_ = targetsInViewRadius[i].gameObject;
                    if (targetsInViewRadius[i].gameObject.GetComponent<Dwarf>() != null){
                        if (targetsInViewRadius[i].gameObject.GetComponent<Dwarf>().state == 2){ // if dwarf is following player, then ignore
                            continue;
                        }
                    }
                    Vector2 dirToTarget = (target_.transform.position - transform.position).normalized;
                    if (Vector2.Angle(transform.up, dirToTarget) < viewAngle / 2)
                    {
                        dstToTarget = Vector2.Distance(transform.position, target_.transform.position);
                        //If line draw form object to target is not interrupted by wall, add target to list of visible targets
                        //if(!Physics2D.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                        visibleTargets.Add(target_);
                        target = target_;
                        
                        visibleTargets.Clear();
                        state = (int)States.seek;
                    }
                }

                //Movement
                steering = wander.Movement(body.velocity, speed);

                break;
            case (int)States.seek:
                if(target == null || Vector2.Distance(transform.position, target.transform.position) > 5f){
                    state = (int)States.wander;
                    return;
                }

                //Collision avoidance
                Vector2 avoidance = Vector2.zero;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 2f, obstacleMask);
                Debug.DrawRay(transform.position, transform.up, Color.green);
                if(hit.collider != null){
                    Debug.Log("Hit");
                    Vector3Int coordinate = grid.WorldToCell(hit.point);
 
                    Vector2 hitCentre = new Vector2(obstacleMap.GetCellCenterWorld(coordinate).x, obstacleMap.GetCellCenterWorld(coordinate).y);
                    Debug.DrawLine(transform.position, hitCentre, Color.blue);
                    avoidance = (body.velocity.normalized + new Vector2(transform.position.x, transform.position.y) * 2.0f) - hitCentre;
                    avoidance = avoidance.normalized * speed;
                    avoidance = Vector2.ClampMagnitude(avoidance, speed);
                    avoidance /= 15f;
                }

                //Movement
                steering = seek.Movement(transform.position, target.transform.position, body.velocity, speed);
                steering = steering + avoidance;
                Debug.DrawRay(transform.position, steering, Color.magenta);
                break;
        }

        body.velocity = Vector2.ClampMagnitude(body.velocity + steering, speed);
        transform.up = body.velocity.normalized;
    }
}
