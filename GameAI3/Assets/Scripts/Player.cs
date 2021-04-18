using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour{

    private GameManager gm;

    List<WorldTile> movementPoints = new List<WorldTile>();
    List<WorldTile> path = new List<WorldTile>(); 
    List<WorldTile> smoothedPath = new List<WorldTile>();
    
    Vector2 target;
    Vector2 desiredVelocity;
    Vector2 steering;

    public LayerMask layer;

    private Rigidbody2D body;
    public float speed = 50;

    private int index = 0;
    private bool movementDone = true;

    private Pathfinding pf;

    public List<Dwarf> followers = new List<Dwarf>(){};

    public AudioClip deathSound;
    float dstToTarget;

    void Start(){
        body = GetComponent<Rigidbody2D>();
        pf = GetComponent<Pathfinding>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update(){
        if(Input.GetButtonDown("Fire1")){
            //Checking to see if input is valid
            WorldTile checkTile = pf.map.GetWorldTileByCellPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if(checkTile == null)
                return;

            //Make sure our movement is all reset
            movementPoints.Clear();
            index = 0;
            movementDone = false;

            //Finding the path - this trims the whole pathfinding algorithm down to nodes
            //These nodes are based on when the direction the entity must go changes
            path = pf.FindPathFromWorldPos(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));

            for(int i = 0; i < path.Count; i++){          
                if(i + 1 <= path.Count - 1 && path[i].direction != path[i + 1].direction){
                    movementPoints.Add(path[i]);
                }
            }
            //Add in last position
            movementPoints.Add(path[path.Count - 1]);


            Vector2 startPos = new Vector2(movementPoints[0].cellX + 0.5f, movementPoints[0].cellY + 0.5f);
            smoothedPath.Clear();
            smoothedPath.Add(movementPoints[0]);
            bool smoothing = true;
            int currentTargetNode = 1; //starting at second point the path because the first is always valid
            int emergencyBreak = 0;
            while(smoothing){
                if(emergencyBreak > 30)
                    smoothing = false;

                for(int i = currentTargetNode; i < movementPoints.Count; i++){ 
                    Vector2 targetPos = new Vector2(movementPoints[i].cellX + 0.5f, movementPoints[i].cellY + 0.5f);
                    Vector2 targetDir = (targetPos - startPos).normalized;
                    float targetDst = Vector2.Distance(targetPos, startPos); 
                    RaycastHit2D hit = Physics2D.BoxCast(startPos, new Vector2(0.75f,0.75f), 0, targetDir, targetDst, layer);
                    if(hit.collider != null){
                        smoothedPath.Add(movementPoints[i - 1]);
                        startPos = new Vector2(movementPoints[i - 1].cellX + 0.5f, movementPoints[i - 1].cellY + 0.5f);
                        currentTargetNode = i - 1;
                        continue;
                    }
                }
                if(currentTargetNode + 1 >= movementPoints.Count - 1)
                    smoothing = false; 

                emergencyBreak++; 
            }
            smoothedPath.Add(movementPoints[movementPoints.Count - 1]);
            target = new Vector2(smoothedPath[index].cellX + 0.5f, smoothedPath[index].cellY + 0.5f);
        }
    }

    void FixedUpdate(){
        if(movementDone == true)
            return;
        
        /*
        if(body.velocity == Vector2.zero){
            Vector2 oldTarget = new Vector2(movementPoints[movementPoints.Count - 1].cellX, movementPoints[movementPoints.Count - 1].cellY);
            path = pf.FindPathFromWorldPos(transform.position, oldTarget);
            movementPoints.Clear();
            index = 0;
            for(int i = 0; i < path.Count; i++){
                if(i + 1 <= path.Count - 1 && path[i].direction != path[i + 1].direction){
                    movementPoints.Add(path[i]);
                }
            }

            movementPoints.Add(path[path.Count - 1]);

            target = new Vector2(movementPoints[index].cellX + 0.5f, movementPoints[index].cellY + 0.5f);
        }
        */

        desiredVelocity = target - (Vector2)transform.position;
        desiredVelocity = desiredVelocity.normalized * speed;

        steering = desiredVelocity - body.velocity;
        steering = Vector2.ClampMagnitude(steering, speed);
        steering /= 15f;
         
        body.velocity = Vector2.ClampMagnitude(body.velocity + steering, speed);
        transform.up = body.velocity.normalized;

        if(Vector3.Distance(transform.position, new Vector3(smoothedPath[index].cellX + 0.5f, smoothedPath[index].cellY + 0.5f)) < 1f){
            index++;
            if(index == smoothedPath.Count){
                movementDone = true;
                body.velocity = Vector2.zero;
                index = 0;
                return;
            }
            target = new Vector2(smoothedPath[index].cellX + 0.5f, smoothedPath[index].cellY + 0.5f);
        }

        Debug.DrawRay(transform.position, body.velocity.normalized * 2, Color.green);
        Debug.DrawRay(transform.position, desiredVelocity.normalized * 2, Color.magenta);
        for(int i = 0; i < movementPoints.Count - 1; i++){
            Debug.DrawLine(new Vector2(movementPoints[i].cellX + 0.5f, movementPoints[i].cellY + 0.5f), new Vector2(movementPoints[i + 1].cellX + 0.5f, movementPoints[i + 1].cellY + 0.5f));
        }
        Debug.DrawLine(transform.position, target, Color.blue);
        for(int i = 0; i < smoothedPath.Count - 1; i++){
            Debug.DrawLine(new Vector2(smoothedPath[i].cellX + 0.5f, smoothedPath[i].cellY + 0.5f), new Vector2(smoothedPath[i + 1].cellX + 0.5f, smoothedPath[i + 1].cellY + 0.5f), Color.red);
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.GetComponent<Goblin>() != null)
        {
            gm.MinusPlayerScore();
            if(followers.Count > 0){
                int index = followers.Count-1;
                Dwarf removeDwarf = followers[index];
                followers.RemoveAt(index);
                removeDwarf.flockAgent.RemoveFromFlock();
                AudioSource.PlayClipAtPoint(deathSound, removeDwarf.transform.position, 1f);
                Destroy(removeDwarf.gameObject);
            }
        }

        if (col.gameObject.GetComponent<Dwarf>() != null){
            if (!followers.Contains(col.gameObject.GetComponent<Dwarf>())){ // follow
                gm.AddPlayerScore();
                followers.Add(col.gameObject.GetComponent<Dwarf>());
            }
        }
    }

}