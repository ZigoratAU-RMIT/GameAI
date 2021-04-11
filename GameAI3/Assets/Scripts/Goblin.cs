using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : MonoBehaviour{
    //Define states
    private enum States{
        wander,
        seek,
        arrive,
        flee
    }
    
    int state = (int)States.wander;

    void FixedUpdate(){
        switch(state){
            case (int)States.wander:
                break;
            case (int)States.seek:
                break;
            case (int)States.arrive:
                break;
            case (int)States.flee:
                break;
            default:
                state = (int)States.wander;
                break;
            }
    }
}
