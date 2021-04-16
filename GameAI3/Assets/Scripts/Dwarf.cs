using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dwarf : MonoBehaviour
{
    //Define states
    private enum States
    {
        wander,
        flee,
        follow,
    }

    int state = (int)States.wander;
    private int speed = 5;

    private Rigidbody2D body;
    private Vector2 steering;

    //Wander
    private Vector2 circleCentre;
    private Vector2 displacement;
    private Vector2 wanderForce;

    //Flee
    private Vector2 desiredVelocity;

    private GameObject target;
    public LayerMask targetMask;
    private Renderer rend;
    public List<GameObject> visibleTargets = new List<GameObject>();

    public int viewAngle = 180;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
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
                        float dstToTarget = Vector2.Distance(transform.position, target_.transform.position);
                        //If line draw form object to target is not interrupted by wall, add target to list of visible targets
                        //if(!Physics2D.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                        visibleTargets.Add(target_);
                        target = target_;

                        state = (int)States.flee;
                    }
                }

                break;
            case (int)States.flee:
                desiredVelocity = -((Vector2)target.transform.position - (Vector2)transform.position);
                // distance = desiredVelocity.magnitude;

                desiredVelocity = desiredVelocity.normalized * speed;

                steering = desiredVelocity - body.velocity;
                steering = Vector2.ClampMagnitude(steering, speed);
                steering /= 15f;
                break;
            case (int)States.follow:
                break;
            default:
                state = (int)States.wander;
                break;
        }
        body.velocity = Vector2.ClampMagnitude(body.velocity + steering, speed);
        transform.up = body.velocity.normalized;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.name == "Player")
        {
            rend.material.color = Color.blue;
            System.Threading.Thread.Sleep(500);
            Destroy(rend.gameObject);
        }
    }
}
