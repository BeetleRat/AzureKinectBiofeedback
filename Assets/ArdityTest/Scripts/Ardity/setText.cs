using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class setText : MonoBehaviour
{
    public GameObject GOText1;
    public GameObject GOText2;
    public string msgCon;
    public int msgNum;

    TextMeshPro Text1;
    TextMeshPro Text2;


    // Update is called once per frame
    void Update()
    {
        msgCon = NewRW.messageConnected;
        msgNum = GetPulse.msgnumber;
        GOText1.GetComponent<TMPro.TextMeshProUGUI>().SetText(NewRW.messageConnected);
        GOText2.GetComponent<TMPro.TextMeshProUGUI>().SetText(GetPulse.msgnumber.ToString());
    }
}
