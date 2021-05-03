using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Flying : MonoBehaviour
{
    public float thrustAdjust = 20;
    public float pitchAdjust = 5;
    public float rollAdjust = 5;
    public float yawAdjust = 5;

    HandInputManagement[] HandObj;
    bool hasFoundHands = false;

    HandInputManagement rightController; //Reference til scriptet på hånden til højre
    HandInputManagement leftController;  //Refernce til den til venstre


    Rigidbody rb;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        checkHands();

    }

    void checkHands() //Ser om hænderne er der, og hvis de er, så hent dem ind i rightControllerOverview og leftControllerOverview
    {
        if (hasFoundHands == true) //Found = bare fortsæt hænderne er der
        {
            Controls();
        }
        else //Ikke fundet hænderne endnu = PANIK. Prøv at find hænderne igen så
        {
            HandObj = FindObjectsOfType<HandInputManagement>(); //Finder gameObjektet på de to hænder

            try
            {
                var thing = HandObj[0].GetControllerInfo();
                if (thing.name.Equals(string.Empty))
                {
                    Debug.Log("NO HANDS");
                }
                else
                {
                    Debug.Log("HANDS FOUND = " + thing.name);
                    hasFoundHands = true;

                    HandObj = FindObjectsOfType<HandInputManagement>(); //Finder gameObjektet på de to hænder

                    if (HandObj[0].GetControllerInfo().name.ToLower().Contains("right"))
                    {
                        rightController = HandObj[0].GetComponent<HandInputManagement>();
                        leftController = HandObj[1].GetComponent<HandInputManagement>();
                    }
                    else
                    {
                        rightController = HandObj[1].GetComponent<HandInputManagement>();
                        leftController = HandObj[0].GetComponent<HandInputManagement>();
                    }
                }
            }
            catch
            {
                Debug.Log("NO HANDS (catched)");
            }
        }
    }
    void Controls() //Inputene fra controllerne bliver applied her
    {
        HandInputManagement.ControllerClass.InputValues rightControllerValues = rightController.GetControllerInfo().inputValues;
        HandInputManagement.ControllerClass.InputValues leftControllerValues = leftController.GetControllerInfo().inputValues;

        
        float thrust = leftControllerValues.Primary2DAxis.y * thrustAdjust;
        float roll = leftControllerValues.Primary2DAxis.x * rollAdjust;

        float pitch = rightControllerValues.Primary2DAxis.y * pitchAdjust;
        float yaw = -1 * rightControllerValues.Primary2DAxis.x * yawAdjust;


        Debug.Log("Thrust=" + thrust);
        if(thrust < 0.1 && thrust > -0.1) //Hvis man ikke thruster
        {
            rb.AddRelativeForce(new Vector3(0, 7f, 0)); //Så counter tyngdekraft. INGEN IDE HVORFOR 7???

        }



        Vector3 Torque = new Vector3(pitch, roll, yaw);
        rb.AddRelativeTorque(Torque);
        rb.AddRelativeForce(new Vector3(0, thrust, 0)); //Relativt til dronen //GODT!

        //Debug.Log("pitch=" + pitch + ". roll=" + roll + ". yaw=" + yaw + ". thrust=" + thrust);

    }
}