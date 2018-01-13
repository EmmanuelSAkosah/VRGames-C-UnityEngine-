using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityScript.Lang;


public class PlayControl_qg : MonoBehaviour
{

    [SerializeField]    private Text[] Quadtextfields;            //reference to the quads in game
    [SerializeField]    private Text Display;         // reference to the playground display   
    [SerializeField]    private float gameSpeed = 7f;                    //game speed ,1-5
    [SerializeField]    private ListScript_qg m_List;                //database to pull gameplay elements from 
    [SerializeField]    private QuadBehaviour_qg[] Quads;      //reference to the quads,NOTE the size MUST be set as 4 in the inspector before running
    [SerializeField]    private Text scoreDisplay;
    [SerializeField]    private GameObject separator;

    private IEnumerator gameRoutine;    // used as reference to the game running coroutine  

    private string[] textbk = new string[4];                 //array to hold quadfield text for back up
    public string[] userSelections;
    public int index = 0;       // to keep track of how many selections are received
    public Text gmDisplay;

    //spin 
    private Transform sepTransform;
    private Transform sepvTransform;
    private Transform sephTransform;



    private int score;   
    public bool done_selecting, gameRunning;    //flags for controlling player selection and
    private bool game_over;
    int separatorz;

    // Use this for initialization 
    void Start()
    {

        gmDisplay.text = "";
        Display.text = "Press spacebar to start";
        done_selecting = false;
        gameRunning = false;
        game_over = false;
        score = 0;
       
        sepTransform = separator.GetComponent<Transform>();
        sepvTransform = separator.GetComponent<Transform>();
        sephTransform = separator.GetComponent<Transform>();       

        controllerTextbkBackup(); //backs up quad text, copies quadtextfields to textbk
                                 
        resetPlayerSelection(); //initiallizes and clears player selection

        gameRoutine = playGame();  //reference to the game play coroutine
    }

    // Update is called once per frame
    void Update()
    {
        Display.text = gmDisplay.text;
        scoreDisplay.text = "Score :" + score.ToString();



        if (Input.GetKeyDown("space"))
        {
            if (!gameRunning)
            {
                StartCoroutine(gameRoutine);
                gameRunning = true;
                game_over = false;

            }
            else if (game_over)
            {
                StopCoroutine(gameRoutine);
                gameRunning = false;
            }
        }

    }

   // An IEnumerator to introduce waits inside gameplay 
    private IEnumerator wait(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
    }


    private IEnumerator spinSeparator(int type, float speed)
    {
       
        // Create a timer and reset passed selection time.
        float timer = 0f;
        float spinPeriod = 3f;

        // This loop is executed once per frame until the timer exceeds the duration.
        while (timer < spinPeriod)
        {
            if (type == 1)
            {
                //rotate separator
                sepTransform.Rotate(new Vector3(0, 0, (speed) * Time.deltaTime));
            }
            else
            {
                //rotate separator for game over
                //sepTransform.Rotate(new Vector3(speed * Time.deltaTime, speed * Time.deltaTime, speed * Time.deltaTime));
                sepvTransform.Rotate(new Vector3(speed * Time.deltaTime, speed * Time.deltaTime, speed * Time.deltaTime));
                sephTransform.Rotate(new Vector3(speed * Time.deltaTime, speed * Time.deltaTime, speed * Time.deltaTime));
            }
            // Increase the timer by the time between frames and wait for the next frame.
            timer += Time.deltaTime;
            yield return null;

        }

        //have separator set properly before existing spin
        sepTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
    }


    private string[] resetPlayerSelection()
    {
        for (int i = 0; i < 3; i++)
        {
            userSelections[i] = "";
        }
        index = 0;  //reset index too
        done_selecting = false;
        return userSelections;
    }


    //method to get a list of gameplay elements from list script
    public List<string> getList(string[] array)
    {
        List<string> list = new List<string>(array);

        Debug.Log("Got list :" + array.ToString());
        return list;
    }


    public string[] getArray(List<string> list)
    {
        string[] array = new string[4];

        for (int i = 0; i < 3; i++)
        {
            array[i] = list[i];
        }

        print("Got array :" + array.ToString());
        return array;
    }




    // randomize elements in a list   
    private List<E> randomizeList<E>(List<E> inputList)
    {
        List<E> randomList = new List<E>();

        int randomIndex = 0;
        while (inputList.Count > 0)
        {
            //use floor or ceil
            randomIndex = (int)(Random.value * inputList.Count); //Choose a random object in the list
            randomList.Add(inputList[randomIndex]); //add it to the new, random list
            inputList.RemoveAt(randomIndex); //remove to avoid duplicates
        }
        print("Randomized list " + randomList.ToString());
        return randomList; //return the new random list
    }


    private void showPattern(List<string> randomizedList)
    {
        //Change text of the quads
        for (int i = 0; i < 3; i++)
        {
            Quadtextfields[i].text = randomizedList[i];

        }
        Debug.Log("Showed pattern is :" + randomizedList.ToString());
    }


    private void controllerTextbkBackup()               //4 - 3
    {
        //back up controllers version of textbk
        for (int i = 0; i < 3; i++)
        {
            textbk[i] = Quadtextfields[i].text;
        }
    }



