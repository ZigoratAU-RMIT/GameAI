using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/Avoid Obstacles")]
public class AvoidObstacles : FilteredFlockBehavior
{
    public override Vector2 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        int layermask = 1 << 8;
        float radius = flock.avoidRadius; 
        Vector2 avoidanceMove = Vector2.zero;

        Vector2 currentPosition = (Vector2)agent.transform.position;
        Vector2 north = currentPosition + new Vector2(radius, 0);
        Vector2 south = currentPosition + new Vector2(-radius, 0);
        Vector2 east = currentPosition + new Vector2(0, radius);
        Vector2 west = currentPosition + new Vector2(0, -radius);

        RaycastHit2D hitNorth = Physics2D.Raycast(agent.transform.position, north, radius, layermask);
        RaycastHit2D hitSouth = Physics2D.Raycast(agent.transform.position, south, radius, layermask);
        RaycastHit2D hitEast = Physics2D.Raycast(agent.transform.position, east, radius, layermask);
        RaycastHit2D hitWest = Physics2D.Raycast(agent.transform.position, west, radius, layermask);

        Vector2 northMove = currentPosition;
        Vector2 southMove = currentPosition;
        Vector2 eastMove = currentPosition;
        Vector2 westMove = currentPosition;

        List<Vector2> collides = new List<Vector2>();
        if (hitNorth.collider != null)
        {
            collides.Add( (Vector2) hitNorth.transform.position - currentPosition);
        }

        if (hitSouth.collider != null)
        {
            collides.Add( (Vector2)hitSouth.transform.position - currentPosition);
        }

        if (hitEast.collider != null)
        {
            collides.Add((Vector2)hitEast.transform.position - currentPosition);
        }

        if (hitWest.collider != null)
        {
            collides.Add((Vector2)hitWest.transform.position - currentPosition);
        }
        float x = 0;
        float y = 0;
        foreach(Vector2 pos in collides){
            x += pos.x;
            y += pos.y;


        }

        x = x / collides.Count;
        y = y / collides.Count;

        return new Vector2(x, y);

    }
}
