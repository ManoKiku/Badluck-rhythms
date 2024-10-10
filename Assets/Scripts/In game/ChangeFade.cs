using UnityEngine;
using UnityEngine.UI;


public class ChangeFade : MonoBehaviour
{
    [SerializeField]
    private string prefsName = "Fade";
    
    private void Awake() {
        Color temp = gameObject.GetComponent<Image>().color;
        temp.a = PlayerPrefs.GetFloat("Fade");
        gameObject.GetComponent<Image>().color = temp;
    }
}
