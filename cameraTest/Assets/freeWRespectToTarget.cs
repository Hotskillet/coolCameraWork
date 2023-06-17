using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class freeWRespectToTarget : MonoBehaviour
{
    //first, establishing important things camera track player
    public Transform target;
    //the below is gonna determine how sensitive camera is to mouse
    public float rotationWeight;
    
    //the actual offset vectors
    //as in where the camera is in relation to the target
    private Vector3 rotationVector;
    private Vector3 offsetPosition;

    public float cameraDistanceStarting = 100.0f;
    public float cameraScrollStrength = 1.0f;
    private float cameraDistance;
    //definiting the mouse input tracker things
    private Vector2 mouseInputLast;
    private Vector2 mouseInputNew;

    //defiining the things that will tell where camera is (cumulative mouse movement)
    private float cumulativeH = 0.0f;
    private float cumulativeY = 0.0f;

    //same but in radians
    private float cumulativeHRadians = 0.0f;
    private float cumulativeYRadians = 0.0f;


    //this function is intended to convert degrees into radians. 
    //this is because the camera rotation and trig functions use radians and degrees respectively.
    //the function itself is pretty common sense tho.
    float convertToRadians(float deg) {
        return (deg * Mathf.PI) / 180;
    }

    //this function makes me feel better about clamping for the cumulative y
    //cheecks if greater than and less than max and min
    float clamp(float n, float min, float max) {
        if (n > max) {
            return max;
        } else if (n < min) {
            return min;
        } else {
            return n;
        }
    }

    //function for doing everything related to user input for the camera
    //includes: scrolling, clicking and dragging
    //figured out thanks to this graph i made (: 
    // https://www.geogebra.org/3d/gsxwja6v
    //i feel rlly proud of it
    void cameraControls() {
        //first thing we do is detect the new mouse position
        mouseInputNew = Input.mousePosition;

        //now we find the difference between new and last mouse position
        Vector2 mouseInputDifference = mouseInputNew - mouseInputLast;

        //now that we found the difference, we can just set the last input to be the new one.

        //NOTE TO SELF
        //IF WE MAKE IT SO THAT THE CURSOR LOCKS ON WHEN DRAGGING
        //WE NEED TO MOVE THIS TO AFTER THE IF STATEMENT
        mouseInputLast = mouseInputNew;

        //this makes it so that the thing only updates if you hold down right click
        if (Input.GetMouseButton(1)) {

            //separate the differences into their x and y counterparts.
            //a lot of the negative multiplication like with DiffY is to mirror the input.
            //scrolling against the grain feels better with dragging imo
            float diffX = mouseInputDifference.x;
            float diffY = -mouseInputDifference.y;

            //add the difference in x to the cumulative horizontal movement var
            cumulativeH += (diffX * rotationWeight);
            cumulativeHRadians = convertToRadians(cumulativeH);

            //do the same with y(if this isnt an if statement with stuff then do that)
            //use the clamp fucntion to feel better
            //as in feel better when forcing it to stay within a certain range
            float toBeY = cumulativeY + (diffY * rotationWeight); 
            cumulativeY = clamp(toBeY % 360, (-80), (80));
            



            //cumulativeY += (diffY * rotationWeight);
            cumulativeYRadians = convertToRadians(cumulativeY);
            
            //horizontal offsets
            float offsetX = Mathf.Sin(cumulativeHRadians);
            float offsetZ = Mathf.Cos(cumulativeHRadians);



            //the vertical one uses cumYRadians instead
            float offsetY = Mathf.Sin(cumulativeYRadians);
            //this is for the vertical scrolling part. since the camera would be moving in a sphere.
            float offsetYCompensation = Mathf.Cos(cumulativeYRadians);
            

            //put all of these together into one big vector3
            //theres some interesting math that goes on here
            //but for now just refer to the graph
            //basically the vertical stuff is going up in a circular path
            //and we multiply by the horizontal ness of the circular path which is that offsetYCompensation
            //really cool in my opinion
            Vector3 basicOffset = new Vector3(offsetX * offsetYCompensation, offsetY, offsetZ * offsetYCompensation);
            
            //now we adjust for camera distance and ta da
            offsetPosition = basicOffset * cameraDistance;
        

            //alrighty so we just did camera position and now lets figure out rotation. 
            //honestly, this is probably way easier loll

            //ive labeling them rotation H and rotation Y, but we need to remembere that rotation H goes on Y
            //and rotation Y goes on X
            //because they're about the axis
            //let me know if you think its more consistent to name this offsetRotation
            rotationVector = new Vector3(-cumulativeY + 180, cumulativeH, 180);
            
            //rotation was so much more nitpicky. at least is WAYY simpler. not a single lick of trig.
        }

        //temporary. will implement a more sophisticated scroll input detector at another time lol.
        //by more sophisticated, was thinking to have some kind of linear interpolation between scrolling
        //so scrolling away for the camera would be smoother
        //like roblox
        //in fact this whole thing was meant to look like roblox
        cameraDistance = clamp((cameraDistance - Input.mouseScrollDelta.y) * cameraScrollStrength, 0, 100);

    }

    // Start is called before the first frame update
    void Start() {

        //to keep it seamless
        mouseInputLast = Input.mousePosition;
        cameraDistance = cameraDistanceStarting;
        cameraControls();
    }

    // Update is called once per frame
    void Update() {
        cameraControls();

        transform.position = target.transform.position + offsetPosition;
        transform.rotation = Quaternion.Euler(rotationVector);
    }
}
