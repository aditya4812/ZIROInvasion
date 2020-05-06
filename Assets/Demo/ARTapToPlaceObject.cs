using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class ARTapToPlaceObject : MonoBehaviour {
    public GameObject placementIndicator;
    private ARSessionOrigin arOrigin;
    private Pose placementPose;
    private bool placementPoseIsValid = false;
    public GameObject enemyOriginal;
    public GameObject bullet;
    public GameObject gun;
    public int scoreboard;
    public int turn; //0 = none, 1= left, 2 = right
    public bool fire;
    public bool firstRun;
    public int counter; //keeps track of when to generate enemy
    public int generationRate; //speed at which enemy is generated
    public int realLevel; //level
    //public static int highScore = 0;
    public int colorFlash; //keeps track of gun celebration (red or green flash)
    public Material[] skins;
    public AudioClip gunfire;
    public AudioClip enemyKilled;
    public AudioClip userKilled;
    public int bulletSoundCount; //keeps track of looping gunfire audio
    public int enemyDead; //number of killed enemies in current level
    public int incrementLevel; //alternates what aspect of enemy increases difficulty
    public float enemySpeed; //enemy speed
    public int enemyStrength; //enemy full health

    void Start () {
        bulletSoundCount = 9;
        this.gameObject.AddComponent<AudioSource> ();
        fire = false;
        scoreboard = PlayerPrefs.GetInt ("currentScore");
        arOrigin = FindObjectOfType<ARSessionOrigin> ();
        turn = 0;
        firstRun = true;
        counter = 0;
        GameObject.Find ("Enemy").transform.position = new Vector3 (Random.Range (-20, 20), placementIndicator.transform.position.y, Random.Range (-20, -10));
        colorFlash = 101; //0-50 =red, 51-100 = green
        nextLevel ();

    }

    // Update is called once per frame
    void Update () {
        if (PlayerPrefs.GetInt ("userDeath") <= 2) {
            string healthBar = "";
            string deathBar = "";
            for (int i = 0; i < (3 - PlayerPrefs.GetInt ("userDeath")); i++) {
                healthBar += "● ";
                deathBar += "   ";
            }
            for (int i = 0; i < (PlayerPrefs.GetInt ("userDeath")); i++) {
                deathBar += "● ";
            }
            GameObject.Find ("UserHealth").GetComponent<TextMesh> ().text = healthBar.Remove (healthBar.Length - 1);
            GameObject.Find ("Death Bar").GetComponent<TextMesh> ().text = deathBar.Remove (deathBar.Length - 1);

        }

        if (PlayerPrefs.GetInt ("userDeath") > 2) {
            PlayerPrefs.SetInt ("level", 0);
            PlayerPrefs.SetInt ("increment", 0);
            //PlayerPrefs.SetInt ("userDeath", 0);
            PlayerPrefs.SetInt ("currentScore", 0);
            PlayerPrefs.Save ();
            GameObject.Find ("UserHealth").GetComponent<TextMesh> ().text = "";
            GameObject.Find ("Death Bar").GetComponent<TextMesh> ().text = "● ● ●";
            //realLevel = PlayerPrefs.GetInt ("level");
            //incrementLevel = PlayerPrefs.GetInt ("increment");
            StartCoroutine (WaitTime (1, "Homepage"));

        }
        //****************HANDLES CHANGE TO NEXT LEVEL*****************************//
        if (enemyDead >= 5) {
            //Debug.Log (realLevel);
            PlayerPrefs.SetInt ("level", realLevel + 1);
            PlayerPrefs.SetInt ("generationRate", generationRate);
            PlayerPrefs.SetFloat ("enemySpeed", enemySpeed);
            PlayerPrefs.SetInt ("strength", enemyStrength);
            PlayerPrefs.SetInt ("currentScore", scoreboard);

            if (incrementLevel == 3) {
                PlayerPrefs.SetInt ("increment", 1);

            } else {
                PlayerPrefs.SetInt ("increment", incrementLevel + 1);
            }
            PlayerPrefs.Save ();
            Debug.Log (PlayerPrefs.GetFloat ("enemySpeed"));
            nextLevel ();

        }
        //********************************************************************//

        //****************MANAGES COLOR FLASH OF GUN*****************************//
        if (colorFlash < 101) {
            if (colorFlash < 51) {
                GameObject.Find ("AR").GetComponent<MeshRenderer> ().material = skins[0];
                colorFlash++;
                if (colorFlash == 50) {
                    colorFlash = 101;
                }
            } else {
                GameObject.Find ("AR").GetComponent<MeshRenderer> ().material = skins[1];
                colorFlash++;
                if (colorFlash == 100) {
                    colorFlash = 101;
                }
            }
        } else {
            GameObject.Find ("AR").GetComponent<MeshRenderer> ().material = skins[2];

        }
        //********************************************************************//

        //****************DISPLAYS SCORE AND DIFFICULTY*****************************//
        if (scoreboard >= PlayerPrefs.GetInt ("highscore")) {
            PlayerPrefs.SetInt ("highscore", scoreboard);
            PlayerPrefs.Save ();
        }
        GameObject.Find ("Scoreboard").GetComponent<Text> ().text = "Score: " + scoreboard.ToString ();
        if (realLevel == 0) {
            GameObject.Find ("Level").GetComponent<Text> ().text = "Level 1";

        } else {
            GameObject.Find ("Level").GetComponent<Text> ().text = "Level " + realLevel.ToString ();

        }
        //GameObject.Find ("Slider Text").GetComponent<Text> ().text = "Difficulty: " + (realLevel).ToString ();
        if (realLevel >= 10) { //makes enemies smart on hardest level
            GameObject.FindWithTag ("bullet").GetComponent<Rigidbody> ().mass = 1;
        }
        //********************************************************************//

        counter++;
        UpdatePlacementPose ();
        UpdatePlacementIndicator ();
        gun.transform.position = placementIndicator.transform.position;
        bullet.transform.position = placementIndicator.transform.position;

        //****************TURRET TURN AND FIRE LOGIC*****************************//
        if (turn == 1) {
            placementIndicator.transform.Rotate (-Vector3.up * 100 * Time.deltaTime);
            //GameObject.Find ("Bullet").transform.Rotate (-Vector3.up * 100 * Time.deltaTime);
            gun.transform.Rotate (-Vector3.up * 100 * Time.deltaTime);
        }

        if (turn == 2) {

            placementIndicator.transform.Rotate (Vector3.up * 100 * Time.deltaTime);
            //GameObject.Find ("Bullet").transform.Rotate (Vector3.up * 100 * Time.deltaTime);
            gun.transform.Rotate (Vector3.up * 100 * Time.deltaTime);

        }
        if (fire) {
            GameObject clone;
            clone = Instantiate (bullet, placementIndicator.transform.position + placementIndicator.transform.forward, placementIndicator.transform.rotation) as GameObject;
            clone.SetActive (true);
            clone.GetComponent<Collider> ().enabled = true;
            clone.GetComponent<Rigidbody> ().velocity = placementIndicator.transform.forward * 10;
            clone.transform.position = new Vector3 (clone.transform.position.x, placementIndicator.transform.position.y + 0.121f, clone.transform.position.z);
            if (bulletSoundCount == 10) {
                clone.AddComponent<AudioSource> ();
                clone.GetComponent<AudioSource> ().clip = gunfire;
                clone.GetComponent<AudioSource> ().Play ();
                bulletSoundCount = 0;
            }
            bulletSoundCount++;

        }
        //********************************************************************//

        //****************CREATES ENEMIES*****************************//
        if (counter >= (int) generationRate * 20) {
            GameObject enemy;
            enemy = Instantiate (enemyOriginal, new Vector3 (Random.Range (-20, 20), placementIndicator.transform.position.y, Random.Range (-20, 20)), placementIndicator.transform.rotation) as GameObject;
            enemy.transform.LookAt (placementIndicator.transform);
            if ((enemy.transform.position.x < 7 && enemy.transform.position.x > -7) && (enemy.transform.position.z < 7 && enemy.transform.position.z > -7)) {
                enemy.SetActive (false);
                counter--;
            } else {
                enemy.SetActive (true);
                counter = 0;
            }
            enemy.GetComponent<Rigidbody> ().velocity = enemy.transform.forward * enemySpeed;
            enemy.transform.position = new Vector3 (enemy.transform.position.x, placementIndicator.transform.position.y, enemy.transform.position.z);
        }
        //********************************************************************//

    }
    private void UpdatePlacementIndicator () {
        if (placementPoseIsValid) {
            placementIndicator.SetActive (true);
            if (firstRun) {
                placementIndicator.transform.SetPositionAndRotation (placementPose.position, placementPose.rotation);
                firstRun = false;
            } else {
                placementIndicator.transform.position = placementPose.position;
            }

        } else {
            placementIndicator.SetActive (false);
        }
    }
    private void UpdatePlacementPose () { //PLACES THE GUN ON SCREEN
        var screenCenter = Camera.current.ViewportToScreenPoint (new Vector3 (0.5f, 0.5f));
        var hits = new List<ARRaycastHit> ();
        arOrigin.Raycast (screenCenter, hits, TrackableType.Planes);
        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid) {
            placementPose = hits[0].pose;

            // if (turn == 1) {
            //     placementIndicator.transform.rotation = Quaternion.Euler (placementPose.rotation.x, placementPose.rotation.y + 100, placementPose.rotation.z);
            // } else if (turn == 2) {
            //     placementIndicator.transform.rotation = Quaternion.Euler (placementPose.rotation.x, placementPose.rotation.y - 100, placementPose.rotation.z);
            // }
            // placementPose.rotation = placementIndicator.transform.rotation;
            //auto rotates the object with the camera
            // var cameraForward = Camera.current.transform.forward;
            // var cameraBearing = new Vector3 (cameraForward.x, 0, cameraForward.z).normalized;
            // placementPose.rotation = Quaternion.LookRotation (cameraBearing);
        }
    }
    IEnumerator WaitTime (float seconds, string sceneName) {

        yield return new WaitForSeconds (seconds);
        if (sceneName == "Homepage") {
            SceneManager.LoadScene ("Homepage");
            PlayerPrefs.SetInt ("userDeath", 0);
            PlayerPrefs.Save ();
        } else if (sceneName == "SampleScene") {
            SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
        }

    }

    public void nextLevel () {

        incrementLevel = PlayerPrefs.GetInt ("increment");
        //****************REMEMBERS WHAT LEVEL USER IS ON*****************************//
        if (PlayerPrefs.GetInt ("level") == 0) {
            realLevel = 1;

        } else {
            realLevel = PlayerPrefs.GetInt ("level");
        }
        //********************************************************************//

        //************INCREMENTS STRENGTH, SPEED, AND GENERATION RATE*************************//
        if (incrementLevel == 0) {
            generationRate = 10 - realLevel; //how fast enemies are generated, 10 = easiest, 1 = hardest
            enemySpeed = 0.3f;
            enemyStrength = 10;
        } else if (incrementLevel == 1) {
            generationRate = PlayerPrefs.GetInt ("generationRate");
            enemySpeed = PlayerPrefs.GetFloat ("enemySpeed");
            enemyStrength = PlayerPrefs.GetInt ("strength");
            enemyStrength += 10;

        } else if (incrementLevel == 2) {
            generationRate = PlayerPrefs.GetInt ("generationRate");
            enemySpeed = PlayerPrefs.GetFloat ("enemySpeed");
            enemySpeed += 0.1f;
            enemyStrength = PlayerPrefs.GetInt ("strength");

        } else if (incrementLevel == 3) {
            generationRate = PlayerPrefs.GetInt ("generationRate");
            generationRate--;
            enemySpeed = PlayerPrefs.GetFloat ("enemySpeed");
            enemyStrength = PlayerPrefs.GetInt ("strength");
        }

        if (generationRate == 0) {
            generationRate = 1;
        }
        //********************************************************************//
        Debug.Log ("Level: " + realLevel.ToString ());
        Debug.Log ("Increment: " + incrementLevel.ToString ());
        Debug.Log ("genrate: " + generationRate.ToString ());
        Debug.Log ("speed: " + enemySpeed.ToString ());
        Debug.Log ("str: " + enemyStrength.ToString ());
        enemyDead = 0;
    }
}