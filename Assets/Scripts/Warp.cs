using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// teleports within a scene
public class Warp : MonoBehaviour
{
    public GameObject player;
    public Transform destination;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player"); // the player
        destination = transform.GetChild(0); // transform of the (only) child object
    }

    // Update is called once per frame
    void Update()
    {
        if ((transform.position - player.transform.position).magnitude < 1) {
            // Debug.Log("warped");
            // warp to the child object
            player.transform.position = destination.position;
        }
    }
}
