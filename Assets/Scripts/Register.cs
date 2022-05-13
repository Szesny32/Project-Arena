using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Register : MonoBehaviour
{
    public InputField nameField;
    public InputField passwordField;

    public Button submitButton;

    public void CallRegister() {

        StartCoroutine(RegisterAccount());

    }

    IEnumerator RegisterAccount() {

        WWWForm form = new WWWForm();
        form.AddField("username", nameField.text);
        form.AddField("password", passwordField.text);

        UnityWebRequest www = UnityWebRequest.Post("http://192.168.0.178/register.php", form);
        yield return www.SendWebRequest();

        if (www.responseCode != 200)
            Debug.Log(www.error);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);

    }

    public void goToMainMenu() {
        SceneManager.LoadScene(0);
    }

}
