using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueAgent : MonoBehaviour
{
    public Transform player;
    public float maxSpeed = 15f;
    private GameObject gameObj;
    private Rigidbody2D body;
    private FleeBehavior flee;

    // Start is called before the first frame update
    void Start()
    {
        gameObj = GameObject.Find("BlueAgent");
        body = GetComponent<Rigidbody2D>();
        flee = new FleeBehavior();
        float locX = Random.Range(-10f, 10f);
        float locY = Random.Range(-10f, 10f);
        gameObj.transform.position = new Vector2(locX, locY);
    }

    // Update is called once per frame
    void Update()
    {
        // finite state machine
        float distance = Vector2.Distance(transform.position, player.position); // should translate this to world tiles
        if (distance < 5)
        {
            body.velocity = flee.calculateMove(transform.position, player.position, body, maxSpeed);
        }
    }
}
