using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public TMP_InputField logUsername, logPassword, username, password, passwordConfirm, email;
    public Text signError, logError, usernameMenu;

    public TMP_Dropdown resolutionDropdown;
    public Slider musicSlider, vfxSlider;
    float musicVolume, vfxVolume;
    Resolution[] resolutions;

    private static bool isFirstStartUp = true;

    void Awake()
    {
        if(isFirstStartUp) {
            Debug.Log("First!");
            isFirstStartUp = false;
        }
        else {
            Debug.Log("No!");
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

    public void SetVolume(float volume)
    {
        musicVolume = volume;
        SaveSettings();
    }
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        SaveSettings();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        SaveSettings();
    }

    
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        SaveSettings();
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("FullscreenPreference", System.Convert.ToInt32(Screen.fullScreen));
        PlayerPrefs.SetFloat("MusicPreference", musicVolume);
        PlayerPrefs.SetFloat("vfxPreference", vfxVolume);
    }

    public void LoadSettings(int currentResolutionIndex)
    {
        if (PlayerPrefs.HasKey("ResolutionPreference"))
            resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionPreference");
        else
            resolutionDropdown.value = currentResolutionIndex;
        if (PlayerPrefs.HasKey("FullscreenPreference"))
            Screen.fullScreen = System.Convert.ToBoolean(PlayerPrefs.GetInt("FullscreenPreference"));
        else
            Screen.fullScreen = true;
        if (PlayerPrefs.HasKey("MusicPreference"))
            musicSlider.value = PlayerPrefs.GetFloat("MusicPreference");
        else
            musicSlider.value = PlayerPrefs.GetFloat("MusicPreference");
        if (PlayerPrefs.HasKey("vfxPreference"))
            vfxSlider.value = PlayerPrefs.GetFloat("vfxPreference");
        else
            vfxSlider.value = PlayerPrefs.GetFloat("vfxcPreference");
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
            logError.color = Color.green;
            logError.text = "Successfully logged in!";
            PlayerPrefs.SetString("Player", username.text);
        }
        else
        {
            logError.text = "Invalid login information entered!";
        }
    }

    public void SignIn()
    {
        signError.color = Color.red;
        if(username.text.Length < 1) {
            signError.text = "The username field cannot be empty!";
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
        else if(email.text.Length < 1) {
            signError.text = "The email field cannot be empty!";
            return;
        }

        string query = $"SELECT * FROM Users WHERE Username = '{username.text}' OR Email = '{email.text}';";
        Debug.Log(query);
        string answer = AppDataBase.ExecuteQueryWithAnswer(query);

        Debug.Log(answer);

        if (answer != null)
        {
            signError.text = "This username or email is already in use!";
        }
        else
        {
            usernameMenu.text = username.text;
            signError.color = Color.green;
            signError.text = "Successfully registered!";
            AppDataBase.ExecuteQueryWithoutAnswer($"INSERT INTO Users (Username, Password, Email) VALUES ('{username.text}', '{password.text}', '{email.text}')");
            PlayerPrefs.SetString("Player", username.text);
        }
    }
}
