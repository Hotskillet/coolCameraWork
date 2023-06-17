using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movementScript : MonoBehaviour {

    //first let's define what we can pick out
    //we pick out a target 
    //this is so that movement is relative to the camera
    public Transform target;
    private Rigidbody rb;


    //movespeed and jumpspeed should be self explanatory.
    //jump speed will be the velocity at the thing will go for a jump
    public float moveSpeed;
    public float jumpSpeed;
    public float rotationSpeed = 5.0f;


    //this has a very specific purpose
    //to rotate a vector3 about the y axis
    //we assume the vector3 is rooted at the origin
    Vector3 rotateVectorAboutY(Vector3 thang, float rads) {
        float a = thang.x;
        float b = thang.z;
        return new Vector3((Mathf.Cos(rads) * a) - (Mathf.Sin(rads) * b), thang.y, (Mathf.Sin(rads) * a) + (Mathf.Cos(rads) * b));
    }

    //formula for converting degrees to radians
    //so taht rotateVectorAboutY is easier to use
    float convertToRadians(float deg) {
        return (deg * Mathf.PI) / 180;
    }

    // Start is called before the first frame update
    void Start() {

        //assign the rigidbody to our guy
        //or our guy to the rigidbody im not quite sure
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update() {

        //this is the base movement vector
        //we will add all the transforms to it
        Vector3 baseGoVector = new Vector3(0.0f, 0.0f, 0.0f);

        //define the difference between this and the target (meant to be camera)
        Vector3 targetDiff = (transform.position - target.transform.position);

        //then we take out the vertical difference so its a flat difference
        //then we normalize it because the camera can scroll (varying target to mover distance)
        Vector3 forward = Vector3.Normalize(Vector3.Scale(targetDiff, new Vector3(1.0f, 0.0f, 1.0f)));

        //now wwe get the left and right vectors
        //honestly we only need rght just like how we only need forward
        //at least in terms of what we're doing for controls
        Vector3 left = rotateVectorAboutY(forward, convertToRadians(90));
        Vector3 right = rotateVectorAboutY(forward, convertToRadians(-90));


        //detect key inputs. wasd and arrows respectively.
        //we add the corresponding direction vectors to the baseGoVector
        //that way opposing keys would cancel out like w and s
        //also allows diagonal travel
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            baseGoVector += forward;
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            baseGoVector -= forward;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            baseGoVector += left;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            baseGoVector += right;
        }

        //so now we have baseGoVector
        //to account for game stuff we're gonna multiply it by speed and time difference
        Vector3 goVector = (baseGoVector * moveSpeed * Time.deltaTime);

        //update player position with the vector
        transform.position += goVector;



        //next order of business is rotation

        //only rotate if the player is actually moving
        if (baseGoVector != new Vector3(0.0f, 0.0f, 0.0f)) {

            //honestly this stuff feels weird to do
            //so we account rotation speed for time
            float rotationStep = rotationSpeed * Time.deltaTime;


            //lerp returns the third valueth place between first value and second value
            //use that to make rotation target
            //then we make our guy look to there
            Vector3 rotationTarget = Vector3.Lerp(transform.forward, baseGoVector, 0.8f);
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, rotationTarget, rotationStep, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
            
        }



        //oh ya also jumping 
        //when the space bar goes down
        if (Input.GetKeyDown("space")) {

            //create a new vector with only a vertical 
            Vector3 newJump = new Vector3(0.0f, jumpSpeed, 0.0f);
            
            rb.AddForce(newJump, ForceMode.Impulse);
        }
    }
}
