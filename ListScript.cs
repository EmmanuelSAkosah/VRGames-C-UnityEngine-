using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListScript : MonoBehaviour {



    public string[] letters1 = { "a", "b", "c", "d" };
    public string[] letters2 = { "e", "f", "g", "h" };
    public string[] letters3 = { "w", "x", "y", "z" };
    public string[] numbers1 = { "1", "2", "3", "4" };
    public string[] numbers2 = { "5", "6", "7", "8" };
    public string[] numbers3 = { "18", "19", "20", "21" };
    public string[] symbols1 = { "*", "$", "@", "#" };
    public string[] symbols2 = { "/", "!", "&", "%" };
    public string[] symbols3 = { "`", ">", "'", "|" };

    public string[][] elements; 
    // Use this for initialization
    void Start () {

          elements = { { letters1}, { letters2} } ;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    


}
