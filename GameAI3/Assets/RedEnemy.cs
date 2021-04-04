using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedEnemy : MonoBehaviour
{
  private GameObject redEnemy;
    // Start is called before the first frame update
    void Start()
    {
      //find the spawnObjects
      redEnemy = GameObject.Find("RedEnemy");
      InvokeRepeating("DelayShow", 0 , 0.1f);
    }

    void Update()
    {
      //delay loading redEnemy
      new WaitForSeconds(1f);
      CancelInvoke("DelayShow");
    }

  void DelayShow() {
    if(redEnemy.activeSelf)
      redEnemy.SetActive(false);
    else
      redEnemy.SetActive(true);
  }
}
