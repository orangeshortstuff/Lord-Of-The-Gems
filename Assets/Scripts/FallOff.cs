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

    // Start is called before the first frame update
    void Start()
    {
        spawn = gameObject.transform.position;
        rb = GetComponent<Rigidbody>();
        sm = GameObject.Find("score").GetComponent<ScoreManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.transform.position.y < respawnHeight) {
            GameObject gemList = GameObject.Find("gems");
            // find all gems, including inactive ones
            Transform[] gems = gemList.GetComponentsInChildren<Transform>(true);
            //Debug.Log(gems.Length);
            
            // take only inactive ones
            List<Transform> inactiveGems = new List<Transform>();
            foreach (Transform g in gems) {
                if (g.gameObject.activeInHierarchy != true) {
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
                inactiveGems[i].gameObject.SetActive(true); // and re-activate the nearest gems
            }

            // respawn the player, at last
            gameObject.transform.position = spawn;
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }
    }
}
