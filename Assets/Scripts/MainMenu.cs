using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{
    public static void ExitGame()
    {
         #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
