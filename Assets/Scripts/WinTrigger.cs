using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    public GameObject player;
    public WinManager wm;
    Vector3 thisXZ, playerXZ;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        wm = GameObject.Find("winner").GetComponent<WinManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(wm.won || wm.lost) {
            return;
        }
        if ((player.transform.position - transform.position).magnitude < 1.5f ) {
            wm.won = true;
        }
    }
}
