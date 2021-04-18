using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : MonoBehaviour
{
    //Define states
    private enum States
    {
        wander,
        seek,
        chase,
        flee
    }

    private GameManager gm;
    public Wander wander;
    public Seek seek;
    public Flee flee;

    //Pathfinding
    public Pathfinding pf;
    List<WorldTile> movementPoints = new List<WorldTile>();
    List<WorldTile> path = new List<WorldTile>();

    public int state = (int)States.wander;
    private int speed = 5;

    private Rigidbody2D body;
    private Renderer rend;
    private Vector2 steering;

    // float distance;
    float dstToTarget;
    private int recheck = 45;

    //Chase
    private Vector2 targetPosition;

    private GameObject target;
    private int index = 0;

    public LayerMask targetMask;

    public List<GameObject> visibleTargets = new List<GameObject>();

    public int viewAngle = 180;

    void Start()
    {
        transform.position = pf.map.GetRandomPoint(false);
        body = GetComponent<Rigidbody2D>();
        rend = GetComponent<Renderer>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void FixedUpdate()
    {
        switch (state)
        {
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

                        movementPoints.Clear();
                        movementPoints = pathFind(transform.position, target.transform.position, pf);
                        targetPosition = new Vector2(movementPoints[index].cellX + 0.5f, movementPoints[index].cellY + 0.5f);
                        
                        visibleTargets.Clear();
                        state = (int)States.seek;
                    }
                }

                //Movement
                steering = wander.Movement(body.velocity, speed);

                break;
            case (int)States.seek:
                if(target == null){
                    state = (int)States.wander;
                    return;
                }

                if (Vector2.Distance(transform.position, targetPosition) < 3f){
                    state = (int)States.chase;
                    return;
                }

                if (body.velocity == Vector2.zero){
                    Vector2 oldTarget = new Vector2(movementPoints[movementPoints.Count - 1].cellX, movementPoints[movementPoints.Count - 1].cellY);
                    movementPoints.Clear();
                    index = 0;
                    movementPoints = pathFind(transform.position, oldTarget, pf);

                    targetPosition = new Vector2(movementPoints[index].cellX + 0.5f, movementPoints[index].cellY + 0.5f);
                }

                if (recheck <= 0){
                    recheck = 45;
                    movementPoints.Clear();
                    index = 0;
                    movementPoints = pathFind(transform.position, target.transform.position, pf);
                    targetPosition = new Vector2(movementPoints[index].cellX + 0.5f, movementPoints[index].cellY + 0.5f);
                }

                //Movement
                steering = seek.Movement(transform.position, targetPosition, body.velocity, speed);

                recheck--;

                // DEBUG BLOCK //
                //Debug.DrawRay(transform.position, body.velocity.normalized * 2, Color.green);
                //Debug.DrawRay(transform.position, desiredVelocity.normalized * 2, Color.magenta);
                //for (int i = 0; i < movementPoints.Count - 1; i++)
                //{
                //    Debug.DrawLine(new Vector2(movementPoints[i].cellX + 0.5f, movementPoints[i].cellY + 0.5f), new Vector2(movementPoints[i + 1].cellX + 0.5f, movementPoints[i + 1].cellY + 0.5f));
                //}
                //Debug.DrawLine(transform.position, targetPosition, Color.blue);
                //for (int i = 0; i < path.Count - 1; i++)
                //{
                //    Debug.DrawLine(new Vector2(path[i].cellX + 0.5f, path[i].cellY + 0.5f), new Vector2(path[i + 1].cellX + 0.5f, path[i + 1].cellY + 0.5f), Color.red);
                //}

                break;
            case (int)States.chase:
                if(target == null){
                    state = (int)States.wander;
                    return;
                }

                steering = seek.Movement((Vector2)target.transform.position, transform.position, body.velocity, speed);

                // distance = desiredVelocity.magnitude;
                dstToTarget = Vector2.Distance(transform.position, target.transform.position);
                if(dstToTarget > 10f)
                    state = (int)States.wander;

                break;
            case (int)States.flee:
                dstToTarget = Vector2.Distance(transform.position, target.transform.position);
                if (dstToTarget > 5f)
                {
                    state = (int)States.wander;
                }

                //Movement
                steering = flee.calculateMove((Vector2)target.transform.position, transform.position, body, speed);

                break;
            default:
                state = (int)States.wander;
                break;
        }

        body.velocity = Vector2.ClampMagnitude(body.velocity + steering, speed);
        transform.up = body.velocity.normalized;
    }

    private List<WorldTile> pathFind(Vector2 position, Vector2 target, Pathfinding pf)
    {
        List<WorldTile> movementPoints = new List<WorldTile>();
        path = pf.FindPathFromWorldPos(position, target);
        for (int i = 0; i < path.Count; i++){
            if (i + 1 <= path.Count - 1 && path[i].direction != path[i + 1].direction)
                movementPoints.Add(path[i]);
        }

        movementPoints.Add(path[path.Count - 1]);

        return movementPoints;
    }

    private void OnCollisionEnter2D(Collision2D col){
        if (col.gameObject.GetComponent<Player>() != null)
        {
            state = (int)States.flee;
        }
        if (col.gameObject.GetComponent<Dwarf>() != null)
        {
            if(col.gameObject.GetComponent<Dwarf>().state != 2){ // if dwarf is not following the player
                gm.AddGoblinScore(); 
                state = (int)States.wander;
                // Debug.Log("Entering wander");
            }
        }
    }
}
