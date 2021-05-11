using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;

public class HandInputManagement : MonoBehaviour
{
    public InputDeviceCharacteristics controllerCharacteristics;    //De spefikke karakteristikker der er ved vores controller. F.eks. left, right, controller osv
    public List<GameObject> ControllerPrefabsList;                  //Liste over alle controllerprefabs //Mappen Vr Controllers Models

    private InputDevice controllerDevice;                           //Vores kontroller er en input device. Altså en fysiske
    private GameObject spawnedController;                           //Ikke så vigtig. Men GameObject over controlleren inde i spillet som vi spawner


    public class ControllerClass //En klasse som holder styr på information om vores fysiske kontrollere
    {
        public string name;
        public InputDeviceCharacteristics characteristics;

        public InputValues inputValues;

        public struct InputValues
        {
            //Bare lige de vigtigste (der er flere)
            public Vector2 Primary2DAxis;
            public bool PrimaryButton;
            public bool SecondaryButton;

            public float Trigger;
            public float Grip;
            public bool TriggerButton;
        }
    }
    ControllerClass ControllerOverview = new ControllerClass(); //Objektet for vores fysiske kontroller



    void Start()
    {
        List<InputDevice> devices = new List<InputDevice>();                            //En InputDevice er f.eks. en controller, kamera, bodytracker osv
        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);
        //Få fat i alle devices, men kun dem som matcher vores controllerCharacteristics. F.eks. right, controller, left. De devices som den finder bliver smidt ind i listen "devices"

        foreach (var device in devices)
        {
            Debug.Log(device.name + " ---> " + device.characteristics); //Bare lige skriv dem alle ud
        }
        //Debug.Log("Total of " + devices.Count + " devices");


        if (devices.Count > 0) //HVIS den rent faktisk har fundet nogle devices, så fortsæt
        {
            controllerDevice = devices[0]; //Det er næsten umuligt at der er mere end 1 objekt i devices.
            GameObject controllerPrefab = ControllerPrefabsList.Find(controller => controller.name == controllerDevice.name); //Find den controller som passer til i vores ControllerPrefabsList, og gem den

            if (controllerPrefab) //Så hvis der er en model der passer, fortsæt
            {
                spawnedController = Instantiate(controllerPrefab, transform); //Så spawner man så den passende controller
            }
            else
            {
                Debug.LogError("Did not a corresponding controller model");
                spawnedController = Instantiate(ControllerPrefabsList[0], transform); //Hvis ikke man finder en passende controller, så bare spawn den første
            }
        } else
        {
            Debug.Log("No devices found");
        }

        //Opdater Controlleroverview med dens navn og katakteristisk.
        ControllerOverview.name = controllerDevice.name;
        ControllerOverview.characteristics = controllerDevice.characteristics;

    }


    void Update()
    {


        ////Det her var bare lige noget testing. Gør dig nok søsyg ahaha
        //GameObject VrRig = GameObject.Find("VR Rig");
        //VrRig.transform.position = new Vector3(GetControllerInfo().inputValues.Primary2DAxis.x, 0, GetControllerInfo().inputValues.Primary2DAxis.y);

    }


    public ControllerClass GetControllerInfo() //Kald denne metode for at få kontrollerens information tilbage i form af klassen ControllerClass (navn, karakteristikke, og alle inputværdierne)
    {
        //LÆS ->          TryGetFeatureValue(Knappen, variablen den skal skrive resultatet til)
        controllerDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out ControllerOverview.inputValues.Primary2DAxis);      //Få thumbstickens værdier som en 2D vector
        controllerDevice.TryGetFeatureValue(CommonUsages.primaryButton, out ControllerOverview.inputValues.PrimaryButton);      //Få primary button bool værdi //A
        controllerDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out ControllerOverview.inputValues.SecondaryButton);  //Få secondary button bool værdi //B
        controllerDevice.TryGetFeatureValue(CommonUsages.grip, out ControllerOverview.inputValues.Grip);
        controllerDevice.TryGetFeatureValue(CommonUsages.trigger, out ControllerOverview.inputValues.Trigger);

        return ControllerOverview;
    }
}
