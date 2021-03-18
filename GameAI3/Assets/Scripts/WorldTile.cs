using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTile : MonoBehaviour{
    /*************
    Data structure for the worldTiles
    Might need further abstraction because movement nodes share exactsame data with worldTiles

    - Code sourced heavily from:
        pavcreations.com
    
    - Other references from:
        http://www.jgallant.com/nodal-pathfinding-in-unity-2d-with-a-in-non-grid-based-games/
    *************/

    //G-cost is distance from starting cell
    //H-cost is distance to end cell
    //f-cost is the sum of g and h

    //Clockwise
    public enum Directions{
        Up,         //0
        UpRight,    //1
        Right,      //2
        DownRight,  //3
        Down,       //4
        DownLeft,   //5
        Left,       //6
        UpLeft      //7
    }

    public int gCost;
    public int hCost;
    public int gridX, gridY;
    public float cellX, cellY;
    public bool walkable = true;
    public int direction = 0;

    public List<WorldTile> myNeighbours;
    public WorldTile parent;

    public WorldTile(bool walkable, int gridX, int gridY){
        this.walkable = walkable;
        this.gridX = gridX;
        this.gridY = gridY;
    }

    public void setDirection(int direction){
        this.direction = direction;
    }

    public int fCost{
        get{
            return gCost + hCost;
        }
    }
}

