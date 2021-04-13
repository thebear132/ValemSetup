using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandInputManagement : MonoBehaviour
{
    private InputDevice RControllerDevice;
    private InputDevice LControllerDevice;


    private struct ControllerValuesStruct
    {
        public Vector2 Primary2DAxis;
        public bool PrimaryButton;
        public bool SecondaryButton;


        public float Trigger;
        public float Grip;
        public bool TriggerButton;

    }


    static ControllerValuesStruct RControllerValues;
    static ControllerValuesStruct LControllerValues;


    void Start()
    {
        List<InputDevice> devices = new List<InputDevice>(); //Få alle devices knyttet til Oculus'en
        foreach (var device in devices)
        {
            Debug.Log(device.name + " | " + device.characteristics);
        }

        InputDeviceCharacteristics RControllerCharac = InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller; //Få fat i den højre controller.
        InputDevices.GetDevicesWithCharacteristics(RControllerCharac, devices); //Opdater devices til kun at have den højre controller
        if (devices.Count > 0)
            RControllerDevice = devices[0]; //Lav et nyt objekt med den højre controller

        InputDeviceCharacteristics LControllerCharac = InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller; //Få fat i den VENSTRE controller.
        InputDevices.GetDevicesWithCharacteristics(LControllerCharac, devices);
        if (devices.Count > 0)
            LControllerDevice = devices[0];

    }


    void Update()
    {
        //Få thumbstickens værdier som en 2D vector
        RControllerDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out RControllerValues.Primary2DAxis); //TryGetFeatureValue(Knappen den skal finde, variablen den skal skrive til)
        LControllerDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out LControllerValues.Primary2DAxis);

        //Få primary button bool værdi //A
        RControllerDevice.TryGetFeatureValue(CommonUsages.primaryButton, out LControllerValues.PrimaryButton);
        LControllerDevice.TryGetFeatureValue(CommonUsages.primaryButton, out LControllerValues.PrimaryButton);

        //Få secondary button bool værdi //B
        RControllerDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out LControllerValues.SecondaryButton);
        LControllerDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out LControllerValues.SecondaryButton);


        //OUTPUT DEBUG DELEN
        Debug.Log("2DAxis Right: " + RControllerValues.Primary2DAxis.x + "|" + RControllerValues.Primary2DAxis.y);
        Debug.Log("2DAxis Left: " + LControllerValues.Primary2DAxis.x + "|" + LControllerValues.Primary2DAxis.y);
        Debug.Log("Right Primary = " + RControllerValues.PrimaryButton);
        Debug.Log("Left Primary = " + LControllerValues.PrimaryButton);


    }
}
