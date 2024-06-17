using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class AirplaneController1 : MonoBehaviour
{
    [SerializeField]
    float rollControlSensitivity = 0.2f;
    [SerializeField]
    float pitchControlSensitivity = 0.2f;
    [SerializeField]
    float yawControlSensitivity = 0.2f;
    [SerializeField]
    float thrustControlSensitivity = 0.01f;
    [SerializeField]
    float flapControlSensitivity = 0.15f;

    float pitch;
    float yaw;
    float roll;
    float flap;

    float thrustPercent;
    bool brake = false;
    private bool dontChange= false;
    AircraftPhysics aircraftPhysics;
    Rotator propeller;

    private void Start()
    {
        aircraftPhysics = GetComponent<AircraftPhysics>();
        propeller = FindObjectOfType<Rotator>();
        SetThrust(0);
    }
    public float x;
    public  float y;

    public float _GetValueY;
    public float _GetValueX;

    public _ControllerType _Type;
    public _ControllerTypeX _Typex;
    public enum _ControllerType
    {
        CENTER,
        UP,
        DOWN
    }
    public enum _ControllerTypeX
    {
        CENTERX,
        Left,
        Right
    }
    private void Update()
    { 
        x = Input.GetAxis("HorizontalJoy");
        
        Debug.Log("x" + x);
       
        y = Input.GetAxis("VerticalJoy");

        if (y > 0) // 
        {
            if (_Type == _ControllerType.CENTER)
            {
                _Type = _ControllerType.UP;
              
            }
        }
        else if (y == 0)
        {
            _Type = _ControllerType.CENTER;
        }
        if (y < 0) // 
        {
            if (_Type == _ControllerType.CENTER)
            {
                _Type = _ControllerType.DOWN;
                
               
            }
        }

        if (_Type == _ControllerType.UP)
        {
            y = Math.Abs(Input.GetAxis("VerticalJoy"));
        }
        if (x > 0) // 
        {
            if (_Typex == _ControllerTypeX.CENTERX)
            {
                _Typex = _ControllerTypeX.Right;
              
            }
        }
        else if (x == 0)
        {
            _Typex = _ControllerTypeX.CENTERX;
        }
        if (x < 0) // 
        {
            if (_Typex == _ControllerTypeX.CENTERX)
            {
                _Typex = _ControllerTypeX.Left;
                
               
            }
        }
        if (_Typex == _ControllerTypeX.Left)
        {
            x = -Math.Abs(Input.GetAxis("HorizontalJoy"));
        }
        
        
        
        
        
       

        
        
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            SetThrust(thrustPercent + thrustControlSensitivity);
        }
        propeller.speed = thrustPercent * 1500f;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            thrustControlSensitivity *= -1;
            flapControlSensitivity *= -1;
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            brake = !brake;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            flap += flapControlSensitivity;
            //clamp
            flap = Mathf.Clamp(flap, 0f, Mathf.Deg2Rad * 40);
        }
        
        // Assign control inputs to pitch and yaw
        pitch = y * pitchControlSensitivity;
        roll = rollControlSensitivity * x;
       // Debug.Log("roll = " + roll + " Pitch = " + pitch);
    }

    private void SetThrust(float percent)
    {
        thrustPercent = Mathf.Clamp01(percent);
    }

    private void FixedUpdate()
    {
        aircraftPhysics.SetControlSurfecesAngles(pitch, roll, yaw, flap);
        aircraftPhysics.SetThrustPercent(thrustPercent);
        aircraftPhysics.Brake(brake);
    }
    public void OnButtonPressed()
    {
        SetThrust(thrustPercent + thrustControlSensitivity);
    }
}
