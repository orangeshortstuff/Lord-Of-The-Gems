using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Text timerText;
    public float time;
    float startTime;
    public float timeLimit;
    WinManager wm;
    public float penalties;
    
    void Start() 
    {
        timerText = GetComponent<Text>();
        wm = GameObject.Find("winner").GetComponent<WinManager>();
        time = 0;
        timeLimit = 360;
        startTime = Time.time;
        penalties = 0;
    }

    void Update() 
    {
        if (wm.won || wm.lost) {
            return;
        }
        time = Time.time - startTime + penalties;
        float timeLeft = timeLimit - time;
        if (timeLeft < 0) {
            wm.lost = true;
        }
        timerText.text = "Time left: " + timeLeft.ToString("0.00");
        //timerText.text = "Time left: " + timeLeft.ToString("0.00") + "s";
    }
}
