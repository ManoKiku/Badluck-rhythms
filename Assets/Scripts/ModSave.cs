using System.Collections;
using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.UI;

public class ModSave : MonoBehaviour
{
    [SerializeField]
    string saveName;

    void Start() {
        gameObject.GetComponent<Toggle>().isOn = PlayerPrefs.GetInt(saveName) == 0 ? false : true;
    }

    public void SaveMod() {
        Debug.Log(gameObject.GetComponent<Toggle>().isOn);
        PlayerPrefs.SetInt(saveName, gameObject.GetComponent<Toggle>().isOn ? 1 : 0);
    }
}
