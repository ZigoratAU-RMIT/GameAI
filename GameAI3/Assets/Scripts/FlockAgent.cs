using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FlockAgent : MonoBehaviour
{

    Flock agentFlock;
    public Flock AgentFlock { get { return agentFlock; } }

    Collider2D agentCollider;
    public Collider2D AgentCollider { get { return agentCollider; } }

    Vector2 flockVelocity; // for transform.up
    Vector2 flockPosition;


    // avoidance, alignment, steered cohesion, stay in radius, avoid obstacles
    public float[] weights= { 1, 3, 4, 1, 5};

    // Start is called before the first frame update
    void Start()
    {
        agentCollider = GetComponent<Collider2D>();
        
    }

    public void Initialize(Flock flock)
    {
        agentFlock = flock;
    }

    public void UpdateFlockMove(Vector2 velocity)
    {
        flockVelocity = velocity;
        flockPosition = (Vector3)velocity * Time.deltaTime;
    }

    public Vector2 GetFlockVelocity(){
        return flockVelocity;
    }

    public Vector2 GetFlockPosition(){
        return flockPosition;
    }

    public void RemoveFromFlock(){
        agentFlock.RemoveAgent(this);
    }

    public void UpdateWeights(float a, float b, float c, float d, float e){
        weights[0] = a;
        weights[1] = b;
        weights[2] = c;
        weights[3] = d;
        weights[4] = e;
    }
}
