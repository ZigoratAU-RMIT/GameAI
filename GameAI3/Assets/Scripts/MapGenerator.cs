using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/****
PROCEDURAL GENERATION
to do:
- create streams on a different layer
- check if at the starting point, player can get to the 4 corners of the board -- uses pathfinding
****/

public class MapGenerator : MonoBehaviour {
    public int mapWidth;
    public int mapHeight;

    public Grid map;
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

    // NPC
    public GameObject dwarf;
    public int dwarfNum = 15;
    public GameObject goblin;
    public int goblinNum = 5;
    
    // RIVER GENERATION HELPER VARIABLES
    // public string[] riverDirection = {"north", "south"};
    // [Range(1,5)]
    // public int riverWidth = 3; // must be odd number

    // public Pathfinding pf; // to access pathfinding functions
    // public CreateGrid cg; // to access creategrid functions

    // public GameObject gridNode;
    // public GameObject nodePrefab;

    float offsetX;
    float offsetY;

    // Start is called before the first frame update
    void Awake() {
        // helper variables to make generated map different every time
        offsetX = Random.Range(0f,9999f);
        offsetY = Random.Range(0f,9999f);

        // initialize map
        for(int x = 0; x < mapWidth; x++){
            for(int y = 0; y < mapHeight; y++){
                // set base
                baseLayer.SetTile(TmapTransform(x, y), grass);
                
                // set obstacle
                if (IsMapEdge(x,y)){
                    obstacleLayer.SetTile(TmapTransform(x,y), tree);
                } else {
                    if (!InStartPointRadius(-x + mapWidth/2, -y + mapHeight/2)){
                        float xCoord = -(float)x/mapWidth * scale + offsetX;
                        float yCoord = -(float)y/mapHeight * scale + offsetY;
                        float sample = Mathf.PerlinNoise(xCoord, yCoord); // value would be between 0 and 1 inclusive

                        if (sample > 0f && sample <= 0.2f){
                            obstacleLayer.SetTile(TmapTransform(x, y), rock);
                        } else if (sample > 0.2f && sample <= 0.4f){ //|| sample > 0.65f && sample <= 0.7f  ){
                            obstacleLayer.SetTile(TmapTransform(x, y), tree);
                        }
                    }
                }
            }
        }

        GridLayout gl = obstacleLayer.transform.parent.GetComponentInParent<GridLayout>();
        
        // NPC generation
        for(int i=0;i<dwarfNum;i++){
            Vector3 dwarfPos = (Vector3)gl.WorldToCell(GetRandomPoint());
            Instantiate(dwarf, dwarfPos, Quaternion.identity);
        }

        for(int i=0;i<goblinNum;i++){
            Vector3 goblinPos = (Vector3)gl.WorldToCell(GetRandomPoint());
            Instantiate(goblin, goblinPos, Quaternion.identity);
        }
    }

    Vector3Int TmapTransform(int x, int y){
        int xTmap = -x + mapWidth/2;
        int yTmap = -y + mapHeight/2;
        int zTmap = 0;

        Vector3Int tmap = new Vector3Int(xTmap, yTmap, zTmap);

        return tmap;
    }

    Vector3Int GetRandomPoint(){
        bool pass = false;
        int x = 0;
        int y = 0;
        while(!pass){
            x = Random.Range(0, mapWidth-1);
            y = Random.Range(0, mapHeight-1);
            pass = ObstacleFree(TmapTransform(x,y));
        }
        return TmapTransform(x,y);
    }

    // to be replaced by reachable point method
    bool InStartPointRadius(int x, int y){
        bool inRadius = false;

        if (Mathf.Abs(x) + Mathf.Abs(y) <= startPointRadius){
            inRadius = true;
        }

        return inRadius;
    }

    bool ObstacleFree(Vector3Int checkPoint){
        return !obstacleLayer.HasTile(checkPoint);
    }

    bool IsMapEdge(int x, int y){
        bool edge = false;
        if (x == 0 || x == mapWidth-1 || y == 0 || y == mapHeight-1){
            edge = true;
        }
        return edge;
    }

