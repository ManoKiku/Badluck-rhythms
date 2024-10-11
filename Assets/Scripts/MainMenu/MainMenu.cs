using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject staticAdd, mainMenu, playMenu;
    [SerializeField]
    public static GameObject fadePanel;
    [SerializeField]
    public static MainMenu instance;
    [SerializeField]
    private SpriteRenderer bg;
    [SerializeField]
    private Sprite deffaultBg, jonkler;
    [SerializeField]
    private AudioClip deffaultClip, jonklerClip;

    private bool isChanged = false;

    private void Awake() {
        if(!Sign.isFirstStartUp) {
            mainMenu.SetActive(false);
            playMenu.SetActive(true);
        }

        instance = this;
        fadePanel = staticAdd;
        StartCoroutine(FadeOut(0.5f));
    }

    private void Update() {
        if(LevelChoose.levelId == 25 && gameObject.GetComponent<AudioSource>().clip != jonklerClip) {
            bg.sprite = jonkler;
            gameObject.GetComponent<AudioSource>().clip = jonklerClip;
            gameObject.GetComponent<AudioSource>().Play();
        }
        else if(LevelChoose.levelId != 25 && gameObject.GetComponent<AudioSource>().clip != deffaultClip){
            bg.sprite = deffaultBg;
            gameObject.GetComponent<AudioSource>().clip = deffaultClip;
            gameObject.GetComponent<AudioSource>().Play();
        }
    }

    public void ChangeMenu(GameObject hide, GameObject show) {
        StartCoroutine(ChangeWait(hide, show));
    }

    private IEnumerator ChangeWait(GameObject hide, GameObject show) {
        StartCoroutine(MainMenu.FadeIn(0.5f));
        yield return new WaitForSeconds(0.55f);
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
