using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuChange : MonoBehaviour
{
    [SerializeField]
    private GameObject hide, show;

    private void Awake() {
        gameObject.GetComponent<Button>().onClick.AddListener(() => MainMenu.instance.ChangeMenu(hide, show));
    }
}
