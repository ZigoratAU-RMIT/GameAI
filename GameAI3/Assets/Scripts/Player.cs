using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour{
    public Pathfinding pf;
    List<WorldTile> movementPoints = new List<WorldTile>();
    List<WorldTile> path = new List<WorldTile>(); 

    float speed = 5f;
    private int index = 0;
    private bool movementDone = true;

    void Update(){
        if(Input.GetButtonDown("Fire1")){
            //Make sure our movement is all reset
            movementPoints.Clear();
            index = 0;
            movementDone = false;

            //Finding the path - this trims the whole pathfinding algorithm down to nodes
            //These nodes are based on when the direction the entity must go changes
            path = pf.FindPath(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            for(int i = 0; i < path.Count; i++){          
                if(i + 1 <= path.Count - 1&& path[i].direction != path[i + 1].direction){
                    movementPoints.Add(path[i]);
                }
            }
            //Add in last position
            movementPoints.Add(path[path.Count - 1]);
        }
    }
}
