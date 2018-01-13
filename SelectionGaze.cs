using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;


namespace VRStandardAssets.Utils
{

    public class SelectionGaze : MonoBehaviour
    {

        public event Action OnGazeComplete;                                                   // This event is triggered when the bar has filled.


        [SerializeField]
        private float lookPeriod = 2f;                                      // How long it takes for the bar to fill.


        private Coroutine StareRoutine;                                                     // Used to start and stop the 'GazeSelecting' coroutine based on input.   
        private bool lookPeriodOver;                                                            // Used to allow the coroutine to wait for 'GazeSelecting'
        private float passedSelectionTime;


        // Use this for initialization
        void Start()
        {
            // at the start, no selection time has passed
            passedSelectionTime = 0f;
        }


        // Update is called once per frame
        void Update()
        {

        }



        private IEnumerator GazeSelect()
        {
            // At the start of the coroutine, GazeSelecting is not complete.
            lookPeriodOver = false;

            // Create a timer and reset passed selection time.
            float timer = 0f;
            passedSelectionTime = 0f;

            // This loop is executed once per frame until the timer exceeds the duration.
            while (timer < lookPeriod)
            {
                // The image's fill amount requires a value from 0 to 1 so we normalise the time.
                passedSelectionTime = timer / lookPeriod;

                // Increase the timer by the time between frames and wait for the next frame.
                timer += Time.deltaTime;
                yield return null;
            }

            // When the loop is finished set elapsed selection time to be full.
            passedSelectionTime = 1f;

            // set flag to indicate look period is over
            lookPeriodOver = true;

            // If there is anything subscribed to OnSelectionComplete call it.
            if (OnGazeComplete != null)
                OnGazeComplete();
        }

        public IEnumerator WaitForGazeSelecting()
        {
            //  GazeSelecting is not complete.
            lookPeriodOver = false;

            // Check every frame if the radial is filled.
            while (!lookPeriodOver)
            {
                yield return null;
            }
        }

        //method to do GazeSelecting ,start timer
        public void startGaze()
        {
            StareRoutine = StartCoroutine(GazeSelect());   //start filling
        }


        //method to both  hide and unfill radial at the same time
        public void endGaze()
        {
            StopCoroutine(StareRoutine); //stop the previous GazeSelecting event
            passedSelectionTime = 0f;    //reset elapsed selection time to zero        
        }

    }
}