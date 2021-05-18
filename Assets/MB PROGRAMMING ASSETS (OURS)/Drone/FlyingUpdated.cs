using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class FlyingUpdated : MonoBehaviour
{
    [SerializeField]
    GameObject RightRotor;
    [SerializeField]
    GameObject LeftRotor;
    [SerializeField]
    GameObject VrRig;

    public float gravityForce = 9.82f;
    public float thrustForce = 20;
    public float pitchForce = 5;
    public float rollForce = 5;
    public float yawForce = 5;


    //PID gains
    [SerializeField]
    private Vector3 Propertional; //paramtre som angiver propertional parametret til de forskellige aksers af PID'erne
    [SerializeField]
    private Vector3 integral; //paramtre som angiver integraler parametret til de forskellige aksers af PID'erne
    [SerializeField]
    private Vector3 Derivative; //paramtre som angiver diffrentialet parametret til de forskellige aksers af PID'erne

    //PID ting
    private PID[] regulators = new PID[3]; //array med PID regulatore for hver akse
    float distToGround;
    public bool TurnedOff = false;


    //Rigidbody til dronen
    Rigidbody rb;
    void Start()
    {
        //får rigidbodyen af dronen
        rb = gameObject.GetComponent<Rigidbody>();

        //overføre PID værdierne til PID objekter
        regulators[0] = new PID(Propertional.x, integral.x, Derivative.x);
        regulators[1] = new PID(Propertional.y, integral.y, Derivative.y);
        regulators[2] = new PID(Propertional.z, integral.z, Derivative.z);

        //set hvor langt der skal være stil jorden ud fra hvor stor collideren på dronen er.
        distToGround = gameObject.GetComponent<Collider>().bounds.extents.y;
    }

    void Update()
    {
        if (ControllerManager.CheckHands() == true) //Kun virk hvis hænderne er loaded ind. //Her kan man skifte perspektiv til FPV
        {
            SwitchView();
        }

    }

    bool ABtnPressedLastFrame = false;
    int cameraPosition = 0; //0=seated, 1=FPV
    Vector3 seatedPosition;
    Quaternion seatedRotation;
    void SwitchView()
    {
        bool AButtonNow = ControllerManager.rightController.GetControllerInfo().inputValues.PrimaryButton;
        if (ABtnPressedLastFrame == false && AButtonNow == true)  //why "== true"  //Pressed A and didnt press it last frame
        {                                                       //SWITCH CAMERA POSITION
            Debug.Log("Switching camera position = " + cameraPosition);
            nextCameraPosition();
            switch (cameraPosition)
            {
                case 1: //Over til FPV
                    //Gem der hvor man sidder, til når man skal tilbage
                    seatedPosition = VrRig.transform.position;
                    seatedRotation = VrRig.transform.rotation;

                    //VrRig.transform.parent = gameObject.transform; //Virker måske idk
                    VrRig.transform.SetParent(this.transform, false); //Set VrRig parent til denne drone. False betyder IKKE behold sin verdensposition

                    Matrix4x4 m = Matrix4x4.Rotate(this.transform.rotation);

                    Vector3 myVec = new Vector3(0, -0.8f, 0.7f);
                    VrRig.transform.localPosition = m.MultiplyPoint3x4(myVec);


                    break;
                default: //Siddende ned
                    VrRig.transform.parent = null; //Ingen parent
                    VrRig.transform.position = seatedPosition; //Tilbage til der hvor man sad i starten
                    VrRig.transform.rotation = seatedRotation;
                    break;
            }

        }
        ABtnPressedLastFrame = AButtonNow;
    }

    void nextCameraPosition() { if (++cameraPosition > 1) cameraPosition = 0; }



    void FixedUpdate() //Skal være fixedupdate for at fysikken dur med 9,82 i tyngdekraft
    {
        if (ControllerManager.CheckHands() == true) //If hands are found, apply fly controls
        {
            ApplyFlyControls();
        }
        else
            Debug.Log("Hands not found yet, skipping FixedUpdate");

    }
    
    /// <summary>
    /// funktion som udregner alle aksernes resultater af PID resultater
    /// </summary>
    /// <param name="err"> det er en vector som beskriver hvor meget at fejl der er på akserne</param>
    /// <returns></returns>
    Vector3 calculateRotationCompensation(Vector3 err)
    {
        float x = regulators[0].calculate(err.x, Time.fixedDeltaTime);
        float y = regulators[1].calculate(err.y, Time.fixedDeltaTime);
        float z = regulators[2].calculate(err.z, Time.fixedDeltaTime);

        return new Vector3(x, y, z);
    }



    public Vector3 rotationTarget = Vector3.zero;
    /// <summary>
    /// funktion som tjekker om dronen har ramt jorden
    /// </summary>
    /// <returns></returns>
    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }
    /// <summary>
    /// funktion somholder holder tager controllernes input og ruglere dronen.
    /// </summary>
    void ApplyFlyControls() //Inputene fra controllerne bliver anvendt på dronen her
    {
        //få fat i controllers sticks
        HandInputManagement.ControllerClass.InputValues rightControllerValues = ControllerManager.rightController.GetControllerInfo().inputValues;
        HandInputManagement.ControllerClass.InputValues leftControllerValues = ControllerManager.leftController.GetControllerInfo().inputValues;

        //overfør sticksnes værdi til akserne.
        float thrust = leftControllerValues.Primary2DAxis.y * thrustForce;
        float yaw = leftControllerValues.Primary2DAxis.x;
        float pitch = -rightControllerValues.Primary2DAxis.y; //Var vendt om
        float roll = rightControllerValues.Primary2DAxis.x;

        //anti tyngdekraft så det er at dronen ikke falder.
        if (gravityForce == -1)
            gravityForce = -1 * Physics.gravity.y * rb.mass; //-1 * -9,82 * 1kg, hvis massen er 1kg
        //Debug.Log("Countergravity = " + gravityForce);
        rb.AddRelativeForce(new Vector3(0, gravityForce, 0)); //Counter tyngdekraften så den svæver selv

        //tjek om dronen slukket og den er på jorden for at sørge for at den ikke flyver når den ikke skal.
        if (TurnedOff) return;
        if (!IsGrounded())
        {
            //konverter dronens rotationsakser til unitys akser.
            Vector3 rotationTarget = (pitch * transform.right * pitchForce) + (yaw * transform.up * yawForce) + (roll * transform.forward * rollForce);
            Vector3 rotationError = rotationTarget - rb.angularVelocity; //udregen fejlen på hastigheden
            Vector3 compensation = calculateRotationCompensation(rotationError); //udregn PID værdierne

            rb.AddTorque(rotationError);// tilføj momenterne
        }
        else
        {
            rotationTarget = Vector3.zero;//reset rotations målet.
        }

        rb.

        rb.AddForce(transform.up * thrust); //tilføj thrusten.
    }



    public class PID
    {
        public float k_p, k_i, k_d;

        float collector = 0;
        float lastValue = 0;

        public PID(float propertional, float integral, float Derivative)
        {
            k_p = propertional;
            k_i = integral;
            k_d = Derivative;
        }
        public PID()
        {
        }

        /// <summary>
        /// udregn PID værdien
        /// </summary>
        /// <param name="err">fejlen som den skal regulere efter</param>
        /// <param name="timestep"> hvor lang tid siden sidste udregning</param>
        /// <returns></returns>
        public float calculate(float err, float timestep)
        {
            collector += k_i * err * timestep; //udfør integrallets step.
            float output = Mathf.Clamp(k_p * err + collector + k_d * (err - lastValue)/timestep, -3, 3); //udregn og begræns PID værdien
            lastValue = err; //gem værdie til næste udregning
            return output;
        }
    }



    static class ControllerManager //EN KLASSE FOR AT HOLDE STYR PÅ CONTROLLERNE. HØJRE OG VENSTRE CONTROLLER
    {
        //Array til controllerne
        public static HandInputManagement[] HandObj;
        public static bool hasFoundHands = false;

        //Referencer til controllerne
        public static HandInputManagement rightController; //Reference til scriptet på hånden til højre
        public static HandInputManagement leftController;  //Refernce til den til venstre


        public static bool CheckHands() //Ser om hænderne er der, og hvis de er, så hent dem ind som referencer i rightControllerOverview og leftControllerOverview
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
                        else if (HandObj[0].GetControllerInfo().name.ToLower().Contains("left"))
                        {
                            rightController = HandObj[1].GetComponent<HandInputManagement>();
                            leftController = HandObj[0].GetComponent<HandInputManagement>();
                        }
                        else
                            Debug.Log("pis");


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
    }
}
