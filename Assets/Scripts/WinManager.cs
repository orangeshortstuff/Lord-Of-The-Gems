using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinManager : MonoBehaviour
{
    public bool won;
    public bool lost;
    public Text winText;
    // Start is called before the first frame update
    void Start()
    {
        winText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        winText.text = "";
        if (won) {
            winText.text = "You Win!";
            // in future, maybe call for name entry for leaderboard?
        } else if (lost) {
            winText.text = "You Lost";
        }
    }
}
