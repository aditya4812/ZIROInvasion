using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARFoundation;

public class PositionTracker : MonoBehaviour {

    public GameObject placementIndicator;
    public bool collisionDetect;
    public int hitCount;
    public int strength;

    // Start is called before the first frame update
    void Start () {
        collisionDetect = false;
        hitCount = 0;
        strength = 10; //how many hits needed to kill enemy
    }

    // Update is called once per frame
    void Update () {

        if (collisionDetect == false) {
            this.transform.LookAt (placementIndicator.transform);
            this.GetComponent<Rigidbody> ().velocity = this.transform.forward * 0.3f;
        }
        this.transform.position = new Vector3 (this.transform.position.x, placementIndicator.transform.position.y, this.transform.position.z);
        transform.Find ("Health").GetComponent<TextMesh> ().text = "Health: " + (GameObject.Find ("Interaction").GetComponent<ARTapToPlaceObject> ().enemyStrength - hitCount).ToString ();

    }

    void OnCollisionEnter (Collision target) {
        if (target.gameObject.tag == "Player") { //if enemy kills user
            collisionDetect = true;
            this.gameObject.active = false;
            GameObject.Find ("Interaction").GetComponent<ARTapToPlaceObject> ().scoreboard--;
            GameObject.Find ("Interaction").GetComponent<ARTapToPlaceObject> ().colorFlash = 0;
            Debug.Log (GameObject.Find ("AR").GetComponent<MeshRenderer> ().materials[0].ToString ());
            GameObject.Find ("Interaction").GetComponent<AudioSource> ().clip = GameObject.Find ("Interaction").GetComponent<ARTapToPlaceObject> ().userKilled;
            GameObject.Find ("Interaction").GetComponent<AudioSource> ().Play ();
            PlayerPrefs.SetInt ("userDeath", PlayerPrefs.GetInt ("userDeath") + 1);

        } else if (target.gameObject.tag == "bullet") { // if user kills enemy
            hitCount++;
            target.gameObject.SetActive (false);
            if (hitCount >= GameObject.Find ("Interaction").GetComponent<ARTapToPlaceObject> ().enemyStrength) {
                collisionDetect = true;
                this.gameObject.active = false;
                GameObject.Find ("Interaction").GetComponent<ARTapToPlaceObject> ().scoreboard++;
                GameObject.Find ("Interaction").GetComponent<ARTapToPlaceObject> ().colorFlash = 51;
                GameObject.Find ("Interaction").GetComponent<AudioSource> ().clip = GameObject.Find ("Interaction").GetComponent<ARTapToPlaceObject> ().enemyKilled;
                GameObject.Find ("Interaction").GetComponent<AudioSource> ().Play ();
                GameObject.Find ("Interaction").GetComponent<ARTapToPlaceObject> ().enemyDead++;
            }

        }
    }

}