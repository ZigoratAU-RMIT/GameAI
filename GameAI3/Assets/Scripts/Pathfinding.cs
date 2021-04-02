using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinding : MonoBehaviour{
    /*************
    Brains of the pathfinding implementation
    - Code sourced heavily from:
        pavcreations.com
    
    - Other references from:
        http://www.jgallant.com/nodal-pathfinding-in-unity-2d-with-a-in-non-grid-based-games/


    Issues:
        - No checking for if the found path is invalid resulting in null exeption error being thrown until it can make a path
        - Pathfinding isn't independant to movement right now
        - Weird behaviours where the entity can get stuck
    *************/

    public CreateGrid cg; //So we can access grid functions

    Vector3 lastDirection = Vector3.zero;
    List<WorldTile> reachedPathTiles = new List<WorldTile>();

    //The bread and butter of the A* angorithm
    public List<WorldTile> FindPath(Vector3 startPosition, Vector3 endPosition){
        List<WorldTile> path = new List<WorldTile>();
        WorldTile startNode = cg.GetWorldTileByCellPosition(startPosition);
        WorldTile targetNode = cg.GetWorldTileByCellPosition(endPosition);
        
        List<WorldTile> openSet = new List<WorldTile>();
        HashSet<WorldTile> closedSet = new HashSet<WorldTile>();
        openSet.Add(startNode);

        while(openSet.Count > 0){
            WorldTile currentNode = openSet[0];
            for(int i = 1; i < openSet.Count; i++){
                if(openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost){
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if(currentNode == targetNode){
                path = RetracePath(startNode, targetNode);
                return path ;
            }

            foreach(WorldTile neighbour in currentNode.myNeighbours){
                if(!neighbour.walkable || closedSet.Contains(neighbour)) continue;

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if(newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)){
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if(!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }   
        }
        return path;
    }

    //Distance helper function
    //May not be calculating the value of a directional movement correctly
    int GetDistance(WorldTile nodeA, WorldTile nodeB){
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if(dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    //The construction of the walkable path
    //It is then reversed so the entity can iterate through it
    List<WorldTile> RetracePath(WorldTile startNode, WorldTile targetNode){
        List<WorldTile> path = new List<WorldTile>();
        WorldTile currentNode = targetNode;

        Debug.Log(startNode);

        while(currentNode != startNode){
            //direction obtained to assist with steering implementation
            currentNode.direction = GetDirection(currentNode, currentNode.parent);
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }


    //Direction helper function
    //Sets the direction the entity must go between nodes 
    //Will assist with steering
    int GetDirection(WorldTile nodeA, WorldTile nodeB){
        int direction = 0;
        if(nodeA.gridX == nodeB.gridX){
            if(nodeA.gridY < nodeB.gridY)
                direction = (int)WorldTile.Directions.Down;
            else if(nodeA.gridY > nodeB.gridY)
                direction = (int)WorldTile.Directions.Up;
        }
        else if(nodeA.gridY == nodeB.gridY){
            if(nodeA.gridX < nodeB.gridX)
                direction = (int)WorldTile.Directions.Right;
            else if(nodeA.gridX > nodeB.gridX)
                direction = (int)WorldTile.Directions.Left;
        }
        else if(nodeA.gridX < nodeB.gridX){
            if(nodeA.gridY > nodeB.gridY)
                direction = (int)WorldTile.Directions.UpRight;
            else if(nodeA.gridY < nodeB.gridY)
                direction = (int)WorldTile.Directions.DownRight;
        }
        else if(nodeA.gridX > nodeB.gridX){
            if(nodeA.gridY > nodeB.gridY)
                direction = (int)WorldTile.Directions.UpLeft;
            else if(nodeA.gridY < nodeB.gridY)
                direction = (int)WorldTile.Directions.DownLeft;
        }

        return direction;
    }
}
