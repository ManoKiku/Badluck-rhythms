using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections;


public class SettingsMenu : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField logUsername, logPassword, username, password, passwordConfirm;
    [SerializeField]
    private Text signError, logError, usernameMenu, loginText, passwordText, loggedAsText;
    [SerializeField]
    private Button logInButton, logOutButton;

    [SerializeField]
    private TMP_Dropdown resolutionDropdown;
    [SerializeField]
    private Slider musicSlider, vfxSlider;
    [SerializeField]
    private Toggle rememberAccount, fullScreen;
    float musicVolume, vfxVolume;
    Resolution[] resolutions;

    private static bool isFirstStartUp = true;
    public static string login = String.Empty;

    void Awake()
    {
        rememberAccount.isOn = PlayerPrefs.GetInt("DoRemember") != 0;
        if(isFirstStartUp) {
            isFirstStartUp = false;
            if(IsLogenIn() && rememberAccount.isOn) {
                login = PlayerPrefs.GetString("Player");
            }
            else
                PlayerPrefs.SetString("Player", String.Empty);
        }
        
        if(login != String.Empty) {
            usernameMenu.text = login;
            loggedAsText.text = "Logged as " + login;
            LoggedMenu(false);
        }


        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        resolutions = Screen.resolutions;
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height + " " + resolutions[i].refreshRate + "Hz";
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width
                  && resolutions[i].height == Screen.currentResolution.height)
                currentResolutionIndex = i;
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.RefreshShownValue();
        LoadSettings(currentResolutionIndex);
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

    bool IsLogenIn() {
        return PlayerPrefs.HasKey("Player") && PlayerPrefs.GetString("Player") != String.Empty;
    }

    public void SetVfx(float volume)
    {
        vfxVolume = volume;
    }

    public void SetMusic(float volume)
    {
        musicVolume = volume;
        GetComponent<AudioSource>().volume = musicSlider.value;
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResolutionPreference", resolutionIndex);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("FullscreenPreference", System.Convert.ToInt32(Screen.fullScreen));
        PlayerPrefs.SetFloat("MusicPreference", musicVolume);
        PlayerPrefs.SetFloat("vfxPreference", vfxVolume);
        Debug.Log(PlayerPrefs.GetFloat("MusicPreference"));
        Debug.Log("Saved!");
    }

    public void LoadSettings(int currentResolutionIndex)
    {
        if (PlayerPrefs.HasKey("ResolutionPreference"))
            resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionPreference");
        else
            resolutionDropdown.value = currentResolutionIndex;
        if (PlayerPrefs.HasKey("FullscreenPreference")) {
            Screen.fullScreen = System.Convert.ToBoolean(PlayerPrefs.GetInt("FullscreenPreference"));
            fullScreen.isOn = Screen.fullScreen;
        }
        if (PlayerPrefs.HasKey("MusicPreference")) {
            musicVolume = PlayerPrefs.GetFloat("MusicPreference");
            musicSlider.value = musicVolume;
            GetComponent<AudioSource>().volume = musicVolume;
        }
        else 
            musicVolume = 1;
        if (PlayerPrefs.HasKey("vfxPreference")) {
            vfxVolume = PlayerPrefs.GetFloat("vfxPreference");
            vfxSlider.value = vfxVolume;
        }
        else
            vfxVolume = 1;
    }

    public void LogIn()
    {
        logError.color = Color.red;

        if(logUsername.text.Length < 1) {
            logError.text = "The username field cannot be empty!";
            return;
        }
        else if(logPassword.text.Length < 1) {
            logError.text = "The password field cannot be empty!";
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
            loggedAsText.text = "Logged as " + login;
            logError.text = "Successfully logged in!";
            PlayerPrefs.SetString("Player", logUsername.text);
            Debug.Log(PlayerPrefs.GetString("Player"));
        }
        else
        {
            logError.text = "Invalid login information entered!";
        }
    }

    public void RememberUser(bool check) {
        PlayerPrefs.SetInt("DoRemember", check ? 1 : 0);
    }

    public void LogOut() {
        PlayerPrefs.SetString("Player", null);
        LoggedMenu(true);
        usernameMenu.text = "Guest";
        login = null;
    }

    public void SignIn()
    {
        signError.color = Color.red;
        if(username.text.Length < 4) {
            signError.text = "The username must be at least 4 characters!";
            return;
        }
        else if(password.text.Length < 1) {
            signError.text = "The password field cannot be empty!";
            return;
        }
        else if(passwordConfirm.text.Length < 1) {
            signError.text = "The password confirm field cannot be empty!";
            return;
        }
        else if (password.text.Length < 8) {
            signError.text = "password must be at least 8 characters";
            return;
        }
        else if(password.text != passwordConfirm.text) {
            signError.text = "The Passwords must match!";
            return;
        }

        string query = $"SELECT * FROM Users WHERE Username = '{username.text}';";
        Debug.Log(query);
        string answer = AppDataBase.ExecuteQueryWithAnswer(query);

        Debug.Log(answer);

        if (answer != null)
        {
            signError.text = "This username is already in use!";
        }
        else
        {
            signError.color = Color.green;
            signError.text = "Successfully registered!";
            AppDataBase.ExecuteQueryWithoutAnswer($"INSERT INTO Users (Username, Password) VALUES ('{username.text}', '{password.text}')");
        }
    }
}
