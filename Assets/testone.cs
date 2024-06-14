using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testone : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Log a message when the game starts
        Debug.Log("Game Started. Ready to read joystick input.");
    }

    // Update is called once per frame
    void Update()
    {
        // Read the horizontal and vertical axes from the joystick
        float horizontal = Input.GetAxisRaw("HorizontalJoy");
        float vertical = -Input.GetAxisRaw("VerticalJoy"); // Invert vertical axis

        // Log the axis values to the console
        Debug.Log("Horizontal Axis: " + horizontal);
        Debug.Log("Vertical Axis: " + vertical);
    }
}