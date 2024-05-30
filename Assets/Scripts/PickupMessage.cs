using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickupMessage : MonoBehaviour
{
    public Text text;
    Outline outline;
    float duration, timer;
    // Start is called before the first frame update
    void Start() {
        text = GetComponent<Text>();
        outline = GetComponent<Outline>();
        duration = 0.001f;
        text.text = "";
    }

    // Update is called once per frame
    void Update() {
        timer += Time.deltaTime;
        float alpha = Mathf.Min(1,((duration - timer) / duration) * 2); // hold at max opacity for half the time, then fade out
        text.color = new Color(1f,1f,1f, alpha); // set the colour to white, with the current opacity / alpha
        outline.effectColor = new Color(0f, 0f, 0f, alpha * 0.735f); // multiply the text alpha by the base outline alpha
        if (timer > duration) {
            text.text = "";
        }
    }

    public void SetMessage(string message, float duration) {
        text.text = message;
        this.duration = duration;
        this.timer = 0;
    }
}
