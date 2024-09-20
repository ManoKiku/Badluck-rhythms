using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{
    public GameObject settings, mainMenu, playMenu, signInMenu, modMenu;

    public TMP_Text username, password, passwordConfirm, email;
    public Text Error;

    public static void ExitGame()
    {
        Application.Quit();
    }

    public void ShowSettings()
    {
        settings.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void HideSettings()
    {
        settings.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void ShowPlayMenu()
    {
        playMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void HidePlayMenu()
    {
        playMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void ShowModMenu()
    {
        modMenu.SetActive(true);
        playMenu.SetActive(false);
    }

    public void HideModMenu()
    {
        modMenu.SetActive(false);
        playMenu.SetActive(true);
    }

    public void ShowSignIn()
    {
        signInMenu.SetActive(true);
        settings.SetActive(false);
    }

    public void HideSignIn()
    {
        signInMenu.SetActive(false);
        settings.SetActive(true);
    }

    public void SetMod(bool setMod, string modName) {
        PlayerPrefs.SetInt(modName, setMod ? 1 : 0);
    }

    public void SignIn()
    {
        if(username.text.Length <= 1) {
            Error.text = "The username field cannot be empty!";
            return;
        }
        else if(password.text.Length <= 1) {
            Error.text = "The password field cannot be empty!";
            return;
        }
        else if(passwordConfirm.text.Length <= 1) {
            Error.text = "The password confirm field cannot be empty!";
            return;
        }
        else if (password.text.Length <= 8) {
            Error.text = "password must be at least 8 characters";
            return;
        }
        else if(password.text != passwordConfirm.text) {
            Error.text = "The Passwords must match!";
            return;
        }
        else if(email.text.Length <= 1) {
            Error.text = "The email field cannot be empty!";
            return;
        }

        string query = $"SELECT * FROM Users WHERE Username = '{username.text.ToString()}' OR Email = '{email.text.ToString()}';";
        Debug.Log(query);
        string answer = AppDataBase.ExecuteQueryWithAnswer(query);

        Debug.Log(answer);

        if (answer != null)
        {
            Error.text = "This username or email is already in use!";
        }
        else
        {
            Debug.Log("Register");
        }
        Error.text = "";
    }
}
