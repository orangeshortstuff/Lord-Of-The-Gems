using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public int scene;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log((transform.position - player.transform.position).magnitude);
        if ((transform.position - player.transform.position).magnitude < 1.5f) {
            Debug.Log("load scene");
            SceneManager.LoadScene(scene);
        }
    }
}
