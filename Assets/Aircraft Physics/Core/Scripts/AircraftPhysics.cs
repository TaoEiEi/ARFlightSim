using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AircraftPhysics : MonoBehaviour
{
    const float PREDICTION_TIMESTEP_FRACTION = 0.5f;
    const float GRAVITY = 9.81f; // m/s²

    [SerializeField]
    float thrust = 0;
    [SerializeField]
    List<AeroSurface> aerodynamicSurfaces = null;
    [SerializeField]
    List<ControlSurface> controlSurfaces = null;

    Rigidbody rb;
    float thrustPercent;
    bool isBraking = false;
    BiVector3 currentForceAndTorque;
    Vector3 lastVelocity; // To store last frame velocity

    public void SetThrustPercent(float percent)
    {
        thrustPercent = Mathf.Clamp01(percent);
    }

    // New method to set control surfaces angles
    public void SetControlSurfecesAngles(float pitch, float roll, float yaw, float flap)
    {
        foreach (var controlSurface in controlSurfaces)
        {
            if (controlSurface.surface == null) continue;

            float angle = 0;
            switch (controlSurface.type)
            {
                case ControlSurfaceType.Pitch:
                    angle = pitch * controlSurface.flapAngle;
                    break;
                case ControlSurfaceType.Roll:
                    angle = roll * controlSurface.flapAngle;
                    break;
                case ControlSurfaceType.Yaw:
                    angle = yaw * controlSurface.flapAngle;
                    break;
                case ControlSurfaceType.Flap:
                    angle = flap * controlSurface.flapAngle;
                    break;
            }
            controlSurface.surface.SetFlapAngle(angle);
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        BiVector3 forceAndTorqueThisFrame = CalculateAerodynamicForces(
            rb.velocity, rb.angularVelocity, Vector3.zero, 1.2f, rb.worldCenterOfMass);

        Vector3 velocityPrediction = PredictVelocity(forceAndTorqueThisFrame.p);
        Vector3 angularVelocityPrediction = PredictAngularVelocity(forceAndTorqueThisFrame.q);

        BiVector3 forceAndTorquePrediction = CalculateAerodynamicForces(
            velocityPrediction, angularVelocityPrediction, Vector3.zero, 1.2f, rb.worldCenterOfMass);

        currentForceAndTorque = (forceAndTorqueThisFrame + forceAndTorquePrediction) * 0.5f;
        rb.AddForce(currentForceAndTorque.p);
        rb.AddTorque(currentForceAndTorque.q);

        // Add thrust force
        rb.AddForce(transform.forward * thrust * thrustPercent);

        // Apply braking by increasing drag
        rb.drag = isBraking ? 2f : 0.2f; // Increase drag to simulate braking

        // Calculate G-force
        Vector3 gForce = CalculateGForce();
        // You can now use the gForce vector for further processing or display
        Debug.Log("G-Force: " + gForce);
    }

    private BiVector3 CalculateAerodynamicForces(Vector3 velocity, Vector3 angularVelocity, Vector3 wind, float airDensity, Vector3 centerOfMass)
    {
        BiVector3 forceAndTorque = new BiVector3();
        foreach (var surface in aerodynamicSurfaces)
        {
            Vector3 relativePosition = surface.transform.position - centerOfMass;
            forceAndTorque += surface.CalculateForces(
                -velocity + wind - Vector3.Cross(angularVelocity, relativePosition),
                airDensity, relativePosition);
        }
        return forceAndTorque;
    }

    private Vector3 PredictVelocity(Vector3 force)
    {
        return rb.velocity + Time.fixedDeltaTime * PREDICTION_TIMESTEP_FRACTION * (force / rb.mass + Physics.gravity);
    }

    private Vector3 PredictAngularVelocity(Vector3 torque)
    {
        Quaternion inertiaTensorWorldRotation = rb.rotation * rb.inertiaTensorRotation;
        Vector3 torqueInDiagonalSpace = Quaternion.Inverse(inertiaTensorWorldRotation) * torque;
        Vector3 angularVelocityChangeInDiagonalSpace;
        angularVelocityChangeInDiagonalSpace.x = torqueInDiagonalSpace.x / rb.inertiaTensor.x;
        angularVelocityChangeInDiagonalSpace.y = torqueInDiagonalSpace.y / rb.inertiaTensor.y;
        angularVelocityChangeInDiagonalSpace.z = torqueInDiagonalSpace.z / rb.inertiaTensor.z;

        return rb.angularVelocity + Time.fixedDeltaTime * PREDICTION_TIMESTEP_FRACTION
            * (inertiaTensorWorldRotation * angularVelocityChangeInDiagonalSpace);
    }

    public void Brake(bool isBraking)
    {
        this.isBraking = isBraking;
    }

    private Vector3 CalculateGForce()
    {
        // Calculate the change in velocity
        Vector3 acceleration = (rb.velocity - lastVelocity) / Time.fixedDeltaTime;

        // Update last velocity for the next frame
        lastVelocity = rb.velocity;

        // Convert acceleration to G-force
        Vector3 gForce = acceleration / GRAVITY;

        // Return the G-force vector
        return gForce;
    }

#if UNITY_EDITOR
    public void CalculateCenterOfLift(out Vector3 center, out Vector3 force, Vector3 displayAirVelocity, float displayAirDensity, float pitch, float yaw, float roll, float flap)
    {
        Vector3 com;
        BiVector3 forceAndTorque;
        if (aerodynamicSurfaces == null)
        {
            center = Vector3.zero;
            force = Vector3.zero;
            return;
        }

        if (rb == null)
        {
            com = GetComponent<Rigidbody>().worldCenterOfMass;
            foreach (var surface in aerodynamicSurfaces)
            {
                if (surface.Config != null)
                    surface.Initialize();
            }
            SetControlSurfecesAngles(pitch, roll, yaw, flap);
            forceAndTorque = CalculateAerodynamicForces(-displayAirVelocity, Vector3.zero, Vector3.zero, displayAirDensity, com);
        }
        else
        {
            com = rb.worldCenterOfMass;
            forceAndTorque = currentForceAndTorque;
        }

        force = forceAndTorque.p;
        center = com + Vector3.Cross(forceAndTorque.p, forceAndTorque.q) / forceAndTorque.p.sqrMagnitude;
    }
#endif
}

[System.Serializable]
public class ControlSurface
{
    public AeroSurface surface;   // พื้นผิวควบคุมที่ใช้สำหรับการควบคุมการบิน
    public float flapAngle;       // มุมของแผงควบคุม
    public ControlSurfaceType type; // ประเภทของแผงควบคุม
}

public enum ControlSurfaceType
{
    Pitch,  // ควบคุมการก้มและเงย
    Yaw,    // ควบคุมการหันซ้ายและขวา
    Roll,   // ควบคุมการกลิ้ง
    Flap    // ควบคุมแผงปรับองศา
}
