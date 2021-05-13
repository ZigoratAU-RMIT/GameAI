﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// the base flocking code is taken from https://github.com/boardtobits/flocking-algorithm


public class Flock : MonoBehaviour
{
    public FlockAgent agentPrefab;
    public Map map;
    List<FlockAgent> agents = new List<FlockAgent>();
    public FlockBehavior behavior;

    GameManager gm;

    [Range(5, 200)]
    public int startingCount = 15;
    const float AgentDensity = 0.08f;

    [Range(1f, 100f)]
    public float driveFactor = 10f;
    [Range(1f, 100f)]
    public float maxSpeed = 5f;
    [Range(1f, 10f)]
    public float neighborRadius = 1.5f;
    [Range(0f, 1f)]
    public float avoidanceRadiusMultiplier = 0.5f;

    public float avoidRadius;

    float squareMaxSpeed;
    float squareNeighborRadius;
    float squareAvoidanceRadius;
    public float SquareAvoidanceRadius { get { return squareAvoidanceRadius; } }

    public int mapSizeX = 50;

    public int mapSizeY = 50;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        avoidRadius = avoidanceRadiusMultiplier + neighborRadius;
        squareMaxSpeed = maxSpeed * maxSpeed;
        squareNeighborRadius = neighborRadius * neighborRadius;
        squareAvoidanceRadius = squareNeighborRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier;

        for (int i = 0; i < startingCount; i++)
        {
            bool isDwarf = (agentPrefab.name == "Dwarf") ? true : false;
            Vector3 agentPos = map.GetRandomPoint(isDwarf);
            FlockAgent newAgent = Instantiate(
                agentPrefab, agentPos, Quaternion.identity, transform);
            newAgent.name = agentPrefab.name + i;
            newAgent.Initialize(this);
            agents.Add(newAgent);
        }

        gm.SetDwarvesRemaining(agents.Count);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (FlockAgent agent in agents)
        {
            List<Transform> context = GetNearbyObjects(agent);

            //FOR DEMO ONLY
            //agent.GetComponentInChildren<SpriteRenderer>().color = Color.Lerp(Color.white, Color.red, context.Count / 6f);

            Vector2 move = behavior.CalculateMove(agent, context, this);
            move *= driveFactor;
            if (move.sqrMagnitude > squareMaxSpeed)
            {
                move = move.normalized * maxSpeed;
            }
            agent.UpdateFlockMove(move);
        }
    }

    List<Transform> GetNearbyObjects(FlockAgent agent)
    {
        List<Transform> context = new List<Transform>();
        Collider2D[] contextColliders = Physics2D.OverlapCircleAll(agent.transform.position, neighborRadius);
        foreach (Collider2D c in contextColliders)
        {
            if (c != agent.AgentCollider)
            {
                context.Add(c.transform);
            }
        }
        return context;
    }

    public void RemoveAgent(FlockAgent agent){
        agents.Remove(agent);
        gm.MinusDwarvesRemaining();
    }

}
