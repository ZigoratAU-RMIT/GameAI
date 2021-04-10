using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueEnemyPursuitBehaviour : MonoBehaviour
{
    public Transform player;
    private Rigidbody2D rb;
    private Vector2 movement;
    public float moveSpeed = 2f;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance < 5)
        {
            StartChase();
        }
        else
        {
            StopChase();
        }
    }


    void StartChase()
    {
        if (transform.position.x < player.position.x)
        {
            rb.velocity = new Vector2(moveSpeed, 0);
        }
        else
        {
            rb.velocity = new Vector2(-moveSpeed, 0);
        }
    }

    void StopChase()
    {
        rb.velocity = new Vector2(0, 0);
    }

}
