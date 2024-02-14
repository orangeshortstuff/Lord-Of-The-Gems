using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// got sick of having to deal with weird texture scaling, so this scales at
public class TextureScaler : MonoBehaviour
{
    Renderer rend;
    // use xz for floors, xy or zy for walls depending on which side you scale 
    // (rule of thumb, pick the two "large" sides)
    public enum modes {XY, XZ, ZY}
    
    public float xFactor; // repeat texture after this many units on first axis
    public float yFactor; // likewise for second axis
    public modes mode;
    Transform transform;

    void Start()
    {
        rend = GetComponent<Renderer>();
        transform = GetComponent<Transform>();
        SetTextureScale();
        
    }

    void Update()
    {
        // uncomment the call to get live updates
        // SetTextureScale();
    }

    void SetTextureScale() {
        Vector3 scale = transform.localScale;
        if (this.mode == modes.XY) {
            rend.material.mainTextureScale = new Vector2(scale.x / xFactor, scale.y / yFactor);
        } else if (this.mode == modes.XZ) {
            rend.material.mainTextureScale = new Vector2(scale.x / xFactor, scale.z / yFactor);
        } else if (this.mode == modes.ZY) {
            rend.material.mainTextureScale = new Vector2(scale.z / xFactor, scale.y / yFactor);
        }
    }
}
