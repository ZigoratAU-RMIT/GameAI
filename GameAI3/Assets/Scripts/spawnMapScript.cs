using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnMapScript : MonoBehaviour
{

    private int maxRows = 21;
    private int maxCols = 21;
    private SpriteRenderer sprite;
    // Start is called before the first frame update
    void Start()
    {
        spawnTiles();
        spawnObjects();
        spawnPlayer();

    }

    private void spawnPlayer()
    {
        GameObject player = Instantiate((GameObject)Resources.Load("Prefab/Player"));
        player.transform.position = new Vector3(0, 0, 0);
    }

    // spawns tiles in the background
    private void spawnTiles()
    {

        for (int row = -21; row < maxRows; row++)
        {
            for (int col = -21; col < maxCols; col++)
            {

                //creates a new tile game object
                GameObject tile = Instantiate((GameObject)Resources.Load("Prefab/Tile"));
                sprite = tile.GetComponent<SpriteRenderer>();

                // alternates the colours of the tiles to create a check board pattern
                if ((row % 2 == 0 && col % 2 != 0) || (row % 2 != 0 && col % 2 == 0))
                {
                    sprite.color = new Color(0.23f, 0, 0.1f, 1);
                }
                else
                {
                    sprite.color = new Color(0.3f, 0, 0.2f, 1);
                }

                //moves tile to row and col postion
                tile.transform.position = new Vector3(col, row, 1);

            }
        }
    }

    //spawns obsticales and enemies
    private void spawnObjects()
    {
        for (int row = -21; row < maxRows; row++)
        {
            for (int col = -21; col < maxCols; col++)
            {
                if (row == 20 || col == 20 || row == -21 || col == -21)
                {
                    GameObject tree = Instantiate((GameObject)Resources.Load("Prefab/Tree"));
                    tree.transform.position = new Vector3(col, row, 0);
                }
                else
                {
                    float placeObject = Random.Range(0, 100);
                    Debug.Log(placeObject);

                    // if the chance is less than 0.5 then spawn something
                    if (placeObject < 30)
                    {
                        // if the chance is less than 0.25 spawn an obstacle
                        if (placeObject < 10)
                        {
                            int obstacle = Random.Range(0, 4);

                            //spawn river tile
                            if (obstacle == 0)
                            {
                                GameObject river = Instantiate((GameObject)Resources.Load("Prefab/River"));
                                river.transform.position = new Vector3(col, row, 0);
                            }

                            //spawn stone tile
                            else if (obstacle == 1)
                            {
                                GameObject stone = Instantiate((GameObject)Resources.Load("Prefab/Stone"));
                                stone.transform.position = new Vector3(col, row, 0);
                            }

                            //spawn tree tile
                            else if (obstacle == 2)
                            {
                                GameObject tree = Instantiate((GameObject)Resources.Load("Prefab/Tree"));
                                tree.transform.position = new Vector3(col, row, 0);
                            }

                            //spawn brench tile
                            else if (obstacle == 3)
                            {
                                GameObject branch = Instantiate((GameObject)Resources.Load("Prefab/Branch"));
                                branch.transform.position = new Vector3(col, row, 0);
                            }

                        }
                        // spawn an enemy 
                        else if (placeObject < 15)
                        {
                            int enemyChance = Random.Range(0, 2);

                            if (enemyChance == 0)
                            {
                                GameObject redEnemy = Instantiate((GameObject)Resources.Load("Prefab/RedEnemy"));
                                redEnemy.transform.position = new Vector3(col, row, 0);
                            }
                            else
                            {
                                GameObject blueEnemy = Instantiate((GameObject)Resources.Load("Prefab/BlueEnemy"));
                                blueEnemy.transform.position = new Vector3(col, row, 0);
                            }



                        }


                    }
                }
            }


        }



    }

    // Update is called once per frame
    void Update()
    {

    }
}
