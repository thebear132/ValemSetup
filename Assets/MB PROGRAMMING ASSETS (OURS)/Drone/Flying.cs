using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Flying : MonoBehaviour
{
    [SerializeField]
    GameObject RightRotor;
    [SerializeField]
    GameObject LeftRotor;

    [SerializeField]
    GameObject VrRig;

    public float gravityForce = 6.7f;
    public float thrustForce = 20;
    public float pitchForce = 5;
    public float rollForce = 5;
    public float yawForce = 5;

    //Error adjustment
    public float errorRollForce = 4;
    public float errorPitchForce = 4;
    public float errorYawForce = 4;

    HandInputManagement[] HandObj;
    bool hasFoundHands = false;

    HandInputManagement rightController; //Reference til scriptet på hånden til højre
    HandInputManagement leftController;  //Refernce til den til venstre


    Rigidbody rb;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }


    bool AButtonLastFrame = false;
    int cameraPosition = 0; //0=seated, 1=FPV
    void Update()
    {

        if (CheckHands() == true) //Only do if hands are 
        {
            bool AButtonNow = rightController.GetControllerInfo().inputValues.PrimaryButton;
            if (AButtonLastFrame == false && AButtonNow == true)    //Pressed A and didnt press it last frame
            {                                                       //SWITCH CAMERA POSITION
                Debug.Log("Switching camera position = " + cameraPosition);
                switch (cameraPosition)
                {
                    case 0:

                        cameraPosition++;
                        break;

                    case 1:
                        VrRig.transform.SetParent(this.transform, false);

                        VrRig.transform.localPosition = new Vector3(0, -0.8f, 0.7f);
                        VrRig.transform.Rotate(0, 180, 0);

                        cameraPosition--;
                        break;

                    default:
                        cameraPosition = 0;
                        break;
                }

            }
            AButtonLastFrame = AButtonNow;
        }

    }

    void FixedUpdate() //Skal være fixedupdate for at fysikken dur med 9,82 i tyngdekraft
    {
        if (CheckHands() == true) //If hands are found, apply fly controls
        {
            ApplyFlyControls();
        }
    }

    bool CheckHands() //Ser om hænderne er der, og hvis de er, så hent dem ind som referencer i rightControllerOverview og leftControllerOverview
    {
        if (hasFoundHands == true) //hasFoundHands er hvis hænderne allerede er blevet fundet tidligere
        {
            return true;
        }
        else //Ikke fundet hænderne endnu = PANIK. Prøv at find hænderne i runtime
        {
            HandObj = FindObjectsOfType<HandInputManagement>(); //Finder gameObjektet på de to hænder

            try
            {
                if (HandObj[0].GetControllerInfo().name.Equals(string.Empty)) //Hvis altså bare 1 af hænderne har et navn initialiseret, betyder det at de nu er i projektet
                {
                    Debug.Log("NO HANDS");
                    return false;
                }
                else
                {
                    Debug.Log("HANDS FOUND = " + HandObj[0].GetControllerInfo().name);  //Lige print til consollen så man kan se hvad navnet er, og om det er rigtigt
                    HandObj = FindObjectsOfType<HandInputManagement>();                 //Finder gameObjektet på de to hænder

                    if (HandObj[0].GetControllerInfo().name.ToLower().Contains("right")) //DENNE IF giver "rightController" den højre controller, og "leftController" den venstre
                    {
                        rightController = HandObj[0].GetComponent<HandInputManagement>();
                        leftController = HandObj[1].GetComponent<HandInputManagement>();
                    }
                    else
                    {
                        rightController = HandObj[1].GetComponent<HandInputManagement>();
                        leftController = HandObj[0].GetComponent<HandInputManagement>();
                    }
                    //Nu er hænderne initaliseret i rightController og leftController, og vi kan tilgå dem. Nice
                    hasFoundHands = true;//Nu er hænderne fundet
                }
            }
            catch
            {
                Debug.Log("NO HANDS (catched)");
            }
            return false;
        }
    }
    void ApplyFlyControls() //Inputene fra controllerne bliver anvendt på dronen her
    {
        HandInputManagement.ControllerClass.InputValues rightControllerValues = rightController.GetControllerInfo().inputValues;
        HandInputManagement.ControllerClass.InputValues leftControllerValues = leftController.GetControllerInfo().inputValues;

        float thrust = leftControllerValues.Primary2DAxis.y * thrustForce;
        float roll = leftControllerValues.Primary2DAxis.x * rollForce;
        float pitch = rightControllerValues.Primary2DAxis.y * pitchForce * -1; //Var vendt om
        float yaw = rightControllerValues.Primary2DAxis.x * yawForce;


        if (gravityForce == -1)
            gravityForce = -1 * Physics.gravity.y * rb.mass; //-1 * -9,82 * 1kg, hvis massen er 1kg
        //Debug.Log("Countergravity = " + gravityForce);
        rb.AddRelativeForce(new Vector3(0, gravityForce, 0)); //Counter tyngdekraften så den svæver selv


        //BUDGET PID REGULERINGS SYSTEM
        //Autoadjust roll
        Vector3 rollVec = new Vector3(0, rb.angularVelocity.y, 0);
        rb.AddRelativeTorque(-rollVec * errorRollForce);
        //Debug.Log("Roll rotation = " + rollVec);

        //Autoadjust Pitch
        Vector3 pitchVec = new Vector3(rb.rotation.z, 0, 0);
        rb.AddRelativeTorque(pitchVec * errorPitchForce);
        //Debug.Log("Pitch rotation = " + rb.rotation.z);

        //Autoadjust Yaw
        Vector3 yawVec = new Vector3(0, 0, rb.rotation.x);
        rb.AddRelativeTorque(-yawVec * errorYawForce);
        //Debug.Log("Yaw rotation = " + rb.rotation.x);



        //ADD INPUT forces til dronen
        Vector3 Torque = new Vector3(pitch, roll, yaw);
        rb.AddRelativeTorque(Torque);                   //Roteringer i alle retninger
        rb.AddRelativeForce(new Vector3(0, thrust, 0)); //Relativt til dronen //GODT!
        //Debug.Log("pitch=" + pitch + ". roll=" + roll + ". yaw=" + yaw + ". thrust=" + thrust);

    }
}