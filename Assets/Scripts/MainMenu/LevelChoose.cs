using System;
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
    GameObject _record;
    [SerializeField]
    Button _buttonPrefab;
    [SerializeField]
    private Text _levelName, _levelDifficulty, _levelDescription;
    [SerializeField]
    private Transform _parentTransform, _levelsTransform;

    public static int levelId = 1;

    void Awake() {
        GetLevels();
    }

    public void GetLevels() {
        Button[] levelsDelete = _levelsTransform.GetComponentsInChildren<Button>();
        foreach(Button obj in levelsDelete) 
            Destroy(obj.gameObject);
        
        DataTable levels = AppDataBase.GetTable($"SELECT id FROM Levels ORDER BY Difficulty");

        bool isOdd = false;

        foreach(DataRow rows in levels.Rows) {
            string buff = AppDataBase.ExecuteQueryWithAnswer($"SELECT Name FROM Levels WHERE id = {rows["id"]}");
            if(buff != null && File.Exists($"{Application.streamingAssetsPath}/levels/{rows["id"]}.bytes")) {
                Button inst = Instantiate(_buttonPrefab, _levelsTransform);
                inst.GetComponentsInChildren<Text>()[1].text = buff;
                inst.onClick.AddListener(delegate { ChooseLevel(System.Convert.ToInt32(rows["id"])); });

                if(Sign.login != String.Empty) {
                    string rank = AppDataBase.ExecuteQueryWithAnswer($"SELECT Rank FROM Records WHERE Username = '{Sign.login}' AND id = {System.Convert.ToInt32(rows["id"])} ORDER BY Score DESC LIMIT 1");
                    Text rankText = inst.GetComponentsInChildren<Text>()[0];

                    rankText.color = GetRankColor(rank);
                    rankText.text = rank;
                }

                if(isOdd) {
                    inst.GetComponent<Image>().color = new Color(1, 1, 1);
                    inst.GetComponentsInChildren<Text>()[1].color = new Color(0.6313726f, 0.5568628f, 0.8666667f);
                }
                isOdd = !isOdd;
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
        levelId = id;

        DataRow level = AppDataBase.GetTable($"SELECT * FROM LEVELS WHERE id = {id}").Rows[0];
        _levelName.text = level["Name"].ToString();
        _levelDifficulty.text = $"{new LocalizedString("StringTable", "Level difficulty").GetLocalizedString()}: {level["Difficulty"]}";
        _levelDescription.text = level["Description"].ToString();

        Mask[] allObjects = _parentTransform.gameObject.GetComponentsInChildren<Mask>(true);
        foreach(Mask obj in allObjects) 
            Destroy(obj.gameObject);
        
        DataTable records = AppDataBase.GetTable($"SELECT Username, Score, Perfect, Good, Okay, Miss, Rank FROM Records WHERE id = {id} ORDER BY Score DESC");

        Text[] recordUnit = new Text[8];


        int count = 0;
        foreach(DataRow rows in records.Rows) {
            count++;
            GameObject buff = Instantiate(_record, _parentTransform);
            recordUnit = buff.GetComponentsInChildren<Text>();
            recordUnit[0].text =  $"#{count}";
            recordUnit[1].text = rows["Rank"].ToString();
            recordUnit[1].color = GetRankColor(recordUnit[1].text);
            recordUnit[2].text = rows["Username"].ToString();
            recordUnit[3].text = rows["Score"].ToString();
            recordUnit[4].text = rows["Perfect"].ToString();
            recordUnit[5].text = rows["Good"].ToString();
            recordUnit[6].text = rows["Okay"].ToString();
            recordUnit[7].text = rows["Miss"].ToString();
        }
    }

    public static Color GetRankColor(string rank) {
        switch(rank) {
        case "D":
        return new Color(1, 0, 0);
        case "C":
         return new Color(0.6437531f, 0, 1);
        case "B":
         return new Color(0, 0.263f, 0.729f);;
        case "A":
         return new Color(0, 0.851f, 0.114f);
        case "S":
         return new Color(0.5660378f, 0.5660378f, 0.5660378f);
        case "SS":
         return new Color(1, 1, 0);
        }
        return new Color(0, 0, 0, 0);
    }

    public void PlayLevel() {
        StartCoroutine(MainMenu.FadeIn(0.5f));
        StartCoroutine(MainMenu.SceneLoadDelay("Game", 0.5f));
    }
}