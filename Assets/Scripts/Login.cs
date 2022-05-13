using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Login : MonoBehaviour {
    public InputField nameField;
    public InputField passwordField;

    public Button submitButton;

    public void CallLogin() {
        StartCoroutine(LoginAccount());
    }

    IEnumerator LoginAccount() {

        WWWForm form = new WWWForm();
        form.AddField("username", nameField.text);
        form.AddField("password", passwordField.text);

        UnityWebRequest www = UnityWebRequest.Post("http://192.168.0.178/login.php", form);
        yield return www.SendWebRequest();

        if (www.responseCode != 200)
            Debug.Log(www.error);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(3);

    }

    public void goToMainMenu() {
        SceneManager.LoadScene(0);
    }

}
