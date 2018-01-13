using MyScenes.QuadGame.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    [SerializeField] private Text Display;      // reference to the playground display
    //[SerializeField] private QuadBehaviour[] Quads;            //reference to the quads in game
    [SerializeField] private Text[] Quadtextfields;            //reference to the quads in game
    [SerializeField] private int GameSpeed;                    //game speed ,1-5
    [SerializeField] private ListScript m_List;                //database to pull gameplay elements from 

    private Text gmDisplay;                                     //get access to the display of the playground

    public string[] userSelections;
    public int index;       // to keep track of how many selctions are received
    //private Texture[] Quads;


    private QuadBehaviour[] m_Quads;
    private bool game_over;


    // Use this for initialization
    void Start()
    {
        /*
        //new and initialize quads
        QuadBehaviour[] m_Quads = new QuadBehaviour[4];
   
        for (int i = 0; i < 4; i++)
        {
            m_Quads[i] = Quads[i];
        }
        */
        //new and initialize selection
        string[] userSelections = new string[4];
        for (int i = 0; i < 4; i++)
        {
            userSelections[i] = "";
        }

        //initialize user slection index
        index = 0;

        //initialize display
        gmDisplay = Display;

        game_over = false;
    }


    // Update is called once per frame
    void Update()
    {

    }



    //method to get a list of gameplay element from list script
    public List<string> getList(string[] array)
    {
        List<string> list = new List<string>(array);
        return list;
    }

   
    // randomize elements in a list   
    private List<E> randomizeList<E>(List<E> inputList)
    {
        List<E> randomList = new List<E>();
   
        int randomIndex = 0;
        while (inputList.Count > 0)
        {
            //use floor or ceil
            randomIndex = (int) (Random.value * inputList.Count); //Choose a random object in the list
            randomList.Add(inputList[randomIndex]); //add it to the new, random list
            inputList.RemoveAt(randomIndex); //remove to avoid duplicates
        }

        return randomList; //return the new random list
    }


    //display assignment
    private void showPattern(List<string> randomizedList)
    {

        //Change text of the quads
        for (int i = 0; i < 4; i++)
        {
            Quadtextfields[i].text = randomizedList[i]; 
        }

        // wait for a few seconds
    }


    //set up assignment
    private void askPattern(List<string> list)
    {
        string assignment = "";
        //randomize list  then ask
        randomizeList(list);

        //form display string by concatenating
        foreach (string x in list)
        {
            assignment = assignment + x;
        }
       
        Display.text = assignment;
        //wait for a few second
    }



    
    //count score

    private bool verifyInput()
    {
        //
        return true;
    }

    private IEnumerator playGame()
    {
        while (!game_over)
        {
            //Get list from list script
          List<string>  Elements = (List<string>)getList(m_List.letters1);

            //show pattern on quads
          showPattern(Elements);
            //wait a for some fraction of gameSpeed

            //ask user to selct a randomized pattern
            askPattern(Elements);

            //wait for pattern selection completion
           if(verifyInput())
            {

            }
            else
            {
                game_over = true;
            }

            
        }
        yield return null;

    }
}