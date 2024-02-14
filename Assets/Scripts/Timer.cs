using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Text timerText;
    public float time;
    
    void Start() 
    {
        timerText = GetComponent<Text>();
        time = 0;
    }

    void Update() 
    {
        time = Time.time;
        timerText.text = "Time: " + time.ToString("0.00") + "s";
    }
}
