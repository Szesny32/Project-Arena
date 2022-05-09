using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public void goToRegister() {
        SceneManager.LoadScene(1);
    }

    public void goToLogin() {
        SceneManager.LoadScene(2);
    }

}
