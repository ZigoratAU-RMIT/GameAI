using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedEnemyPursuitBehaviour : MonoBehaviour
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
        Vector3 direction = player.position - transform.position;
        direction.Normalize();
        movement = direction;
    }

    private void FixedUpdate()
    {
        moveEnemy(movement);
    }

    void moveEnemy(Vector2 direction)
    {
        Vector3 newPosition = Vector3.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
        rb.MovePosition(newPosition);
    }
}
