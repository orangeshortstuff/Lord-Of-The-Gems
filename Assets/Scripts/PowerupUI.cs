using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerupUI : MonoBehaviour
{
    public PowerupManager pm;
    public Text powerText;
    // Start is called before the first frame update
    void Start()
    {
        powerText = GetComponent<Text>();
        GameObject player = GameObject.FindWithTag("Player");
        pm = player.GetComponent<PowerupManager>();
    }

    // Update is called once per frame
    void Update()
    {
        powerText.text = "";
        if (pm.state == PowerupManager.PowerupState.Inactive) {
            powerText.text = "Press E to use";
        } else if (pm.state == PowerupManager.PowerupState.Active) {
            powerText.text = "Time left: " + pm.timeLeft.ToString("0.00") + "s";
        }
    }
}
