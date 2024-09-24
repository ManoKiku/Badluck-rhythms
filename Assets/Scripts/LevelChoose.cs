using System.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelChoose : MonoBehaviour
{
    [SerializeField]
    Text levelName, levelDifficulty, levelDescription, record;
    [SerializeField]
    Transform parentTransform;

    void Awake() {
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

        int count = 0;
        DataTable records = AppDataBase.GetTable($"SELECT Username, Score, Perfect, Good, Miss FROM Records WHERE id = {id} ORDER BY Score DESC");
        foreach(DataRow rows in records.Rows) {
            Text buff = Instantiate(record, parentTransform);
            foreach(DataColumn col in records.Columns) {
                buff.text += rows[col] + "\t";
            }
            buff.rectTransform.anchoredPosition -= new Vector2(0, 30 * count);
            count++;
        }
    }

    public void PlayLevel() {
         SceneManager.LoadScene("Game");
    }
}
