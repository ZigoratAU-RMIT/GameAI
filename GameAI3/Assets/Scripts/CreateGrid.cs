using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class CreateGrid : MonoBehaviour{
    /*************
    The creation of the grid that we use as the basis for our A* pathfinding
    - Code sourced heavily from:
        pavcreations.com

    - Other references from:
        http://www.jgallant.com/nodal-pathfinding-in-unity-2d-with-a-in-non-grid-based-games/

    - Issues
        - As it stands the nodes needed for path finding are instantiated into the game world, and not apart of some abstract data structure
            this isn't very good behaviour and might need to be fixed
    *************/

    public Grid gridBase;
    public Tilemap floor;
    public List<Tilemap> obstacleLayers;
    public GameObject gridNode;
    public GameObject nodePrefab;

    public int scanStartX = -1000, scanStartY = -1000, scanFinishX = 1000, scanFinishY = 1000, gridSizeX, gridSizeY;

    private List<GameObject> unsortedNodes = new List<GameObject>();
    public GameObject[,] nodes;
    private int gridBoundX = 0, gridBoundY = 0;


    void Start(){
        gridSizeX = Mathf.Abs(scanStartX) + Mathf.Abs(scanFinishX);
        gridSizeY = Mathf.Abs(scanStartY) + Mathf.Abs(scanFinishY);
        createGrid();
    }

    void createGrid(){
        int gridX = 0, gridY = 0;
        bool foundTileOnLastPass = false;
        for(int i = scanStartX; i < scanFinishX; i++){
            for(int j = scanStartY; j < scanFinishY; j++){
                TileBase tb = floor.GetTile(new Vector3Int(i, j, 0));

                if(tb != null){
                    bool foundObstacle = false;
                    foreach(Tilemap t in obstacleLayers){
                        TileBase tb2 = t.GetTile(new Vector3Int(i, j, 0));
                        if(tb2 != null)
                            foundObstacle = true;
                    }

                    //The 0.5fs added to the worldposition coordinates help keep the cells centred
                    Vector3 worldPosition = new Vector3(i + 0.5f + gridBase.transform.position.x, j + 0.5f + gridBase.transform.position.y, 0);
                    GameObject node = (GameObject)Instantiate(nodePrefab, worldPosition, Quaternion.Euler(0,0,0));
                    Vector3Int cellPosition = floor.WorldToCell(worldPosition);
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
                }
            }
        }
    }

    //Helper function to get the tile at a world coordinate
    public WorldTile GetWorldTileByCellPosition(Vector3 worldPosition){
        Vector3Int cellPosition = floor.WorldToCell(worldPosition);
        WorldTile wt = null;
        for(int x = 0; x < gridBoundX; x++){
            for(int y = 0; y < gridBoundY; y++){
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
