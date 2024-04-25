using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunWithRecharge : MonoBehaviour
{
	public float radius = 5f;
    public float depth = 20f;
	public float stunTime = 0.5f;
    public float rechargeTime = 20f;
    public float timePenalty = 5f;
    public Transform playerTransform;
    public Timer timer;
    float cooldown;

    private void Start() {
        playerTransform = GameObject.FindWithTag("Player").transform;
        timer = GameObject.Find("timer").GetComponent<Timer>();
        cooldown = rechargeTime; // skip wait for first recharge
    }

    void Update() {
        Vector2 xzPos = new Vector2(transform.position.x, transform.position.z);
        Vector2 playerXZ = new Vector2(playerTransform.position.x, playerTransform.position.z);
        float xzDist = Vector2.Distance(xzPos, playerXZ);
        float yDist = (transform.position.y - playerTransform.position.y);
        if (xzDist <= radius && yDist > 0 && yDist <= depth && cooldown > rechargeTime) { // if player enters the cylinder
			playerTransform.gameObject.GetComponent<CharacterControls>().HitPlayer(Vector3.zero, stunTime); // stun the player
            timer.penalties += timePenalty;
            cooldown = 0; // reset the cooldown
		}
        cooldown += Time.deltaTime;
    }
}