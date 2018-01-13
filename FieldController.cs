using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using VRStandardAssets.Common;
using VRStandardAssets.Utils;

/*  8/2017
 * By Emmanuel S Akosah, to allow fove gaze tracking.
 * Based on Shooter180 gamecontroller.
 * Added a reference to 'SelectionGaze' which handles fove gaze selecting events
 * Added a reference to 'SelectionGazeSlider' which allows slider to be filled by looking 
 * Game play changed to demonstrate selection by eye gaze
 * 
 * NOTE: don't check the camera attached to the 'Playground',
 * it causes unexpected behaviour
 */

namespace VRStandardAssets.ShootingGallery
{
    
    public class FieldController : MonoBehaviour
    {
        [SerializeField]        private SessionData.GameType m_GameType;       // Whether this is a 180 or 360 shooter.
        [SerializeField]        private BoxCollider m_FieldCollider;           //  the volume that the targets can spawn within.       
        [SerializeField]        private ObjectPool m_TargetObjectPool;         // The object pool that stores all the targets.
        [SerializeField]        private float m_GameLength = 60f;              // Time a game lasts in seconds.
        [SerializeField]        private float m_SpawnInterval = 3f;            // How frequently a target could spawn.
        [SerializeField]        private int desiredGrid;                       // number of grids in which targets are spawned
        [SerializeField]        private SelectionRadial m_SelectionRadial;     // reference to a selection radial script
        [SerializeField]        private TextController m_UIController;         // Used to encapsulate the UI.       
        [SerializeField]        private SelectionGazeSlider m_SelectionSlider; // Used to confirm the user has understood the intro UI. 
        [SerializeField]        private SelectionGaze m_SelectionGaze;         // This controls when the fove gaze selection is complete.
        [SerializeField]        private float m_EndDelay = 1.5f;               // The time the user needs to wait between the outro UI and being able to continue.
        [SerializeField]        private Reticle m_Reticle;                     // This is turned on and off when it is required and not.


        private Vector3[] balloonPositions;
        private int previousIndex = 99;  //to mark the start of getting indexes
        private float gridScale;
       // private Vector3 originalradialposition;
        public bool IsPlaying { get; private set; }                     // Whether or not the game is currently playing.
                                                                        // Use this for initialization
        private IEnumerator Start()
        {
            //make the field collider the parent of the selection radial
            m_SelectionRadial.transform.parent = m_FieldCollider.transform;
           
            //get the original position of the radial, where it will be returned after being moved around
           // originalradialposition = m_FieldCollider.bounds.center;
           // Debug.Log("originalradialposition z is : " + originalradialposition.z);
           // Debug.Log("originalradialposition y is : " + originalradialposition.y);
           // Debug.Log("originalradialposition x is : " + originalradialposition.x);


            //make the right number of grids, depending on desiredGrid
            balloonPositions = makeGrids(desiredGrid);          

            // Set the game type for the score to be recorded correctly.
            SessionData.SetGameType(m_GameType);

            // Continue looping through all the phases.
            while (true)
            {
                yield return StartCoroutine(StartPhase());
                yield return StartCoroutine(PlayPhase());
                yield return StartCoroutine(EndPhase());
            }
        }

        // Update is called once per frame
        void Update()
        {
        
        }

       
        private IEnumerator StartPhase()
        {
            // Wait for the intro UI to fade in.
            yield return StartCoroutine(m_UIController.ShowIntroUI());

          
            //hide the radial.       
            m_SelectionRadial.Hide();

            //enable selection slider if it's disabled
            if (m_SelectionSlider.enabled == false)
                m_SelectionSlider.enabled = true;

            //Wait for the selection slider to finish filling.
            yield return StartCoroutine(m_SelectionSlider.WaitForBarToFill());           

            // Wait for the intro UI to fade out.
            yield return StartCoroutine(m_UIController.HideIntroUI());
        }



        private IEnumerator PlayPhase()
        {           
            // The game is now playing.
            IsPlaying = true;
          
            // Reset the score.
            SessionData.Restart();

            // Wait for the play updates to finish.
            yield return StartCoroutine(gamePlay());
                      
            // The game is no longer playing.
            IsPlaying = false;
        }


        private IEnumerator EndPhase()
        {
            //replace selection radial to the center of the field
          //  m_SelectionRadial.moveToPosition(originalradialposition);

            // In order, wait for the outro UI to fade in then wait for an additional delay.
            yield return StartCoroutine(m_UIController.ShowOutroUI());
            yield return new WaitForSeconds(m_EndDelay);

            // Wait for the radial to fill (this will show and hide the radial automatically).
            yield return StartCoroutine(m_SelectionRadial.WaitForSelectionRadialToFill());

            // Wait for the outro UI to fade out.
            yield return StartCoroutine(m_UIController.HideOutroUI());
        }


        private IEnumerator gamePlay()                          //mod by ESA,
        {
            // Reset the score.
            SessionData.Restart();

            // The time remaining is the full length of the game length.
            float gameTimer = m_GameLength;

            // The amount of time before the next spawn is the full interval.
            float spawnTimer = m_SpawnInterval;

            // While there is still time remaining...
            while (gameTimer > 0f)
            {
                // ... check if the timer for spawning has reached zero.
                if (spawnTimer <= 0f)
                {
                    //  restart the timer for spawning.
                    spawnTimer = m_SpawnInterval;

                    // Spawn a target.
                    Spawn(gameTimer);
                }
                // Wait for the next frame.
                yield return null;

                //disappear after Timetodissappear

                // Decrease the timers by the time that was waited.
                gameTimer -= Time.deltaTime;
                spawnTimer -= Time.deltaTime;
            }  
        }


