using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public int scene;
    public int requirement = 0;
    public GameObject player;
    ScoreManager scoreManager;
    PickupMessage message;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        scoreManager = GameObject.Find("score").GetComponent<ScoreManager>();
        message = GameObject.Find("powerAlert").GetComponent<PickupMessage>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((transform.position - player.transform.position).magnitude < 1.5f) {
            if (scoreManager.score >= requirement) {
                Debug.Log("load scene");
                SceneManager.LoadScene(scene);
            } else {
                message.SetMessage("To pass through, you need " + requirement + " gems!",2);
            }
        }
    }
}
