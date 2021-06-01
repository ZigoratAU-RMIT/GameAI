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
        seek
    }

    private Rigidbody2D body;
    private Vector2 steering;

    public Wander wander;
    public Seek seek;

    public int state = (int)States.wander;

    [Range(0f, 10f)]
    public float unseenDst = 3f;

    
    [Range(0,10)]
    public int speed = 3;

    [Range(0,20)]
    public int avoidanceSpeed = 15;

    [Range(0f, 10f)]
    public float rayDst = 2f;

    float dstToTarget;
    public int viewAngle = 180;

    private GameObject target;
    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public Tilemap obstacleMap;
    public Grid grid;

    public List<GameObject> visibleTargets = new List<GameObject>();

    void Start(){
        body = GetComponent<Rigidbody2D>();
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
                    //if (targetsInViewRadius[i].gameObject.GetComponent<DwarfAgent>() != null){
                    //    continue;
                    //}
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
                if(target == null || Vector2.Distance(transform.position, target.transform.position) > 3f){
                    state = (int)States.wander;
                    return;
                }

                //Collision avoidance
                Vector2 avoidance = Vector2.zero;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, rayDst, obstacleMask);
                Debug.DrawRay(transform.position, transform.up, Color.green);
                if(hit.collider != null){
                    Debug.Log("Hit");
                    Vector3Int coordinate = grid.WorldToCell(hit.point);
 
                    Vector2 hitCentre = new Vector2(obstacleMap.GetCellCenterWorld(coordinate).x, obstacleMap.GetCellCenterWorld(coordinate).y);
                    Debug.DrawLine(transform.position, hitCentre, Color.blue);
                    avoidance = (body.velocity.normalized + new Vector2(transform.position.x, transform.position.y) * 2.0f) - hitCentre;
                    avoidance = avoidance.normalized * avoidanceSpeed;
                    avoidance = Vector2.ClampMagnitude(avoidance, avoidanceSpeed);
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
