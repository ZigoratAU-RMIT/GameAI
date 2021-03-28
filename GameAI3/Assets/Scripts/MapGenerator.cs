using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour {
    public int mapWidth;
    public int mapHeight;

    public Vector3Int tmapSize;

    public Tilemap baseLayer;
    public Tilemap topLayer;

    // base layer tiles
    public Tile grass;
    public Tile water;

    // top layer
    public Tile tree;
    [Range(1,100)]
    public int treeChance;
    [Range(1,100)]
    public int treeNearbyChance; // chance of putting tree if there is another tree nearby
    // public int treeNum; // ignore this for now
    private int treePlacedNum;

    public Tile rock;
    [Range(1,100)]
    public int rockChance;
    [Range(1,100)]
    public int rockNearbyChance; // chance of putting rock if there is another rock nearby
    // public int rockNum; // ignore this for now
    private int rockPlacedNum;

    // Start is called before the first frame update
    void Start() {
        // initialize map
        for(int x = 0; x < mapWidth; x++){
            for(int y = 0; y < mapHeight; y++){
                // set base
                baseLayer.SetTile(new Vector3Int(-x + mapWidth/2, -y + mapHeight/2, 0), grass);
                
                // set top
                // check if currentPos is Player starting point
                bool playerStartPoint = (-x + mapWidth/2 == 0 && -y + mapHeight/2 == 0) ? true : false;
                int randomNum = Random.Range(1,100);
                
                if (!playerStartPoint){
                    // check surroundings, if there's tree around
                    if (TileNearby(x, y, tree, topLayer)){
                        if (randomNum < treeNearbyChance+1){
                            topLayer.SetTile(new Vector3Int(-x + mapWidth/2, -y + mapHeight/2, 0), tree);
                            treePlacedNum++;
                        }
                    } else {
                        if (randomNum < treeChance+1){
                            topLayer.SetTile(new Vector3Int(-x + mapWidth/2, -y + mapHeight/2, 0), tree);
                            treePlacedNum++;
                        }
                    }

                    if (topLayer.GetTile<Tile>(new Vector3Int(-x + mapWidth/2, -y + mapHeight/2, 0)) != tree){
                        // check surroundings, if there's rock around
                        if (TileNearby(x, y, rock, topLayer)){
                            if (randomNum < rockNearbyChance+1){
                                topLayer.SetTile(new Vector3Int(-x + mapWidth/2, -y + mapHeight/2, 0), rock);
                                rockPlacedNum++;
                            }
                        } else {
                            if (randomNum < rockChance+1){
                                topLayer.SetTile(new Vector3Int(-x + mapWidth/2, -y + mapHeight/2, 0), rock);
                                rockPlacedNum++;
                            }
                        }
                    }
                }
                
            }
        }
    }

    private bool TileNearby(int x, int y, Tile tile, Tilemap layer){
        bool tileNearby = false;
        // TODO: Fix later, does not need to check every tile
        for(int a = x-1; a < x+1; a++){
            for(int b = y-1; b < y+1; b++){
                Tile tempTile = layer.GetTile<Tile>(new Vector3Int(-a + mapWidth/2, -b + mapHeight/2, 0));
                if (!(a == x && b == y)){
                    if (tempTile != null){
                        if (tempTile == tile){
                            tileNearby = true;
                            break;
                        }
                    }
                }
            }
        }
        return tileNearby;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
