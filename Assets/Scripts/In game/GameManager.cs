using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Important objects")]
    [SerializeField]
    private AudioSource _music;    
    [SerializeField]
    private AudioSource _vfx;
    [SerializeField]
    private AudioClip _hit, _miss;
    [SerializeField]
    private GameObject _loseMenu;
    [SerializeField]
    private BeatScroller _bs;
    [SerializeField]
    private Slider _hpSlider;
    [SerializeField]
    private Text _lvlPercent;
    [SerializeField]
    private GameObject _fadePanel;
    [SerializeField]
    private bool _startPlaying;
    [SerializeField]
    private float _hp = 1f;

    [Space(30)]
    [Header("Hit settings")]
    [SerializeField]
    private int _scorePerHit = 50;
    [SerializeField]
    private int _scorePerGoodHit = 100;
    [SerializeField]
    private int _scorePerPerfectHit = 300;

    [Space(30)]
    [Header("Statistics")]
    [SerializeField]
    public int currentScore = 0;
    [SerializeField]
    private int _missCount = 0;
    [SerializeField]
    private int _normalCount = 0;
    [SerializeField]
    private int _goodCount = 0;
    [SerializeField]
    private int _perfectCount = 0;
    [SerializeField]
    private int maxCombo = 0;
    [SerializeField]
    public int comboCount = 0;
    [SerializeField]
    private float _modsMul = 1.0f;

    [Space(30)]
    [Header("Texts")]
    [SerializeField]
    public Text scoreText;
    [SerializeField]
    public Text comboText;
    [SerializeField]
    public Text hitText;

    [Space(30)]
    [Header("Result menu")]
    [SerializeField]
    public GameObject resultObject, uploadRecord;
    [SerializeField]
    public Text resultScoreText;
    [SerializeField]
    public Text resultHitText;

    public static GameManager instance;
    public static KeyCode[] basicButtons = {KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K, KeyCode.L};

    void Awake()
    {
        if(PlayerPrefs.GetString("Player") == string.Empty)
            uploadRecord.SetActive(false);
        StartCoroutine(LevelChoose.FadeOut(_fadePanel));

        _music.volume = PlayerPrefs.GetFloat("MusicPreference");
        _vfx.volume = PlayerPrefs.GetFloat("vfxPreference");

        Debug.Log(Application.streamingAssetsPath + PlayerPrefs.GetInt("Level"));
        instance = this;

        LoadDataFromFile();
        GetAudioFile();
        if(PlayerPrefs.GetInt("Speed") != 0) {
            _music.pitch = 1.5f;
            _modsMul += 0.13f;
        }
        if(PlayerPrefs.GetInt("Invisible") != 0)
            _modsMul += 0.06f;
    }

    // Update is called once per frame
    void Update() {
        if(!_startPlaying) {
           if(Input.anyKeyDown) {
                _startPlaying = true;
                _bs.isStarted = true;

                _music.Play();
            }
        }
        else if(!_bs.isEnded) 
        {
            _hp -= _hp > .4f ? 0.03f * _music.pitch *Time.deltaTime : 0;
            _hpSlider.value = _hp;
            _lvlPercent.text = System.Convert.ToString((int)(BeatScroller.timer / (_bs.lastNote + 3f) * 100)) + "%";
            if(Input.GetKeyDown(KeyCode.Escape)) {
                LoadSceneByName("MainMenu");
            }
            if(Input.GetKeyDown(KeyCode.R)) {
                LoadSceneByName("Game");
            }
        }
    }

    public void NormalHit() {
        Debug.Log("Normal hit!");

        hitText.color = Color.yellow;
        hitText.text = "NORMAL";
        currentScore += (int)(_scorePerHit * (1 + comboCount / 100f) * _modsMul);
        ++_normalCount;
        NoteHit();
    }

    public void GoodHit() {
        Debug.Log("Good hit!");

        hitText.color = Color.blue;
        hitText.text = "GOOD";
        currentScore += (int)(_scorePerGoodHit * (1 + comboCount / 100f) * _modsMul);
        ++_goodCount;
        NoteHit();
    }
    public void PerfectHit() {
        Debug.Log("Perfect hit!");

        hitText.color = Color.green;
        hitText.text = "PERFECT";
        currentScore += (int)(_scorePerPerfectHit * (1 + comboCount / 100f) * _modsMul);
        ++_perfectCount;
        NoteHit();
    }

    public void NoteHit() {
        _vfx.Stop();
        _vfx.PlayOneShot(_hit);

        ++comboCount;
        AddHp(.06f);

        ScoreTextChange();

        if(maxCombo < comboCount) 
            maxCombo = comboCount;
        
    }

    public void NoteMissed() {
        Debug.Log("Missed!");
        hitText.color = Color.red;
        hitText.text = "MISS";

        _vfx.Stop();
        _vfx.PlayOneShot(_miss);

        comboCount = 0;
        ++_missCount;
        AddHp(-.1f);

        if(_hp <= 0) {
            _hp = 0;
            _bs.isEnded = true;
            _music.Pause();
            _loseMenu.SetActive(true);
        }

        ScoreTextChange();
    }

    private void ScoreTextChange() {
        scoreText.text = currentScore.ToString();
        comboText.text = comboCount.ToString() + "x";
    }

    public void ShowResult() {
        resultObject.gameObject.SetActive(true);
        resultScoreText.text = "Score: " + currentScore + "\nMax combo: " + maxCombo;
        resultHitText.text = "Perfect hits:" + _perfectCount + "\nGood hits:" + _goodCount + "\nNormal hits:" + _normalCount + "\nMisses:" + _missCount;
    }

    private string GetMusicPath()
    {
        return Application.streamingAssetsPath + "/"+ PlayerPrefs.GetInt("Level");
    }

    void GetAudioFile()
    {
        WWW w = new WWW(GetMusicPath() + ".mp3");
        _music.clip = NAudioPlayer.FromMp3Data(w.bytes);
    }

    public void SaveLevelToFile()
    {
        if (!File.Exists(GetMusicPath() + ".bytes"))
        {
            File.Create(GetMusicPath() + ".bytes");
        }

        var json = JsonUtility.ToJson(_bs);
        File.WriteAllText(GetMusicPath()  + ".bytes", json);
    }

    public void LoadDataFromFile()
    {
        if (!File.Exists(GetMusicPath()  + ".bytes"))
        {
            //Debug.LogWarning($"File /"{ filePath}/ " not found!, this);
            return;
        }

        var json = File.ReadAllText(GetMusicPath()  + ".bytes");
        JsonUtility.FromJsonOverwrite(json, _bs);
        Debug.Log(json);
    }

    public void LoadSceneByName(string sceneName) {
        StartCoroutine(LevelChoose.FadeIn(_fadePanel));
        StartCoroutine(LevelChoose.SceneLoadDelay(sceneName, 0.5f));
    }

    public void UploadResult() {
        AppDataBase.ExecuteQueryWithoutAnswer($"INSERT INTO Records VALUES ({PlayerPrefs.GetInt("Level")}, '{PlayerPrefs.GetString("Player")}', {currentScore}, {_perfectCount}, {_goodCount}, {_normalCount}, {_missCount})");
    }

    public void AddHp(float num) {
        _hp += num;
        if(_hp > 1) 
            _hp = 1;
    }
}
