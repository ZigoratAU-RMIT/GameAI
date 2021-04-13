using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/Composite1")]
public class CompositeBehavior : FlockBehavior
{
    public FlockBehavior[] behaviors ;
    public float[] weights;
    public int i;

    public override Vector2 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        //handle data mismatch
        if (agent.weights.Length != behaviors.Length)
        {
            Debug.LogError("Data mismatch in " + name, this);
            return Vector2.zero;
        }

        //set up move
        Vector2 move = Vector2.zero;

        //iterate through behaviors
        for (int i = 0; i < behaviors.Length; i++)
        {
            Vector2 partialMove = behaviors[i].CalculateMove(agent, context, flock) * agent.weights[i];

            if (partialMove != Vector2.zero)
            {
                if (partialMove.sqrMagnitude > agent.weights[i] * agent.weights[i])
                {
                    partialMove.Normalize();
                    partialMove *= agent.weights[i];
                }

                move += partialMove;

            }
        }

        return move;


    }
}
