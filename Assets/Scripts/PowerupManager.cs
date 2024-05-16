using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupManager : MonoBehaviour
{
    public CharacterControls cc;
    public enum PowerupType { JumpBoost, SpeedBoost } // has to be the same as Powerup
    public PowerupType type;
    public float duration, timeLeft = 0;
    public enum PowerupState { None, Inactive, Active }
    public PowerupState state;
    public float heightMultiplier = 1, speedMultiplier = 1;
    public bool freeJumps = false, freeSprint = false;

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterControls>();
        state = PowerupState.None;
    }

    // Update is called once per frame
    void Update()
    {
        // reset to base values
        heightMultiplier = 1;
        speedMultiplier = 1;
        freeJumps = false;
        freeSprint = false;

        if (state == PowerupState.Active) { // if the powerup is in use, apply effects
            timeLeft -= Time.deltaTime; // and count down the timer
            if (type == PowerupType.JumpBoost) {
                heightMultiplier = 2;
                freeJumps = true;
            }
            else if (type == PowerupType.SpeedBoost) {
                speedMultiplier = 1.5f;
                freeSprint = true;
            }
        }
        if (timeLeft < 0) { // if the time runs out, you don't have a powerup any more
            timeLeft = 0;
            state = PowerupState.None;
        }
    }
}
