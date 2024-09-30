using System.Collections;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelChoose : MonoBehaviour
{
    [SerializeField]
    private GameObject _fadePanel;
    [SerializeField]
    private Text levelName, levelDifficulty, levelDescription, record;
    [SerializeField]
    private Transform parentTransform;

    void Awake() {
        StartCoroutine(FadeOut(_fadePanel));
        if(!PlayerPrefs.HasKey("Level"))
            PlayerPrefs.SetInt("Level", 1);
            
        ChooseLevel(PlayerPrefs.GetInt("Level"));
    }

    public void ChooseLevel(int id) {
        PlayerPrefs.SetInt("Level", id);
        DataRow level = AppDataBase.GetTable($"SELECT * FROM LEVELS WHERE id = {id}").Rows[0];
        levelName.text = level["Name"].ToString();
        levelDifficulty.text = "Level difficulty: " + level["Difficulty"].ToString();
        levelDescription.text = level["Description"].ToString();

        Text[] allObjects = parentTransform.gameObject.GetComponentsInChildren<Text>(true);
        foreach(Text obj in allObjects) 
            Destroy(obj.gameObject);
        
        DataTable records = AppDataBase.GetTable($"SELECT Username, Score, Perfect, Good, Okay, Miss FROM Records WHERE id = {id} ORDER BY Score DESC");

        foreach(DataRow rows in records.Rows) {
            foreach(DataColumn col in records.Columns) {
                Text buff = Instantiate(record, parentTransform);
                buff.text += rows[col];
            }
        }
    }

    public void PlayLevel() {
        StartCoroutine(FadeIn(_fadePanel));
        StartCoroutine(SceneLoadDelay("Game", 0.5f));
    }

    public static IEnumerator FadeIn(GameObject fadePanel) {
        fadePanel.SetActive(true);
        fadePanel.GetComponent<Animator>().Play("FadeIn");
        yield return new WaitForSeconds(0.5f);
    }

    public static IEnumerator FadeOut(GameObject fadePanel) {
        fadePanel.GetComponent<Animator>().Play("FadeOut");
        yield return new WaitForSeconds(0.5f);
        fadePanel.SetActive(false);
    }

    public static IEnumerator SceneLoadDelay(string sceneName, float delay) {
        yield return new WaitForSeconds(delay);
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(sceneName);
    }
}
