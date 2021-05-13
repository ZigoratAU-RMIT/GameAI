using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class DwarfAgent : Agent
{
    Rigidbody2D rBody;
    public float speedMultiplier = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
    }

    public override void OnEpisodeBegin()
    {

    }

    public override void CollectObservations(VectorSensor sensor)
    {

    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        Vector2 forceDirection = Vector2.zero;
        switch (actionBuffers.DiscreteActions[0]) {
            case 0:
                forceDirection = Vector2.zero;
                break;
            case 1:
                forceDirection = new Vector2(1.0f, 0.0f);
                break;
            case 2:
                forceDirection = new Vector2(1.0f, 0.0f);
                break;
            case 3:
                forceDirection = new Vector2(0.0f, 1.0f);
                break;
            case 4:
                forceDirection = new Vector2(0.0f, -1.0f);
                break;
            case 5:
                forceDirection = new Vector2(1.0f, 1.0f);
                break;
            case 6:
                forceDirection = new Vector2(1.0f, -1.0f);
                break;
            case 7:
                forceDirection = new Vector2(-1.0f, 1.0f);
                break;
            case 8:
                forceDirection = new Vector2(-1.0f, -1.0f);
                break;
            default:
                forceDirection = Vector2.zero;
                break;
        }

        rBody.AddForce(forceDirection * speedMultiplier);
    }

}
