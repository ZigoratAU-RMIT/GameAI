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

    public Wander wander;
    public Seek seek;
    public FleeBehavior flee;
    public Follow follow;

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
                steering = wander.Movement(body.velocity, speed);

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
                steering = flee.calculateMove((Vector2)target.transform.position, transform.position, body, speed);
                break;
            case (int)States.follow:
                if(Vector2.Distance(transform.position, target.transform.position) < 2f){
                    body.velocity = Vector2.zero;
                }
                steering = follow.Movement();
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
        if (col.gameObject.GetComponent<Goblin>() != null)
        {
            Destroy(this.gameObject);
        }

        if (col.gameObject.GetComponent<Player>() != null){
            target = col.gameObject;
            state = (int)States.follow;
        }
    }
}
