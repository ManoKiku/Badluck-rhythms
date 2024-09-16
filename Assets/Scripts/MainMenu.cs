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
    public GameObject signInMenu;

    public TMP_Text username, password, passwordConfirm, email;
    public static void ExitGame() {
        Application.Quit();
    }

    public void ShowSettings() {
        settings.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void HideSettings() {
        settings.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void ShowSignIn() {
        signInMenu.SetActive(true);
        settings.SetActive(false);
    }

    public void HideSignIn() {
        signInMenu.SetActive(false);
        settings.SetActive(true);
    }

    public void SignIn() {
        DataTable users = AppDataBase.GetTable("SELECT * FROM Users WHERE Username = '" + username.text + "'OR email = '" + email.text + "';");
        if (users.Rows == null) {
            Debug.Log("Not register");
        }
        else {
            Debug.Log("Register");
        }
    }
}
