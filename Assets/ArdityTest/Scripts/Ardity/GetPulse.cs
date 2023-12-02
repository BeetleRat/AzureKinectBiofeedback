using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPulse : MonoBehaviour
{
    string msg;
    string msgtrim;
    public static int msgnumber;
    

    // Update is called once per frame
    void Update()
    {
        msg = NewRW.messageConnected;
        if (msg.Contains("pulse"))
        {
            msgtrim = msg.Replace("pulse=", "");
            msgnumber = int.Parse(msgtrim);
        }
    }
}