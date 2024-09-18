using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;

public class MainMenu : MonoBehaviour
{
    public GameObject settings;
    public GameObject mainMenu;
    public GameObject playMenu;
    public GameObject signInMenu;

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

        string query = "SELECT * FROM Users WHERE Username = '111' OR Email = '111'";
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
    }
}
