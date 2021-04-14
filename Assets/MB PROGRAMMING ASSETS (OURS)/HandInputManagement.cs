using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;

public class HandInputManagement : MonoBehaviour
{
    public InputDeviceCharacteristics controllerCharacteristics;
    public List<GameObject> ControllerPrefabsList; //Liste over alle controllerprefabs //Mappen Vr Controllers Models

    private InputDevice controllerDevice;
    private GameObject spawnedController;

    private struct ControllerValuesStruct
    { //Bare lige de vigtigste
        public Vector2 Primary2DAxis;
        public bool PrimaryButton;
        public bool SecondaryButton;

        public float Trigger;
        public float Grip;
        public bool TriggerButton;
    }
    static ControllerValuesStruct ControllerValues; //Et struct til hver controller


    void Start()
    {
        List<InputDevice> devices = new List<InputDevice>(); //Få alle devices knyttet til Oculus'en

        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices); //Opdater devices til kun at have den højre controller

        //Debug.Log("Printing all devices attached");
        foreach (var device in devices)
        {
            Debug.Log(device.name + " ---> " + device.characteristics); //Bare lige skriv dem alle ud
        }
        //Debug.Log("Total of " + devices.Count + " devices");


        if (devices.Count > 0) //Hvis den ikke kan finde nogle, så bare lav den til den første device på listen
        {
            controllerDevice = devices[0];
            GameObject controllerPrefab = ControllerPrefabsList.Find(controller => controller.name == controllerDevice.name);
            if (controllerPrefab)
            {
                spawnedController = Instantiate(controllerPrefab, transform);
            }
            else
            {
                Debug.LogError("Did not a corresponding controller model");
                spawnedController = Instantiate(ControllerPrefabsList[0], transform);
            }
        }

    }


    void Update()
    {
        //Få thumbstickens værdier som en 2D vector
        controllerDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out ControllerValues.Primary2DAxis); //TryGetFeatureValue(Knappen den skal finde, variablen den skal skrive resultatet til)

        //Få primary button bool værdi //A
        controllerDevice.TryGetFeatureValue(CommonUsages.primaryButton, out ControllerValues.PrimaryButton);

        //Få secondary button bool værdi //B
        controllerDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out ControllerValues.SecondaryButton);


        //OUTPUT DEBUG DELEN
        Debug.Log(controllerDevice.name);
        Debug.Log("2DAxis : " + ControllerValues.Primary2DAxis.x + "|" + ControllerValues.Primary2DAxis.y);
        Debug.Log("Primary = " + ControllerValues.PrimaryButton);

    }
}
