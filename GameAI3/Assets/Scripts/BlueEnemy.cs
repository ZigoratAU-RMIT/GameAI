using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueEnemy : MonoBehaviour
{
    public float blinking = 0.4f;
    public int delayShow = 100;
    private GameObject blueEnemy;
    private int counter = 0;
    // Start is called before the first frame update
    void Start()
    {
        //find the spawnObjects
        blueEnemy = GameObject.Find("BlueEnemy");
        InvokeRepeating("DelayShow", 0, blinking);
        float locX = Random.Range(-10f, 10f);
        float locY = Random.Range(-10f, 10f);
        blueEnemy.transform.position = new Vector2(locX, locY);
    }

    // Update is called once per frame
    void Update()
    {
        counter++;
        if (counter == delayShow)
        {
            CancelInvoke("DelayShow");
        }
    }

    void DelayShow()
    {
        if (blueEnemy.activeSelf)
        {
            blueEnemy.SetActive(false);
        }
        else
        {
            blueEnemy.SetActive(true);
        }
    }
}
