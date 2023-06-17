using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movementScript : MonoBehaviour {

    //first let's define what we can pick out
    //we pick out a target 
    //this is so that movement is relative to the camera
    public Transform target;

    //movespeed and jumpspeed should be self explanatory.
    //jump speed will be the velocity at the thing will go for a jump
    public float moveSpeed;
    public float jumpSpeed;


    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }
}
