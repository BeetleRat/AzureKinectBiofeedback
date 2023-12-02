using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveServo1 : MonoBehaviour
{
    public string showName = "BASE";                // debug
    public string use = "12";                       // debug
    public GameObject tower;                        // what is moved by 1st servomotor

    string msg = "Servo_1 moved to: 125 degree";    // raw message from arduino
    float rot;                                      // rotation value from servo on arduino
    float rotDelta;                                 // difference between rotation angle of arduino and model

    int startRotationAlignment = 100;               // static difference between start rotation angle of arduino and model
    int minRotationAlignment = 45;                  // static difference between minimal rotation angle of arduino and model
    int maxRotationAlignment = 155;                 // static difference between maximal rotation angle of arduino and model

    // Start is called before the first frame update
    void Start()
    {
        rot = tower.transform.eulerAngles.y;        //start rot value, before we get it from arduino
    }

    // Update is called once per frame
    void Update()
    {
        msg = NewRW.messageConnected;               // raw message from arduino connection script                   //uncomment after debug!!!

        if (NewRW.connected == true)                // means we connected to arduino                                //uncomment after debug!!!
        {
            if (msg.Contains("Servo_1"))            // if message from arduino for Servo_1
            {
                msg = msg.Substring(18, 3);         // loose everything not number
                rot = int.Parse(msg);               // parse rotation angle as number
            }
        }
        else                                        // means we NOT connected to arduino                            //uncomment after debug!!!
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))                                           // if we input "1"
            {
                if (rot + startRotationAlignment > minRotationAlignment) rot = rot - 5F;    // degree -5 until we reach limits
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))                                           // if we input "2"
            {
                if (rot + startRotationAlignment < maxRotationAlignment) rot = rot + 5F;    // degree +5 until we reach limits
            }
        }

        float angle = Mathf.MoveTowardsAngle(tower.transform.eulerAngles.y, rot, 25f * Time.deltaTime);
        tower.transform.eulerAngles = new Vector3(tower.transform.eulerAngles.x, tower.transform.eulerAngles.z, angle);
    }
}
