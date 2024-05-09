using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    public enum PowerupType { JumpBoost, SpeedBoost } // has to be the same as PowerupManager
    public float respawnTime, duration;
    float cooldownTimer;
    public PowerupType type;
    GameObject player;
    Renderer rend;
    CharacterControls cc;
    AudioSource audio;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        rend = GetComponent<Renderer>();
        rend.enabled = true;
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        PowerupManager pm = player.GetComponent<PowerupManager>();
        // only pick up if the player is near, it's not on cooldown, and the player doesn't have a powerup stored or active
        if ((transform.position - player.transform.position).magnitude < 1 && rend.enabled && pm.state == PowerupManager.PowerupState.None) {
            //Debug.Log("pick up");
            pm.type = (PowerupManager.PowerupType)type;
            pm.state = PowerupManager.PowerupState.Inactive; // has it, but not used yet
            pm.duration = duration;
            rend.enabled = false;
            cooldownTimer = respawnTime;
            audio.Play(0);
        }
        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer < 0) {
            rend.enabled = true;
        }
    }
}
