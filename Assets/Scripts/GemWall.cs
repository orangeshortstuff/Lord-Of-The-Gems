using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemDoor : MonoBehaviour
{
    public ScoreManager sm;
    public int gemRequirement;
    // Start is called before the first frame update
    void Start()
    {
        sm = GameObject.Find("score").GetComponent<ScoreManager>();
        gemRequirement = 100;
    }

    // Update is called once per frame
    void Update()
    {
        if(sm.score >= gemRequirement) {
            gameObject.SetActive(false);
        }
    }
}
