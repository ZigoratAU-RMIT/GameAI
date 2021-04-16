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

    //Pathfinding
    public Pathfinding pf;
    List<WorldTile> movementPoints = new List<WorldTile>();
    List<WorldTile> path = new List<WorldTile>();

    int state = (int)States.wander;
    private int speed = 5;

    private Rigidbody2D body;
    private Renderer rend;
    private Vector2 steering;

    //Wander
    private Vector2 circleCentre;
    private Vector2 displacement;
    private Vector2 wanderForce;


    //Seek & Flee
    private Vector2 desiredVelocity;
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
        body = GetComponent<Rigidbody2D>();
        rend = GetComponent<Renderer>();
    }

    void FixedUpdate()
    {
        switch (state)
        {
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

                //Finding target
                visibleTargets.Clear();
                Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, 10, targetMask);

                for (int i = 0; i < targetsInViewRadius.Length; i++)
                {
                    GameObject target_ = targetsInViewRadius[i].gameObject;
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

                        state = (int)States.seek;
                    }
                }

                Debug.DrawRay(transform.position, displacement, Color.green);
                Debug.DrawRay(transform.position, circleCentre, Color.magenta);
                break;
            case (int)States.seek:

                if (Vector2.Distance(transform.position, targetPosition) < 3f)
                {
                    state = (int)States.chase;
                    return;
                }

                if (body.velocity == Vector2.zero)
                {
                    Vector2 oldTarget = new Vector2(movementPoints[movementPoints.Count - 1].cellX, movementPoints[movementPoints.Count - 1].cellY);
                    movementPoints.Clear();
                    index = 0;
                    movementPoints = pathFind(transform.position, oldTarget, pf);

                    targetPosition = new Vector2(movementPoints[index].cellX + 0.5f, movementPoints[index].cellY + 0.5f);
                }

                if (recheck <= 0)
                {
                    recheck = 45;
                    movementPoints.Clear();
                    index = 0;
                    movementPoints = pathFind(transform.position, target.transform.position, pf);
                    targetPosition = new Vector2(movementPoints[index].cellX + 0.5f, movementPoints[index].cellY + 0.5f);
                }

                desiredVelocity = targetPosition - (Vector2)transform.position;
                desiredVelocity = desiredVelocity.normalized * speed;

                steering = desiredVelocity - body.velocity;
                steering = Vector2.ClampMagnitude(steering, speed);
                steering /= 15f;

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
                desiredVelocity = (Vector2)target.transform.position - (Vector2)transform.position;
                desiredVelocity = desiredVelocity.normalized * speed;

                steering = desiredVelocity - body.velocity;
                steering = Vector2.ClampMagnitude(steering, speed);
                steering /= 15f;

                // distance = desiredVelocity.magnitude;
                dstToTarget = Vector2.Distance(transform.position, target.transform.position);
                //print(dstToTarget);
                if (dstToTarget < 1f)
                {
                    state = (int)States.flee;
                }

                break;
            case (int)States.flee:
                desiredVelocity = -((Vector2)target.transform.position - (Vector2)transform.position);
                // distance = desiredVelocity.magnitude;

                desiredVelocity = desiredVelocity.normalized * speed;

                steering = desiredVelocity - body.velocity;
                steering = Vector2.ClampMagnitude(steering, speed);
                steering /= 15f;

                // distance = desiredVelocity.magnitude;
                dstToTarget = Vector2.Distance(transform.position, target.transform.position);
                //print(dstToTarget);
                if (dstToTarget > 5f)
                {
                    state = (int)States.wander;
                }

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
        for (int i = 0; i < path.Count; i++)
        {
            if (i + 1 <= path.Count - 1 && path[i].direction != path[i + 1].direction)
                movementPoints.Add(path[i]);
        }

        movementPoints.Add(path[path.Count - 1]);

        return movementPoints;
    }



    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.name == "Player")
        {
            rend.material.color = Color.red;
            System.Threading.Thread.Sleep(500);
            Destroy(rend.gameObject);
        }
    }
}
