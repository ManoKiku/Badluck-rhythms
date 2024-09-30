using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LocaleSelector : MonoBehaviour
{
    private bool _active = false;
    [SerializeField]
    TMP_Dropdown localeDropDown;
    

    private void Awake() {
        if(PlayerPrefs.HasKey("Language")) 
            StartCoroutine(SetLocale(PlayerPrefs.GetInt("Language")));
        else 
            StartCoroutine(SetLocale(0));
    }

    public void ChangeLocale(int localeID)
    {
        if (_active)
        {
            return;
        }

        StartCoroutine(SetLocale(localeID));
    }

    private IEnumerator SetLocale(int localeID)
    {
        _active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeID];
        localeDropDown.value = localeID;
        PlayerPrefs.SetInt("Language", localeID);
        _active = false;
    }
}