using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRStandardAssets.Utils;
using UnityEngine.UI;


    // This script is for loading scenes from the main menu.
    // Each 'button' will be a rendering showing the scene
    // that will be loaded and use the SelectionRadial.
    public class QuadBehaviour_qg : MonoBehaviour
    {


        [SerializeField]        private VRInteractiveItem m_InteractiveItem;       // The interactive item for where the user should click to load the level.      
        [SerializeField]        private Text itemText;                             // text if this item       
        [SerializeField]        private SelectionRadial m_SelectionRadial;         // This controls when the radial selection is complete.
        [SerializeField]        private PlayControl_qg m_GameController;

        //TODO change rendering to allow game to load and show images

        //  [SerializeField] private MeshRenderer m_ScreenMesh;             // The mesh renderer who's texture will be changed.
        //  [SerializeField] private Texture[] m_AnimTextures;              // The textures that will be looped through.

        public string textbk;         // to be set by the controller
        private bool m_GazeOver;                                            // Whether the user is looking at the VRInteractiveItem currently.       
        private Color originalItemColor;


        // Use this for initialization
        void Start()
        {

            originalItemColor = itemText.color;
            textbk = itemText.text;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void Awake()
        {

        }


        private void OnEnable()
        {
            m_InteractiveItem.OnOver += HandleOver;
            m_InteractiveItem.OnOut += HandleOut;
            m_SelectionRadial.OnSelectionComplete += HandleSelectionComplete;
        }


        private void OnDisable()
        {
            m_InteractiveItem.OnOver -= HandleOver;
            m_InteractiveItem.OnOut -= HandleOut;
            m_SelectionRadial.OnSelectionComplete -= HandleSelectionComplete;
        }


        private void HandleOver()
        {
            // When the user looks at number,

            Debug.Log("Show over state");
            itemText.color = new Color(0, 0, 0);  //change color to black
            m_SelectionRadial.showAndFill();  // show and fill radial
                                              //handel fove gaze too.
            m_GazeOver = true;
        }


        private void HandleOut()
        {
            // When the user looks at number,

            Debug.Log("Show out state");
            itemText.color = originalItemColor;

            m_SelectionRadial.hideAndUnFill();  // hide and unfill radial
            m_GazeOver = false;
        }


        private void HandleSelectionComplete()
        {
            // If the user is looking at the number when the radial's selection finishes, do something.
            if (m_GazeOver)
            {

                //send message to the game controller
                //controller takes care of when to use, and what to do with data

                // m_GameController.userSelections[m_GameController.index] = itemText.text;               

                if (m_GameController.index < 4)
                {
                    if (!m_GameController.done_selecting)  //dont send further input after fourth slot is filled
                    {
                        if (m_GameController.index == 3)
                        {
                            m_GameController.done_selecting = true;
                        }

                        m_GameController.gmDisplay.text += textbk;
                        Debug.Log("Sent number with sendSelection : " + textbk);
                        Debug.Log("Index is " + m_GameController.index);

                        m_GameController.userSelections[m_GameController.index] = textbk;
                        Debug.Log("UserSelection at this index is : " + m_GameController.userSelections[m_GameController.index]);

                        if (m_GameController.index < 3)
                        {
                            m_GameController.index++;
                        }


                    }
                }

                Debug.Log("Selection complete");

            }
        }

        //send selection to game controller
        private void sendSelection()
        {
            if (m_GazeOver)
            {

                //send message to the game controller
                //controller takes care of when to use, and what to do with data

                // m_GameController.userSelections[m_GameController.index] = itemText.text;               

                if (m_GameController.index < 4)
                {
                    if (!m_GameController.done_selecting)  //dont send further input after fourth slot is filled
                    {
                        if (m_GameController.index == 3)
                        {
                            m_GameController.done_selecting = true;
                        }

                        m_GameController.gmDisplay.text += itemText.text;
                        Debug.Log("Sent number with sendSelection : " + itemText.text);
                        Debug.Log("Index is " + m_GameController.index);

                        m_GameController.userSelections[m_GameController.index] = itemText.text;
                        Debug.Log("UserSelection at this index is : " + m_GameController.userSelections[m_GameController.index]);

                        if (m_GameController.index < 3)
                        {
                            m_GameController.index++;
                        }


                    }
                }

            

        }
    }
}