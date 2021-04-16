using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour
{
    //////// MAP GENERATOR

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

    // NPC Generator
    public GameObject dwarf;
    public int dwarfNum = 15;
    public GameObject goblin;
    public int goblinNum = 5;

    float offsetX;
    float offsetY;

    //////// CREATE GRID

    public GameObject gridNode;
    public GameObject nodePrefab;

    public int scanStartX = -1000, scanStartY = -1000, scanFinishX = 1000, scanFinishY = 1000, gridSizeX, gridSizeY;

    private List<GameObject> unsortedNodes = new List<GameObject>();
    public GameObject[,] nodes;
    int gridBoundX = 0, gridBoundY = 0;

    /////// RIVER GENERATION

    public List<string> riverDirection = new List<string>(){"north", "south"};
    List<string> edges = new List<string>(){"north", "south", "east", "west"};
    [Range(2,5)]
    public int riverWidth = 3;
    [Range(1,5)]
    public int riverBranch = 1;
    List<WorldTile> riverPath = new List<WorldTile>();
    List<WorldTile> branchPath = new List<WorldTile>();

    Pathfinding pf; // to access pathfinding functions

    void Awake(){
        pf = GetComponent<Pathfinding>();

        GenerateTiles();

        GenerateGrid();

        GenerateRiver();

        GenerateBranch();

        GenerateBorder();

        // GenerateNPC();
    }

    // MAIN METHODS
    void GenerateTiles(){
        // helper variables to make generated map different every time
        offsetX = Random.Range(0f,9999f);
        offsetY = Random.Range(0f,9999f);

        // initialize map
        for(int x = 0; x < mapWidth; x++){
            for(int y = 0; y < mapHeight; y++){
                // set base
                baseLayer.SetTile(TmapTransform(x, y), grass);
                
                // set obstacle
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

    void GenerateGrid(){
        int gridX = 0, gridY = 0;
        bool foundTileOnLastPass = false;
        for(int i = scanStartX; i < scanFinishX; i++){
            for(int j = scanStartY; j < scanFinishY; j++){
                TileBase tb = baseLayer.GetTile(new Vector3Int(i, j, 0));

                if(tb != null){
                    bool foundObstacle = false;
                    TileBase tb2 = obstacleLayer.GetTile(new Vector3Int(i, j, 0));
                    if(tb2 != null)
                        foundObstacle = true;

                    //The 0.5fs added to the worldposition coordinates help keep the cells centred
                    Vector3 worldPosition = new Vector3(i + 0.5f + map.transform.position.x, j + 0.5f + map.transform.position.y, 0);
                    GameObject node = (GameObject)Instantiate(nodePrefab, worldPosition, Quaternion.Euler(0,0,0));
                    Vector3Int cellPosition = baseLayer.WorldToCell(worldPosition);
                    WorldTile wt = node.GetComponent<WorldTile>();
                    wt.gridX = gridX;
                    wt.gridY = gridY;
                    wt.cellX = cellPosition.x;
                    wt.cellY = cellPosition.y;
                    node.transform.parent = gridNode.transform;

                    if(!foundObstacle){
                        foundTileOnLastPass = true;
                        node.name = "Walkable_" + gridX.ToString() + "_" + gridY.ToString();
                        node.GetComponent<WorldTile>().walkable = true;
                    }else{
                        foundTileOnLastPass = true;
                        node.name = "Unwalkable_" + gridX.ToString() + "_" + gridY.ToString();
                        node.GetComponent<SpriteRenderer>().color = Color.red;
                        node.GetComponent<WorldTile>().walkable = false;
                    }

                    unsortedNodes.Add(node);

                    gridY++;
                    if(gridX > gridBoundX)
                        this.gridBoundX = gridX;
                    if(gridY > gridBoundY)
                        this.gridBoundY = gridY;
                }
            }

            if(foundTileOnLastPass){
                gridX++;
                gridY = 0;
                foundTileOnLastPass = false;
            }

            gridNode.SetActive(false); //Disable if you would like to see the nodes rendered
        }

        nodes = new GameObject[gridBoundX + 1, gridBoundY + 1];

        foreach(GameObject g in unsortedNodes){
            WorldTile wt = g.GetComponent<WorldTile>();
            nodes[wt.gridX, wt.gridY] = g;
        }

        for(int i = 0; i < gridBoundX; i++){
            for(int j = 0; j < gridBoundY; j++){
                if(nodes[i, j] != null){
                    WorldTile wt = nodes[i, j].GetComponent<WorldTile>();
                    wt.myNeighbours = getNeighbours(i, j, gridBoundX, gridBoundY);
                    foreach(WorldTile neighbours in wt.myNeighbours){
                        if(!neighbours.walkable)
                            wt.specialCost = 10;
                    }
                }
            }
        }
    }

    void UpdateGrid(int x, int y, int specialCost, bool walkable){
        GameObject node = nodes[x, y];
        WorldTile wt = nodes[x, y].GetComponent<WorldTile>();
        if (walkable && !wt.walkable){
            wt.walkable = true;
            node.name = "Walkable_" + x.ToString() + "_" + y.ToString();
        } else if (!walkable && wt.walkable){
            wt.walkable = false;
            node.name = "Unwalkable_" + x.ToString() + "_" + y.ToString();
        }
        if (specialCost != wt.specialCost){
            wt.specialCost = specialCost;
        }
    }

    void GenerateRiver(){
        // get random walkable edge from player starting point
        List<Vector3Int> randomMapEdges = new List<Vector3Int>();
        foreach(string edge in riverDirection){
            Vector3Int edgePoint = GetMapEdge(edge);
            randomMapEdges.Add(edgePoint);
        }

        // pick starting position and end position from walkable edge
        Vector3Int startPos = randomMapEdges[0];
        Vector3Int endPos = randomMapEdges[1];

        WorldTile start = GetWorldTileByGrid(startPos.x, startPos.y);
        WorldTile end = GetWorldTileByGrid(endPos.x, endPos.y);

        riverPath = pf.FindPath(start, end);

        riverPath.Add(start);
        foreach(WorldTile tile in riverPath){
            for(int i=0;i<riverWidth;i++){
                int offset = i - (int)Mathf.Ceil((float)riverWidth/2.0f);
                if (tile.gridX+offset > 0 && tile.gridX+offset < mapWidth){
                    if(WorldTileObstacleFree(tile.gridX+offset, tile.gridY)){
                        baseLayer.SetTile(WorldTileTmapTransform(tile.gridX+offset, tile.gridY), water);
                        UpdateGrid(tile.gridX+offset, tile.gridY, 2, true); // special cost = 2
                    }
                }
            }
        }
    }
    
    void GenerateBranch(){
        List<Vector3Int> randomMapEdges = new List<Vector3Int>();
        for(int i=0; i < riverBranch;i++){
            int direction = Random.Range(0,4);
            if(!riverDirection.Contains(edges[direction])){ // branch can only go to other direction
                Vector3Int edgePoint = GetMapEdge(edges[direction]);
                randomMapEdges.Add(edgePoint);
            } else {
                i--;
            }
        }

        foreach(Vector3Int edge in randomMapEdges){
            // get a random river point
            int riverOffset = 10; // we only want to make branch from the middle of the main river
            int index = Random.Range(riverOffset, riverPath.Count-riverOffset);
            WorldTile start = riverPath[index];
            WorldTile end = GetWorldTileByGrid(edge.x, edge.y);
            int branchWidth = riverWidth-1; // branch should be less wide

            List<WorldTile> path = pf.FindPath(start, end);
            // path.Add(end);

            foreach(WorldTile tile in path){
                branchPath.Add(tile);
                for(int i=0;i<branchWidth;i++){
                    int offset = i - (int)Mathf.Ceil((float)riverWidth/2.0f);
                    if (tile.gridX+offset > 0 && tile.gridX+offset < mapWidth){
                        if(WorldTileObstacleFree(tile.gridX+offset, tile.gridY)){
                            baseLayer.SetTile(WorldTileTmapTransform(tile.gridX+offset, tile.gridY), water);
                            UpdateGrid(tile.gridX+offset, tile.gridY, 2, true); // special cost = 2
                        }
                    }
                }
            }
        }
    }

    void GenerateBorder(){
        for(int y=0; y < mapHeight; y++){
            obstacleLayer.SetTile(TmapTransform(0, y), tree);
            obstacleLayer.SetTile(TmapTransform(mapWidth-1, y), tree);
        }
        for(int x=0; x < mapWidth; x++){
            obstacleLayer.SetTile(TmapTransform(x, 0), tree);
            obstacleLayer.SetTile(TmapTransform(x, mapHeight-1), tree);
        }
    }

    // HELPER METHODS: Map Generator
    Vector3Int TmapTransform(int x, int y){
        int xTmap = -x + mapWidth/2;
        int yTmap = -y + mapHeight/2;
        int zTmap = 0;

        Vector3Int tmap = new Vector3Int(xTmap, yTmap, zTmap);

        return tmap;
    }

    Vector3Int WorldTileTmapTransform(int x, int y){
        int xTmap = x+1 - mapWidth/2;
        int yTmap = y+1 - mapHeight/2;
        int zTmap = 0;

        Vector3Int tmap = new Vector3Int(xTmap, yTmap, zTmap);

        return tmap;
    }

    bool InStartPointRadius(int x, int y){
        bool inRadius = false;

        if (Mathf.Abs(x) + Mathf.Abs(y) <= startPointRadius){
            inRadius = true;
        }

        return inRadius;
    }

    bool IsMapEdge(int x, int y){
        bool edge = false;

        if (x == 0 || x == mapWidth-1 || y == 0 || y == mapHeight-1){
            edge = true;
        }

        return edge;
    }

    // to check if a point is point is reachable by player
    bool ReachablePoint(int x, int y){
        bool reachable = false;
        WorldTile checkTile = GetWorldTileByGrid(x, y);
        WorldTile playerTile = GetWorldTileByGrid(mapWidth/2, mapHeight/2);
        List<WorldTile> path = pf.FindPath(playerTile, checkTile);

        if (path.Count > 0){
            reachable = true;
        }

        return reachable;
    }

    bool ObstacleFree(int x, int y){
        Vector3Int checkPoint = TmapTransform(x,y);
        return !obstacleLayer.HasTile(checkPoint);
    }

    bool WorldTileObstacleFree(int x, int y){
        Vector3Int checkPoint = WorldTileTmapTransform(x,y);
        return !obstacleLayer.HasTile(checkPoint);
    }

    // HELPER METHODS: river generation
    // to check if a point is returning high perlin noise value
    // hence the point can be used to put water tiles
    bool WaterAppropriate(int x, int y){
        bool pass = false;
        float xCoord = -(float)x/mapWidth * scale + offsetX;
        float yCoord = -(float)y/mapHeight * scale + offsetY;
        float sample = Mathf.PerlinNoise(xCoord, yCoord); // value would be between 0 and 1 inclusive

        if (sample > 0.5f && sample <= 0.7f){
            if (baseLayer.GetTile<Tile>(TmapTransform(x,y)) == grass){
                pass = true;  
            }
        }
        return pass;
    }

    Vector3Int GetMapEdge(string edge){
        bool walkable = false;
        bool reachable = false;
        Vector3Int checkPoint = new Vector3Int();
        int x = 0;
        int y = 0;
        
        while(!reachable){
            while(!walkable){
                if (edge == "north"){
                    x = Random.Range(riverWidth, mapHeight-1);
                    y = mapWidth-1;
                } else if (edge == "south"){
                    x = Random.Range(riverWidth, mapHeight-1);
                    y = 0;
                } else if (edge == "east"){
                    x = mapHeight-1;
                    y = Random.Range(riverWidth, mapWidth-1);
                } else if (edge == "west"){
                    x = 0; 
                    y = Random.Range(riverWidth, mapWidth-1);
                }
                if (ObstacleFree(x,y) && WaterAppropriate(x,y)){
                    walkable = true;
                }
            }
            if(ReachablePoint(x,y)){
                reachable = true;
            } else{
                walkable = false;
            }
        }

        checkPoint.x = x;
        checkPoint.y = y;

        return checkPoint;
    }

    

    // HELPER METHODS: NPC generation
    // this method is called from somewhere else to get location to instantiate the NPC
    public Vector3 GetRandomPoint(bool dwarf){
        bool pass = false;
        int x = 0;
        int y = 0;
        int dwarfLimit = 40;
        int goblinLimit = 20;
        GridLayout gl = obstacleLayer.transform.parent.GetComponentInParent<GridLayout>();
        while(!pass){
            if (dwarf){
                int index = Random.Range(0, riverPath.Count);
                x = riverPath[index].gridX;
                y = riverPath[index].gridY;
                if (y < dwarfLimit || y > mapWidth-dwarfLimit){
                    if(ObstacleFree(x,y)){
                        pass = true;
                    }
                }
            } else { // goblin
                int index = Random.Range(0, branchPath.Count);
                x = branchPath[index].gridX;
                y = branchPath[index].gridY;
                if (x < goblinLimit || x > mapWidth-goblinLimit){
                    if(ObstacleFree(x,y)){
                        pass = true;
                    }
                }
            }
        }
        return (Vector3)gl.WorldToCell(WorldTileTmapTransform(x,y));
    }

    // HELPER METHODS: Create Grid
    //Helper function to get the tile at a world coordinate
    public WorldTile GetWorldTileByCellPosition(Vector3 worldPosition){
        Vector3Int cellPosition = baseLayer.WorldToCell(worldPosition);
        WorldTile wt = null;
        for(int x = 0; x < mapWidth; x++){
            for(int y = 0; y < mapHeight; y++){
                if(nodes[x, y] != null){
                    WorldTile _wt = nodes[x, y].GetComponent<WorldTile>();
                    //we are interested in walkable cells only
                    if(_wt.walkable && _wt.cellX == cellPosition.x && _wt.cellY == cellPosition.y){
                        wt = _wt;
                        break;
                    }else{
                        continue;
                    }
                }
            }
        }
        return wt;
    }

    public WorldTile GetWorldTileByGrid(int x, int y){
        WorldTile wt = nodes[x, y].GetComponent<WorldTile>();
        return wt;
    }

    //Neighbour finding helper function, works for 8 directions
    //Might be a way to trim it down
    //Bounds checking done inside if statments
    public List<WorldTile> getNeighbours(int x, int y, int width, int height){
        List<WorldTile> myNeighbours = new List<WorldTile>();

        //Need to get all 8 directions
        // // / / // / /
        //1. Up = x, y + 1;
        //2. up-right = x + 1, y + 1;
        //3. right = x + 1, y;
        //4. down right = x + 1, y - 1
        //5. down = x, y - 1;
        //6. down left = x - 1, y - 1;
        //7. left = x - 1;
        //8. left up = x - 1, y + 1;

        if(y != height && nodes[x, y + 1] != null){
            WorldTile wt1 = nodes[x, y + 1].GetComponent<WorldTile>();
            if(wt1 != null) myNeighbours.Add(wt1); 
        }

        if(x != width && y != height && nodes[x + 1, y + 1] != null){
            WorldTile wt2 = nodes[x + 1, y + 1].GetComponent<WorldTile>();
            if(wt2 != null) myNeighbours.Add(wt2);
        }

        if(x != width && nodes[x + 1, y] != null){
            WorldTile wt3 = nodes[x + 1, y].GetComponent<WorldTile>();
            if(wt3 != null) myNeighbours.Add(wt3);
        }

        if(y != 0 && x != width && nodes[x + 1, y - 1] != null){
            WorldTile wt4 = nodes[x + 1, y - 1].GetComponent<WorldTile>();
            if(wt4 != null) myNeighbours.Add(wt4);
        }

        if(y != 0 && nodes[x, y - 1] != null){
            WorldTile wt5 = nodes[x, y - 1].GetComponent<WorldTile>();
            if(wt5 != null) myNeighbours.Add(wt5);
        }


        if(x != 0 && y != 0 && nodes[x - 1, y - 1] != null){
            WorldTile wt6 = nodes[x - 1, y - 1].GetComponent<WorldTile>();
            if(wt6 != null) myNeighbours.Add(wt6);
        }

        if(x != 0 && nodes[x - 1, y] != null){
            WorldTile wt7 = nodes[x - 1, y].GetComponent<WorldTile>();
            if(wt7 != null) myNeighbours.Add(wt7);
        }

        if(x != 0 && y != height && nodes[x - 1, y + 1] != null){
            WorldTile wt8 = nodes[x - 1, y + 1].GetComponent<WorldTile>();
            if(wt8 != null) myNeighbours.Add(wt8);
        }

        return myNeighbours;
    }
}