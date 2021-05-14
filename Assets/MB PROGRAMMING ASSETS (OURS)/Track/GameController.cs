using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameDone gameFinished;

    [NonSerialized]
    public float timeRemaining;
    [NonSerialized]
    public bool timerIsRunning = true;

    [SerializeField]
    private GateController gateController;
    [SerializeField]
    private FlyingUpdated drone;
    [SerializeField]
    private Text time;
    [SerializeField]
    private Transform StartPos;

    [SerializeField]
    [Range(1,10)]
    private int laps = 1;
    private int labsBack;

    // Start is called before the first frame update
    void Start()
    {
        gameFinished = new GameDone();
        labsBack = laps;

        gateController.courseDone.AddListener(lap);
        StartPos = drone.transform;

        time.transform.parent.gameObject.SetActive(true); //Enabler kun i den her scene
    }

    public void restart() //Load den første scene
    {
        drone.TurnedOff = false;
        labsBack = laps;
        time.text = "00:00";
    }

    private void lap()
    {
        if (--labsBack <= 0)
        {
            drone.TurnedOff = true;
            gameFinished.Invoke(timeRemaining);

            Transform droneTransform = drone.gameObject.transform;

            droneTransform.position = StartPos.position;
            droneTransform.rotation = StartPos.rotation;

            Rigidbody rb = drone.gameObject.GetComponent<Rigidbody>();

            rb.angularVelocity = Vector3.zero;
            rb.velocity = Vector3.zero;


            timeRemaining = 0;
            timerIsRunning = false;
            Debug.Log("Game is done!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (timerIsRunning)
        {
            timeRemaining += Time.deltaTime;
            updateTime();
        }
    }

    void updateTime()
    {
        float minutes = Mathf.FloorToInt(timeRemaining / 60);
        float seconds = Mathf.FloorToInt(timeRemaining % 60);
        float milliSeconds = (timeRemaining % 1) * 1000;
        time.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliSeconds);
    }
}

public class GameDone : UnityEvent<float>
{

}
