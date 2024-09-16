using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject settings;
    public GameObject mainMenu;
    public void ExitGame() {
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
}
