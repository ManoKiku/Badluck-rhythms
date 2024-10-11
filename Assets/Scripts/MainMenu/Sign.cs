using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class Sign : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField logUsername, logPassword, username, password, passwordConfirm;

    [SerializeField]
    private Text signError, logError, usernameMenu, loginText, passwordText, loggedAsText;

    [SerializeField]
    private Button logInButton, logOutButton;

    [SerializeField]
    private Toggle rememberAccount;

    public static string login = String.Empty;
    public static bool isFirstStartUp = true;

    void Awake() {
        rememberAccount.isOn = PlayerPrefs.GetInt("DoRemember") != 0;
        if(isFirstStartUp) {
            if(IsLogenIn() && rememberAccount.isOn) {
                login = PlayerPrefs.GetString("Player");
            }
            else
                PlayerPrefs.SetString("Player", String.Empty);
        }
        
        if(login != String.Empty) {
            usernameMenu.text = login;
            loggedAsText.text = new LocalizedString("StringTable", "Logged as").GetLocalizedString() + login;
            LoggedMenu(false);
        }
    }

    void Start() {
        isFirstStartUp = false;
    }

    bool IsLogenIn() {
        return PlayerPrefs.HasKey("Player") && PlayerPrefs.GetString("Player") != String.Empty;
    }

    void LoggedMenu(bool isHide) {
        logUsername.gameObject.SetActive(isHide);
        logPassword.gameObject.SetActive(isHide);
        logInButton.gameObject.SetActive(isHide);
        loginText.gameObject.SetActive(isHide);
        passwordText.gameObject.SetActive(isHide);
        logOutButton.gameObject.SetActive(!isHide);
        rememberAccount.gameObject.SetActive(isHide);
        loggedAsText.gameObject.SetActive(!isHide);
    }

    public void LocaleLogin() 
    {
        loggedAsText.text = new LocalizedString("StringTable", "Logged as").GetLocalizedString() + login;
    }

    public void LogIn()
    {
        logError.color = Color.red;

        if(logUsername.text.Length < 4) {
            logError.text = new LocalizedString("StringTable", "User char error").GetLocalizedString();
            return;
        }
        else if(logPassword.text.Length < 8) {
            logError.text = new LocalizedString("StringTable", "Password error").GetLocalizedString();
            return;
        }

        string query = $"SELECT * FROM Users WHERE Username = '{logUsername.text}' AND Password = '{logPassword.text}';";
        Debug.Log(query);
        string answer = AppDataBase.ExecuteQueryWithAnswer(query);

        Debug.Log(answer);

        if (answer != null)
        {
            usernameMenu.text = logUsername.text;
            login = logUsername.text;
            LoggedMenu(false);
            logError.color = Color.green;
            loggedAsText.text = new LocalizedString("StringTable", "Logged as").GetLocalizedString() + login;
            logError.text = new LocalizedString("StringTable", "Logged in").GetLocalizedString();
            PlayerPrefs.SetString("Player", logUsername.text);
            Debug.Log(PlayerPrefs.GetString("Player"));
        }
        else
        {
            logError.text = new LocalizedString("StringTable", "Invalid information").GetLocalizedString();
        }
    }

    public void RememberUser(bool check) {
        PlayerPrefs.SetInt("DoRemember", check ? 1 : 0);
    }

    public void LogOut() {
        PlayerPrefs.DeleteKey("Player");
        LoggedMenu(true);
        usernameMenu.text = "Guest";
        login = String.Empty;
    }

    public void SignIn()
    {
        signError.color = Color.red;
        if(username.text.Length < 4) {
            signError.text = new LocalizedString("StringTable", "User char error").GetLocalizedString();
            return;
        }
        else if(password.text.Length < 8) {
            signError.text = new LocalizedString("StringTable", "Password error").GetLocalizedString();
            return;
        }
        else if(passwordConfirm.text.Length < 1) {
            signError.text = new LocalizedString("StringTable", "Password confirm error").GetLocalizedString();
            return;
        }
        else if(password.text != passwordConfirm.text) {
            signError.text = new LocalizedString("StringTable", "Password match error").GetLocalizedString();
            return;
        }

        string query = $"SELECT * FROM Users WHERE Username = '{username.text}';";
        Debug.Log(query);
        string answer = AppDataBase.ExecuteQueryWithAnswer(query);

        Debug.Log(answer);

        if (answer != null)
        {
            signError.text = new LocalizedString("StringTable", "Username in use").GetLocalizedString(); 
        }
        else
        {
            signError.color = Color.green;
            signError.text = new LocalizedString("StringTable", "Registered").GetLocalizedString();
            AppDataBase.ExecuteQueryWithoutAnswer($"INSERT INTO Users (Username, Password) VALUES ('{username.text}', '{password.text}')");
        }
    }
}