    private void updateQuadsTextbk()            //4 - 3
    {
        //back up each quad's textbk
        for (int i = 0; i < 3; i++)
        {
            Quads[i].textbk = textbk[i];
        }
    }


    private void hideQuadText()             //4 - 3
    {
        //then delete item text
        for (int i = 0; i < 3; i++)
        {
            Quadtextfields[i].text = "";
        }
    }


    //set up assignment
    private List<string> askPattern(List<string> list)
    {
        string assignment = "";

        //randomize list  then ask
        list = randomizeList(list);

        //form display string by concatenating
        foreach (string x in list)
        {
            assignment += x + " ";
        }

        Display.text = assignment;

        Debug.Log("Asked pattern " + assignment);
        return list;
    }


    //verify input and count score
    private int verifyInput(List<string> user_answer, List<string> assignment)   //4 - 3
    {
        int score = 0;
        //check each quad choice and givescore
        for (int i = 0; i < 3; i++)
        {
            if (user_answer[i] == assignment[i])
            {
                score += 10;
            }
        }

        return score;
    }



    private IEnumerator playGame()
    {
        yield return spinSeparator(1, 80f);
        List<string> user_answer;
        List<string> assignment;
        score = 0;
        int curr_score = 0;
        int level = 0;
        int stage = 0;
        int roundscount = 0;
        int randIndex;   // to hold index to fetching game play messages

        //show brief game intro and directions
        Display.text = "Instructions to this simple game;";
        yield return StartCoroutine(wait(2.3f));
        Display.text = "You are shown items on the quads";
        yield return StartCoroutine(wait(2.3f));
        Display.text = "Pay attention and note thier positions";
        yield return StartCoroutine(wait(2.3f));
        Display.text = "the items are hidden and you are asked to";
        yield return StartCoroutine(wait(2.3f));
        Display.text = "select the items in a certain random order";
        yield return StartCoroutine(wait(2.3f));
        Display.text = "You can't undo a selection";
        yield return StartCoroutine(wait(2.3f));
        Display.text = "get at least two right to keep playing";
        yield return StartCoroutine(wait(2.3f));


        while (!game_over)
        {
            //get current level's elements from list script
            List<string> Elements = (List<string>)getList(m_List.gameElements[level]);

            //show pattern on quads
            showPattern(Elements);

            //back up quad text fields
            controllerTextbkBackup();

            Display.text = "Collecting your input in " + gameSpeed / 2 + "s";
            //wait for some fraction of gameSpeed
            yield return StartCoroutine(wait(gameSpeed / 2));

            //hide quad text
            hideQuadText();

            //then ask user to select a randomized pattern
            assignment = askPattern(Elements);

            //update textbk of quads so user selection can answer assignment
            updateQuadsTextbk();

            //clear player selction
            resetPlayerSelection();

            //wait for some fraction of gameSpeed
            yield return StartCoroutine(wait(gameSpeed * 7 / 4));  //  why 7/4 , hehe ?

            //collectanswer();
            user_answer = getList(userSelections);

            //check player's input, end game or move on to next stage
            curr_score = verifyInput(user_answer, assignment);




            //show some text , have some fun with player//
            randIndex = (int)(Random.value * (m_List.gamemessages0.Length - 1));
            if (curr_score == 0)
            {
                Display.text = m_List.gamemessages0[randIndex];
            }
            else if (curr_score == 10)
            {
                Display.text = m_List.gamemessages10[randIndex];
                //Display.text = "Not good enough.";
            }
            else if (curr_score == 40)
            {
                if ((level + 1) % 4 == 0)  //if player gets a hard level right, show special message
                {
                    Display.text = m_List.gamemessagesSpecial[randIndex];
                    yield return spinSeparator(1, 300f); //spin to congratulate player
                }
                else
                {
                    Display.text = m_List.gamemessages40[randIndex];
                    //Display.text = "Nice, all matched!";
                }
            }
            else
            {
                Display.text = m_List.gamemessages2030[randIndex];
                //Display.text = "Good, you got some right";
            }
            //wait a sec
            yield return StartCoroutine(wait(1.5f));




            //next level logic//
            score += curr_score;
            if (curr_score >= 20)  //if gamer gets two or more right, 
            {
                roundscount++;
                stage++;                                    //proceed to next level
                Display.text = "Moving on to next stage";

                //wait a sec
                yield return StartCoroutine(wait(1.5f));
            }
            else
            {   //end game
                game_over = true;
                Display.text = "GAME OVER";
                yield return spinSeparator(0, 50f);
            }

            if (stage > 1)
            {  //after 2 successful tries of same elements, proceed to next level
                level++;
                stage = 0;
            }


            //if player makes it to last level of game//
            if (roundscount == (m_List.gameElements.Length) * 2)   //calc last stage from num of elements in list script
            {
                Display.text = "Wunderbar!!!! \n Champion!!";
                //wait a sec
                yield return StartCoroutine(wait(1.5f));
                Display.text = "You Won this Game\nGAME OVER";
                stage = 0;
                level = 0;
                roundscount = 0;
            }

        }
    }

}