using UnityEngine;
using System;

[ExecuteInEditMode]
public class Telemetry : MonoBehaviour
{

    string apiMode = "api";  //constant to identify the package
    public string game = "Project Cars 2";  //constant to identify the game
    public string vehicle = "Lamborghini Huracan";  //constant to identify the vehicle
    public string location = "Circuit Gilles-Villeneuve";  //constant to identify the location
    uint apiVersion = 102;  //constant of the current version of the api

    //gets the vehicle body to send speed to SRS
	public Rigidbody vehicleBody;

    void Start ()
    {
        vehicleBody = GetComponent<Rigidbody> ();
    }

    // Update is called once per frame
    void Update()
    {

        SimRacingStudio.SimRacingStudio_SendTelemetry(apiMode.PadRight(50).ToCharArray()
                                                     , apiVersion
                                                     , game.PadRight(50).ToCharArray()
                                                     , vehicle.PadRight(50).ToCharArray()
                                                     , location.PadRight(50).ToCharArray()
                                                     , Convert.ToSingle(vehicleBody.velocity.magnitude * 3.6)
                                                     , 7000
                                                     , 8000
                                                     , -1
                                                     , vehicleBody.rotation.eulerAngles.x
                                                     , vehicleBody.rotation.eulerAngles.y
                                                     , vehicleBody.rotation.eulerAngles.z
                                                     , 0
                                                     , 0
                                                     , 0
                                                     , 0
                                                     , 0
                                                     , 0
                                                     , 0
                                                     , 0
                                                     , 0
                                                     , 0
                                                     , 0
                                                     , 0);
    }
}