        private void Spawn(float timeRemaining)                          //mod by ESA,
        {
            // Get a reference to a target instance from the object pool.
            GameObject target = m_TargetObjectPool.GetGameObjectFromPool();

            //make the field collider the parent of the target
            target.transform.parent = m_FieldCollider.transform;

            Vector3 randomPosition = balloonPositions[getRandomIndex()];

            // Set the target's position to a random position. localPosition ensures relative positioning
            target.transform.localPosition = randomPosition;

            //scale the balloon box collider according to the grid size
            target.GetComponent<BoxCollider>().size = new Vector3(gridScale, gridScale, 0.1f);     //gridScale is determined inside makeGrids

            // Find a reference to the ShootingTarget script on the target gameobject and call it's Restart function.
            ShootingTarget shootingTarget = target.GetComponent<ShootingTarget>();
            shootingTarget.Restart(timeRemaining);

            //assign originalradialposition to the center of the target's collider, where the radial will be placed
           // shootingTarget.originalradialposition = originalradialposition;



                          //Radial stuff//
            //assign radialposition to the same place as target
           // shootingTarget.radialposition = randomPosition;

            //supply the target with a reference to the selection radial being used
            target.GetComponent<ShootingTarget>().m_SelectionRadial = m_SelectionRadial;

            //restart the fill on the selection radial
            target.GetComponent<ShootingTarget>().m_SelectionRadial.Unfill();


                         //Eye tracking stuff//
            //supply the target with a reference to the SelectionGaze script being used
            target.GetComponent<ShootingTarget>().m_SelectionGaze = m_SelectionGaze;
                       
            //reset the gazeselecting processs
            target.GetComponent<ShootingTarget>().m_SelectionGaze.resetGaze();
                                //End//

            // Subscribe to the OnRemove event.
            shootingTarget.OnRemove += HandleTargetRemoved;
        }


        //method to ensure targets are not spawned at the same position consecutively
        private int getRandomIndex()
        {
            if (previousIndex == 99)
              return (previousIndex = Random.Range(0, desiredGrid * desiredGrid));
            

            // get a random position in the grids
            int index = Random.Range(0, desiredGrid * desiredGrid);

            while (index == previousIndex)
            {
                // repeat, get a random position in the grids
                index = Random.Range(0, desiredGrid * desiredGrid);
            }

            previousIndex = index;
            return index;
        }


       //method to dynamically set the number of grids in which to show targets
       private Vector3[] makeGrids(int desiredGrid)                        // added by ESA
       {
            if (desiredGrid < 0)     //correct negative arguments
            {
                desiredGrid = desiredGrid * -1;
            } 

            if (desiredGrid < 2)     //set default number to 2
            {
                desiredGrid = 2;
            }

            Vector3[] spawnPositions;   //array of balloon positions to be returned
            int gridNum = desiredGrid;
            float gridDimx = 0, gridDimy = 0;   // the dimenions of a single grid

            // Find the centre and extents of the field collider, the designated play space.
            Vector3 center = m_FieldCollider.bounds.center;
            Vector3 extents = m_FieldCollider.bounds.extents;
           // Debug.Log("center is: " + center + "\n extents is: " + extents);

            // Get the lengths of each axis.           
            float distx = extents.x*2;
            float disty = extents.y*2;
           // Debug.Log("distx is: " + distx + "\n disty is: " + disty);            
           
            gridDimx = distx / gridNum;                   //a single grid has dimensions 1/gridNum'th of the dimensions of the collider
            gridDimy = disty / gridNum;
           // Debug.Log("gridDimx is: " + gridDimx + "\n gridDimy is: " + gridDimy);

            //begin from the top-left
            float y = center.y + extents.y;
            float x = center.x - extents.x;

            //initiallize array with corresponding number of grids
            spawnPositions = new Vector3[gridNum * gridNum];   //make 4 grids for TwoXTwo, 9 for ThreeXThree etc...

            //get all balloon positions
            //create balloon positions from left to right, top to down
            for (int i = 0; i < gridNum; i++)
            {
                y -= gridDimy;     //started from the top, so decrease in value
                for (int j = 0; j < gridNum; j++)
                {
                    x += gridDimx;     //started from the left, so increase in value

                    // get the center of grid, that's where the target should be spawned
                    float xpos = x - gridDimx / 2;
                    float ypos = y + gridDimy / 2;

                    //xpos and ypos are multiplied by gridDims for relative positioning
                    spawnPositions[j + (gridNum * i)] = new Vector3(xpos, ypos, center.z-1);                    
                }
                //reset x back to the left
                x = center.x - extents.x;
            }
       
            //set gridScale to be used to scale box collider of balloons/targets
            gridScale = gridDimx;

            return spawnPositions; 
        }
    

        private void HandleTargetRemoved(ShootingTarget target)
        {
            // Now that the event has been hit, unsubscribe from it.
            target.OnRemove -= HandleTargetRemoved;

            // Return the target to it's object pool.
            m_TargetObjectPool.ReturnGameObjectToPool(target.gameObject);
        }

    }
}
