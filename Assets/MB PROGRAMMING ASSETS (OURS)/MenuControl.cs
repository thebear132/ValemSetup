using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; //TextMeshPro UI text set gennemsigteligheden

public class MenuControl : MonoBehaviour
{
    public GameObject[] BlinkText;
    public float ShowDuration = 1.5f;
    public float HideDuration = 0.5f;


    float timeCounter = 0;
    bool isVisible = true;



    HandInputManagement[] HandObj;
    bool hasFoundHands = false;

    HandInputManagement rightController;
    HandInputManagement leftController;

    bool PrimaryButtonLastFrame = false;

    void Update()
    {
        RegulateBlinking();


        if (CheckHands()) //If the hands are loaded in the scene, continue
        {
            if(rightController.GetControllerInfo().inputValues.PrimaryButton == true && PrimaryButtonLastFrame == false) //Pressed A and didnt press it last frame
            {
                //Change scenes
                SceneManager.LoadScene(1); //Load spillet
            }
            PrimaryButtonLastFrame = rightController.GetControllerInfo().inputValues.PrimaryButton;
        }
    }

    void RegulateBlinking()
    {
        timeCounter += Time.deltaTime;

        if (isVisible)
        {
            if (timeCounter >= ShowDuration)
            {
                timeCounter = 0;
                isVisible = false;
                foreach (var targetText in BlinkText)
                {
                    Color aColor = targetText.GetComponent<TextMeshProUGUI>().color;
                    aColor.a = 0;
                    targetText.GetComponent<TextMeshProUGUI>().color = aColor;
                }
            }
        }
        else
        {
            if (timeCounter >= HideDuration)
            {
                timeCounter = 0;
                isVisible = true;
                foreach (var targetText in BlinkText)
                {
                    Color aColor = targetText.GetComponent<TextMeshProUGUI>().color;
                    aColor.a = 255;
                    targetText.GetComponent<TextMeshProUGUI>().color = aColor;
                }
            }
        }
    }



    bool CheckHands() //Denne metode checker om hænderne er slået til endnu
    {
        if (hasFoundHands == true)
        {
            return true;
        }
        else
        {
            HandObj = FindObjectsOfType<HandInputManagement>();
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
            return false;
        }
    }

    
}
