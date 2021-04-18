using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dwarf : MonoBehaviour
{
    //Define states
    private enum States
    {
        wander, //0
        flee,   //1
        follow, //2
    }

    public int state = (int)States.wander;
    private int speed = 5;

    private Rigidbody2D body;
    private FlockAgent flockAgent;
    private Vector2 flockVelocity;
    private Vector2 stateVelocity;
    private float flockWeight;
    private float stateWeight;
    private Vector2 steering;

    public Wander wander;
    public Seek seek;
    public FleeBehavior flee;
    public Follow follow;

    private GameObject target;
    public LayerMask targetMask;
    public List<GameObject> visibleTargets = new List<GameObject>();

    public int viewAngle = 180;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        flockAgent = GetComponent<FlockAgent>();
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
                flockWeight = 1;

                //flocking weigh behaviors for wander
                flockAgent.UpdateWeights(1.0f, 3.0f, 4.0f, 0.9f, 5.0f);
                break;
            case (int)States.flee:
                steering = flee.calculateMove((Vector2)target.transform.position, transform.position, body, speed);
                flockWeight = 1;

                //flock behaviour weights for flee
                flockAgent.UpdateWeights(1.0f, 3.0f, 4.0f, 0.9f, 5.0f);
                break;
            case (int)States.follow:
                if(Vector2.Distance(transform.position, target.transform.position) < 2f){
                    body.velocity = Vector2.zero;
                }
                follow.leader = target.GetComponent<Rigidbody2D>();
                steering = follow.Movement();
                flockWeight = 1;

                // flock behaviour weights for follow
                flockAgent.UpdateWeights(0.01f, 0.01f, 0.01f, 0.9f, 1.0f);
                Debug.Log("following");
                Debug.Log(gameObject);
                break;
            default:
                state = (int)States.wander;
                break;
        }
        body.velocity = Vector2.ClampMagnitude(body.velocity + steering, speed);
        stateWeight = 1;
        flockVelocity = flockAgent.GetFlockVelocity().normalized * flockWeight;
        stateVelocity = body.velocity.normalized * stateWeight;
        transform.up = flockVelocity + stateVelocity;
        if (flockWeight > 0){
            transform.position += (Vector3)flockAgent.GetFlockPosition();
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.GetComponent<Goblin>() != null)
        {
            if(state != (int)States.follow){
                flockAgent.RemoveFromFlock();
                Destroy(this.gameObject);
            }
        }

        if (col.gameObject.GetComponent<Player>() != null){
            // target = col.gameObject;
            if(!col.gameObject.GetComponent<Player>().followers.Contains(this)){
                int followersCount = col.gameObject.GetComponent<Player>().followers.Count;
                if(followersCount > 0){
                    target = col.gameObject.GetComponent<Player>().followers[followersCount-1].gameObject;
                } else if (followersCount == 0){
                    target = col.gameObject.GetComponent<Player>().gameObject;
                }
                state = (int)States.follow;
            }
        }
    }
}
