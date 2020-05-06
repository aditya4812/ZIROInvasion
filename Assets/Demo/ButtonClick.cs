using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    ARTapToPlaceObject interaction;
    public bool moveStatus;

    // Start is called before the first frame update
    void Start () {
        interaction = GameObject.Find ("Interaction").GetComponent<ARTapToPlaceObject> ();
        moveStatus = false;
    }

    // Update is called once per frame
    void Update () {
        if (SceneManager.GetActiveScene ().name == "Homepage") {
            GameObject.Find ("High Score").GetComponent<Text> ().text = "High Score: " + PlayerPrefs.GetInt ("highscore").ToString ();
        } //displays high score
        if (moveStatus) {
            if (name == "Joystick Forward") {
                string s_M = "[2,1,0,0,0,0]";
                string s_T = "[60,60,60,60,60,60]";
                string mssg = "{\"pType\":7,\"m_T\":" + s_T + ",\"m_M\":" + s_M + "}";
                GameObject.Find ("Interaction").GetComponent<UDPClient> ().SendValue (mssg);
                s_T = "";
            } else if (name == "Joystick Backward") {
                string s_M = "[1,2,0,0,0,0]";
                string s_T = "[60,60,60,60,60,60]";
                string mssg = "{\"pType\":7,\"m_T\":" + s_T + ",\"m_M\":" + s_M + "}";
                GameObject.Find ("Interaction").GetComponent<UDPClient> ().SendValue (mssg);
                s_T = "";
            } else if (name == "Joystick Left") {
                string s_M = "[1,1,0,0,0,0]";
                string s_T = "[60,60,60,60,60,60]";
                string mssg = "{\"pType\":7,\"m_T\":" + s_T + ",\"m_M\":" + s_M + "}";
                GameObject.Find ("Interaction").GetComponent<UDPClient> ().SendValue (mssg);
                s_T = "";
            } else if (name == "Joystick Right") {
                string s_M = "[2,2,0,0,0,0]";
                string s_T = "[60,60,60,60,60,60]";
                string mssg = "{\"pType\":7,\"m_T\":" + s_T + ",\"m_M\":" + s_M + "}";
                GameObject.Find ("Interaction").GetComponent<UDPClient> ().SendValue (mssg);
                s_T = "";
            }
        }
    }

    public void OnPointerDown (PointerEventData eventData) {
        //SYNTAX: add 'else if' for each additional button, and include what to do on click inside that block
        if (name == "Left" || name == "Left Full") {
            interaction.turn = 1;
        } else if (name == "Right" || name == "Right Full") {
            interaction.turn = 2;
        } else if (name == "Fire" || name == "Fire Full") {
            interaction.fire = true;
        } else if (name == "Joystick Forward") {
            moveStatus = true;
        } else if (name == "Joystick Backward") {
            moveStatus = true;
        } else if (name == "Joystick Left") {
            moveStatus = true;
        } else if (name == "Joystick Right") {
            moveStatus = true;
        } else if (name == "Start") {
            SceneManager.LoadScene ("SampleScene");
        } else if (name == "Home") {
            SceneManager.LoadScene ("Homepage");
        } else if (name == "Reset Score") {
            PlayerPrefs.SetInt ("level", 0);
            PlayerPrefs.SetInt ("increment", 0);
            PlayerPrefs.SetInt ("userDeath", 0);
            PlayerPrefs.SetInt ("currentScore", 0);

            PlayerPrefs.Save ();
            interaction.realLevel = PlayerPrefs.GetInt ("level");
            interaction.incrementLevel = PlayerPrefs.GetInt ("increment");
            SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);

        }
        // else if (name == "Instruction Button") {
        //     SceneManager.LoadScene ("Instructions");

        // } else if (name == "Settings Button") {
        //     SceneManager.LoadScene ("Settings");
        // }

    }

    public void OnPointerUp (PointerEventData eventData) {

        interaction.turn = 0;
        interaction.fire = false;
        moveStatus = false;
        string s_M = "[0,0,0,0,0,0]";
        string s_T = "[60,60,60,60,60,60]";
        string mssg = "{\"pType\":7,\"m_T\":" + s_T + ",\"m_M\":" + s_M + "}";
        GameObject.Find ("Interaction").GetComponent<UDPClient> ().SendValue (mssg);
        s_T = "";
        interaction.bulletSoundCount = 9;

    }
}