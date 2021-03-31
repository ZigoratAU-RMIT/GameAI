using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/****
PROCEDURAL GENERATION
to do:
- clear out obstacle layer at a specified radius
- create streams on a different layer
- check if at the starting point, player can get to the 4 corners of the board -- uses pathfinding
****/

public class MapGenerator : MonoBehaviour {
    public int mapWidth;
    public int mapHeight;

    public Vector3Int tmapSize;

    public Tilemap baseLayer;
    public Tilemap obstacleLayer;

    // base layer tiles
    public Tile grass;
    public Tile water;

    // obstacle layer
    [Range(0f,100f)]
    public float scale = 15f;
    public Tile tree;
    public Tile rock;

    [Range(0,20)]
    public int startPointRadius=5;

    private Pathfinding pf; // to access pathfinding functions

    // Start is called before the first frame update
    void Awake() {
        // helper variables to make generated map different every time
        float offsetX = Random.Range(0f,9999f);
        float offsetY = Random.Range(0f,9999f);

        // initialize map
        for(int x = 0; x < mapWidth; x++){
            for(int y = 0; y < mapHeight; y++){
                // set base
                int xTmap = -x + mapWidth/2;
                int yTmap = -y + mapHeight/2;
                int zTmap = 0;

                baseLayer.SetTile(new Vector3Int(xTmap, yTmap, zTmap), grass);
                
                // set obstacle
                if (!InStartPointRadius(xTmap, yTmap)){
                    float xCoord = -(float)x/mapWidth * scale + offsetX;
                    float yCoord = -(float)y/mapHeight * scale + offsetY;
                    float sample = Mathf.PerlinNoise(xCoord, yCoord); // value would be between 0 and 1 inclusive

                    if (sample >= 0f && sample < 0.2f){
                        obstacleLayer.SetTile(new Vector3Int(xTmap, yTmap, 0), rock);
                    } else if (sample > 0.2f && sample < 0.35f){ //|| sample > 0.65f && sample <= 0.7f  ){
                        obstacleLayer.SetTile(new Vector3Int(xTmap, yTmap, 0), tree);
                    }
                }
            }
        }
    }

    bool InStartPointRadius(int x, int y){
        bool inRadius = false;

        if (Mathf.Abs(x) + Mathf.Abs(y) <= startPointRadius){
            inRadius = true;
        }

        return inRadius;
    }  
}
