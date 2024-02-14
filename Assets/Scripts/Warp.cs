using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Warp : MonoBehaviour
{
    public GameObject player;
    public Transform destination; // should be from an empty game object
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        destination = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        if ((transform.position - player.transform.position).magnitude < 1) {
            Debug.Log("warped");
            player.transform.position = destination.position;
        }
    }
}
