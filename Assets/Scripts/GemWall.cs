using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemDoor : MonoBehaviour
{
    public ScoreManager sm;
    public int gemRequirement;
    public GameObject player;
    PickupMessage message;
    // Start is called before the first frame update
    void Start()
    {
        sm = GameObject.Find("score").GetComponent<ScoreManager>();
        message = GameObject.Find("powerAlert").GetComponent<PickupMessage>();
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(sm.score >= gemRequirement) {
            message.SetMessage("A gem door has been unlocked.", 5);
            gameObject.SetActive(false); // disable the running of all scripts, renderers, colliders, etc.
        }
        if (Vector3.Distance(transform.position, player.transform.position) < 10f) {
            message.SetMessage("Collect all the gems first!", 2);
        }
    }
}