    // RIVER GENERATION CODE
    // // to check if a point is reachable to different ends
    // bool ReachablePoint(int x, int y){
    //     bool reachable = false;
    //     string[] edges = {"north", "south", "east", "west"};
    //     WorldTile currPos = cg.GetWorldTileByGrid(x, y);
    //     WorldTile checkPos = null;
    //     Vector3Int checkGrid = new Vector3Int();
    //     List<WorldTile> path = null;

    //     int pass = 2;
    //     int count = 0;

    //     // check north
    //     foreach (string edge in edges){
    //         checkGrid = GetRandomWalkableEdge(edge);
    //         checkPos = cg.GetWorldTileByGrid(checkGrid.x, checkGrid.y);
    //         path = pf.FindPath(currPos, checkPos);
    //         if (path.Count > 0){
    //             count++;
    //         }
    //     }

    //     if (count >= pass){
    //         reachable = true;
    //     }

    //     return reachable;
    // }

    // Vector3Int GetRandomWalkableEdge(string edge){

    //     bool walkable = false;
    //     Vector3Int checkPoint = new Vector3Int();
        
    //     while(!walkable){
    //         checkPoint = GetEdgeValue(edge);
    //         if (ObstacleFree(checkPoint)){
    //             walkable = true;
    //         }
    //     }

    //     return checkPoint;
    // }

    // // to check if a point is returning high perlin noise value
    // // hence the point can be used to put water tiles
    // bool WaterAppropriate(int x, int y){
    //     bool pass = false;
    //     float xCoord = -(float)x/mapWidth * scale + offsetX;
    //     float yCoord = -(float)y/mapHeight * scale + offsetY;
    //     float sample = Mathf.PerlinNoise(xCoord, yCoord); // value would be between 0 and 1 inclusive

    //     if (sample > 0.6f && sample <= 1f){
    //         pass = true;
    //     }
    //     return pass;
    // }

    // void GenerateRiver(){

    //     bool linkable = false;
    //     Vector3Int startPos = new Vector3Int();
    //     Vector3Int endPos = new Vector3Int();
    //     List<WorldTile> path = null;

    //     while(!linkable){
    //         startPos = GetRiverEndpoint(0);
    //         endPos = GetRiverEndpoint(1);

    //         WorldTile start = cg.GetWorldTileByGrid(startPos.x, startPos.y);
    //         WorldTile end = cg.GetWorldTileByGrid(endPos.x, endPos.y);

    //         path = pf.FindPath(start, end);

    //         if (path.Count > 0){
    //             linkable = true;
    //         }
    //     }

    //     foreach(WorldTile tile in path){
    //         obstacleLayer.SetTile(TmapTransform(tile.getGridX(), tile.getGridY()), water);
    //     }

    // }

    // // RIVER HELPER METHODS
    // Vector3Int GetRiverEndpoint(int i){
    //     // i = 0 means get start pos
    //     // i = 1 means get end pos
    //     Vector3Int pos = new Vector3Int();
    //     bool reachable = false;
        
    //     while(!reachable){
    //         pos = GetRandomWalkableEdge(riverDirection[i]);

    //         if(ReachablePoint(pos.x, pos.y)){
    //             reachable = true;
    //         }
    //     }
        
    //     return pos;
    // }

    // Vector3Int GetEdgeValue(string edge){
    //     Vector3Int edgeValue = new Vector3Int(0,0,0);
    //     if (edge == "north"){
    //         edgeValue.x = Random.Range(riverWidth, mapHeight-1);
    //         edgeValue.y = 0;
    //     } else if (edge == "south"){
    //         edgeValue.x = Random.Range(riverWidth, mapHeight-1);
    //         edgeValue.y = mapWidth-1;
    //     } else if (edge == "east"){
    //         edgeValue.x = 0;
    //         edgeValue.y = Random.Range(riverWidth, mapWidth-1);
    //     } else if (edge == "west"){
    //         edgeValue.x = mapHeight-1;
    //         edgeValue.y = Random.Range(riverWidth, mapWidth-1);
    //     }
    //     return edgeValue;
    // }
}
