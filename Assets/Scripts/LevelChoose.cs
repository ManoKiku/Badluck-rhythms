using System.Collections;
using System.Data;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelChoose : MonoBehaviour
{
    [SerializeField]
    private GameObject _fadePanel;
    [SerializeField]
    Button _buttonPrefab;
    [SerializeField]
    private Text _levelName, _levelDifficulty, _levelDescription, _record;
    [SerializeField]
    private Transform _parentTransform, _levelsTransform;

    void Awake() {
        StartCoroutine(FadeOut(_fadePanel));

        DataTable levels = AppDataBase.GetTable($"SELECT id FROM Levels ORDER BY Difficulty");

        foreach(DataRow rows in levels.Rows) {
            string buff = AppDataBase.ExecuteQueryWithAnswer($"SELECT Name FROM Levels WHERE id = {rows["id"]}");
            if(buff != null && File.Exists($"{Application.streamingAssetsPath}/levels/{rows["id"]}.bytes")) {
                Button inst = Instantiate(_buttonPrefab, _levelsTransform);
                inst.GetComponentInChildren<Text>().text = buff;
                inst.onClick.AddListener(delegate { ChooseLevel(System.Convert.ToInt32(rows["id"])); });
            }
        }

        if(!PlayerPrefs.HasKey("Level"))
            PlayerPrefs.SetInt("Level", 1);
            
        ChooseLevel(PlayerPrefs.GetInt("Level"));
    }

    public void ChooseLevel(int id) {
        PlayerPrefs.SetInt("Level", id);
        DataRow level = AppDataBase.GetTable($"SELECT * FROM LEVELS WHERE id = {id}").Rows[0];
        _levelName.text = level["Name"].ToString();
        _levelDifficulty.text = "Level difficulty: " + level["Difficulty"].ToString();
        _levelDescription.text = level["Description"].ToString();

        Text[] allObjects = _parentTransform.gameObject.GetComponentsInChildren<Text>(true);
        foreach(Text obj in allObjects) 
            Destroy(obj.gameObject);
        
        DataTable records = AppDataBase.GetTable($"SELECT Username, Score, Perfect, Good, Okay, Miss FROM Records WHERE id = {id} ORDER BY Score DESC");

        foreach(DataRow rows in records.Rows) {
            foreach(DataColumn col in records.Columns) {
                Text buff = Instantiate(_record, _parentTransform);
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