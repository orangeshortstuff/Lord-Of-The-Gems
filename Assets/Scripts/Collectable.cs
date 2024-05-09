using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public ScoreManager sm;
    public GameObject player;
    Renderer rend;
    AudioSource audio;
    // Start is called before the first frame update
    void Start()
    {
        sm = GameObject.Find("score").GetComponent<ScoreManager>();
        player = GameObject.FindWithTag("Player");
        rend = GetComponent<Renderer>();
        rend.enabled = true;
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((transform.position - player.transform.position).magnitude < 1 && rend.enabled) {
            //Debug.Log("pick up");
            //gameObject.SetActive(false);
            rend.enabled = false;
            sm.AddScore(1);
            // play sound effect with a random pitch
            audio.pitch = Random.Range(0.9f, 1.2f);
            audio.Play(0);
        }
    }
}
