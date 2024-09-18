using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChoose : MonoBehaviour
{
    public void ChooseLevel(int id) {
        PlayerPrefs.SetInt("Level", id);
    }

    public void PlayLevel() {
         SceneManager.LoadScene("Game");
    }
}
