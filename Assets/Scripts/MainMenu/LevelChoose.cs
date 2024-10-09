using System.Collections;
using System.Data;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelChoose : MonoBehaviour
{
    [SerializeField]
    Button _buttonPrefab;
    [SerializeField]
    private Text _levelName, _levelDifficulty, _levelDescription, _record;
    [SerializeField]
    private Transform _parentTransform, _levelsTransform;

    bool isOdd = false;

    void Awake() {
        GetLevels();
    }

    public void GetLevels() {
        Button[] levelsDelete = _levelsTransform.GetComponentsInChildren<Button>();
        foreach(Button obj in levelsDelete) 
            Destroy(obj.gameObject);
        
        DataTable levels = AppDataBase.GetTable($"SELECT id FROM Levels ORDER BY Difficulty");

        foreach(DataRow rows in levels.Rows) {
            string buff = AppDataBase.ExecuteQueryWithAnswer($"SELECT Name FROM Levels WHERE id = {rows["id"]}");
            if(buff != null && File.Exists($"{Application.streamingAssetsPath}/levels/{rows["id"]}.bytes")) {
                Button inst = Instantiate(_buttonPrefab, _levelsTransform);
                inst.GetComponentsInChildren<Text>()[1].text = buff;
                inst.onClick.AddListener(delegate { ChooseLevel(System.Convert.ToInt32(rows["id"])); });

                if(PlayerPrefs.GetString("Player") != null && PlayerPrefs.HasKey("Player")) {
                    string rank = AppDataBase.ExecuteQueryWithAnswer($"SELECT Rank FROM Records WHERE Username = '{PlayerPrefs.GetString("Player")}' AND id = {System.Convert.ToInt32(rows["id"])} ORDER BY Score DESC LIMIT 1");
                    Text rankText = inst.GetComponentsInChildren<Text>()[0];

                    switch(rank) {
                        case "D":
                        rankText.color = new Color(1, 0, 0);
                        break;
                        case "C":
                        rankText.color = new Color(0.6437531f, 0, 1);
                        break;
                        case "B":
                        rankText.color = new Color(0, 0.263f, 0.729f);;
                        break;
                        case "A":
                        rankText.color = new Color(0, 0.851f, 0.114f);
                        break;
                        case "S":
                        rankText.color = new Color(1, 1, 1);
                        break;
                        case "SS":
                        rankText.color = new Color(1, 1, 0);
                        break;
                    }

                    rankText.text = rank;

                    if(isOdd) {
                        inst.GetComponent<Image>().color = new Color(1, 1, 1);
                        inst.GetComponentsInChildren<Text>()[1].color = new Color(0.6313726f, 0.5568628f, 0.8666667f);
                    }
                    isOdd = !isOdd;
                }
            }
        }
        LoadCurrentLevel();
    }

    public void LoadCurrentLevel() {
        if(!PlayerPrefs.HasKey("Level"))
            PlayerPrefs.SetInt("Level", 1);
            
        ChooseLevel(PlayerPrefs.GetInt("Level"));
    }

    public void ChooseLevel(int id) {
        PlayerPrefs.SetInt("Level", id);
        DataRow level = AppDataBase.GetTable($"SELECT * FROM LEVELS WHERE id = {id}").Rows[0];
        _levelName.text = level["Name"].ToString();
        _levelDifficulty.text = new LocalizedString("StringTable", "Level difficulty").GetLocalizedString() + ": " + level["Difficulty"].ToString();
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
        StartCoroutine(MainMenu.FadeIn(0.5f));
        StartCoroutine(MainMenu.SceneLoadDelay("Game", 0.5f));
    }
}