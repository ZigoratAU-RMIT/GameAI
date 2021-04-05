using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedEnemy : MonoBehaviour
{
  public int delayShow = 150;
  public float blinking = 0.3f;
  private GameObject redEnemy;
  private int counter = 0;
    // Start is called before the first frame update
    void Start()
    {
      //find the spawnObjects
      redEnemy = GameObject.Find("RedEnemy");
      InvokeRepeating("DelayShow", 0 , blinking);
      float locX = Random.Range(-10f,10f);
      float locY = Random.Range(-10f,10f);
      redEnemy.transform.position = new Vector2(locX,locY);          
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
    
    void DelayShow() {
    if(redEnemy.activeSelf){
      redEnemy.SetActive(false);
    }
    else{
      redEnemy.SetActive(true);
    }
  }
}
