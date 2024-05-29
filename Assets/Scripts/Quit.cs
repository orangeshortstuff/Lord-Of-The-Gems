using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quit : MonoBehaviour
{
    void Start() {
        Cursor.lockState = CursorLockMode.None; // free the cursor
        Cursor.visible = true; // and make it visible
    }
    public void OnClick()
    {
        Application.Quit();
    }
}