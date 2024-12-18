using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FallOff : MonoBehaviour
{
    public float respawnHeight; // height at which we respawn the player
    public Vector3 spawn; // the starting position of the player
    private Rigidbody rb; // player's rigidbody
    public ScoreManager sm;
    public Timer timer;

    // Start is called before the first frame update
    void Start()
    {
        spawn = gameObject.transform.position;
        rb = GetComponent<Rigidbody>();
        sm = GameObject.Find("score").GetComponent<ScoreManager>();
        timer = GameObject.Find("timer").GetComponent<Timer>();

    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.transform.position.y < respawnHeight) {
            GameObject gemList = GameObject.Find("gems");
            // get all transforms in child objects, skipping the parent - find all gems
            Transform[] gems = gemList.GetComponentsInChildren<Transform>(true).Skip(1).ToArray();
            //Debug.Log(gems.Length);
            
            // take only inactive ones
            List<Transform> inactiveGems = new List<Transform>();
            foreach (Transform g in gems) {
                Renderer rend = g.gameObject.GetComponent<Renderer>();
                if (rend.enabled != true) {
                    inactiveGems.Add(g);
                }
            }
            //Debug.Log(inactiveGems.Count);

            // sort gem transforms by distance
            // https://forum.unity.com/threads/solved-sort-array-objects-by-distance.811056/
            inactiveGems = inactiveGems.OrderBy((g) => ( (g.position - transform.position).sqrMagnitude)).ToList();

            // get the number of gems to remove
            int maxGemsSpawned = Mathf.Min(10, Mathf.CeilToInt(inactiveGems.Count / 8.0f));
            sm.AddScore(-1 * maxGemsSpawned); // subtract from score
            for (int i = 0; i < maxGemsSpawned; i++) {
                inactiveGems[i].gameObject.GetComponent<Renderer>().enabled = true; // and re-activate the nearest gems
            }

            // respawn the player, at last
            gameObject.transform.position = spawn;
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            timer.penalties += 10; // and take 10 seconds off the timer
        }
    }
}
