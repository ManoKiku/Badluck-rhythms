using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;


public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject staticAdd;
    [SerializeField]
    public static GameObject fadePanel;
    [SerializeField]
    public static MainMenu instance;

    private void Awake() {
        instance = this;
        fadePanel = staticAdd;
        StartCoroutine(FadeOut(0.5f));
    }

    public void ChangeMenu(GameObject hide, GameObject show) {
        StartCoroutine(ChangeWait(hide, show));
    }

    private IEnumerator ChangeWait(GameObject hide, GameObject show) {
        StartCoroutine(MainMenu.FadeIn(0.5f));
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(MainMenu.FadeOut(0.5f));
        hide.SetActive(false);
        show.SetActive(true);
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        StartCoroutine(ExitWait());
        #endif
    }

    public static IEnumerator FadeIn(float time) {
        fadePanel.SetActive(true);
        fadePanel.GetComponent<Animator>().Play("FadeIn");
        yield return new WaitForSeconds(time);
    }

    public static IEnumerator FadeOut(float time) {
        fadePanel.GetComponent<Animator>().Play("FadeOut");
        yield return new WaitForSeconds(time);
        fadePanel.SetActive(false);

    }

    public static IEnumerator SceneLoadDelay(string sceneName, float delay) {
        yield return new WaitForSeconds(delay);
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(sceneName);
    }

    private IEnumerator ExitWait() {
        fadePanel.SetActive(true);
        fadePanel.GetComponent<Animator>().Play("FadeIn");
        yield return new WaitForSeconds(0.5f);
        Application.Quit();
    }
}
