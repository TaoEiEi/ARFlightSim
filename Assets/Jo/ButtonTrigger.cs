using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class ButtonTrigger : MonoBehaviour
{
    [SerializeField] public UnityEvent onButtonPressed;
    private bool pressed = false;

    // Define the tag that the trigger will respond to
    [SerializeField] private string targetTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            pressed = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            pressed = false;
        }
    }

    private void Update()
    {
        if (pressed)
        {
            onButtonPressed?.Invoke();
        }
    }
}