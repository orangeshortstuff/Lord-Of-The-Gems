using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalableUI : MonoBehaviour
{
    public float aspectRatio, pixelsPerScaleUnit;
    float baseScale; // y or z scale on rect transform
    public enum StretchMode { Horizontal, Vertical }
    public StretchMode ResizeMode;
    // Start is called before the first frame update
    void Start() {
        RectTransform rt = GetComponent<RectTransform>();
        float oldScale = rt.localScale.y;
        float xPerScale = rt.position.x / rt.localScale.x;
        float yPerScale = rt.position.y / rt.localScale.y;
        if (ResizeMode == StretchMode.Horizontal) {
            baseScale = Screen.width / pixelsPerScaleUnit;
        } else if (ResizeMode == StretchMode.Vertical) {
            baseScale = Screen.height / pixelsPerScaleUnit;
        }
        
        rt.localScale = new Vector3(baseScale * aspectRatio, baseScale, baseScale);
        float scaleRatio = baseScale / oldScale;
        rt.anchoredPosition *= scaleRatio;
    }
}
