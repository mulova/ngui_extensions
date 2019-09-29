using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveScene : MonoBehaviour {

    public string sceneName = "test1";
	void Update () {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            SceneManager.LoadScene(sceneName);
        }
	}
}
