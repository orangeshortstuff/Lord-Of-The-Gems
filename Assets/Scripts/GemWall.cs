using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemDoor : MonoBehaviour
{
    public ScoreManager sm;
    public int gemRequirement;
    PickupMessage message;
    // Start is called before the first frame update
    void Start()
    {
        sm = GameObject.Find("score").GetComponent<ScoreManager>();
        message = GameObject.Find("powerAlert").GetComponent<PickupMessage>();
    }

    // Update is called once per frame
    void Update()
    {
        if(sm.score >= gemRequirement) {
            message.SetMessage("A gem door has been unlocked.", 5);
            gameObject.SetActive(false); // disable the running of all scripts, renderers, colliders, etc.
        }
    }
}
