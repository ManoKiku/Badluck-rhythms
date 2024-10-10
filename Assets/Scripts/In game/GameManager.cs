using System.IO;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Localization;
using UnityEditor;
using Unity.VisualScripting;
using UnityEngine.Rendering;
using System.Collections;
using System.Net.NetworkInformation;
using System;

public class GameManager : MonoBehaviour
{
    [Header("Important objects")]
    [SerializeField]
    private AudioSource _music, _vfx;    
    [SerializeField]
    private AudioClip _hit, _miss;
    [SerializeField]
    private GameObject _loseMenu, _pauseMenu, _fadePanel, _anyKey;
    [SerializeField]
    public BeatScroller _bs;
    [SerializeField]
    private Slider _hpSlider;
    [SerializeField]
    private Text _lvlPercent, _pauseTimer;
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
    float _accuracy;
    [SerializeField]
    private float _modsMul = 1.0f;
    [SerializeField]
    private bool godMod = false;

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
    public GameObject resultObject;
    [SerializeField]
    public Text resultScoreText, resultHitText, levelRank;

    private string[] locale = new string[4];
    public static GameManager instance;
    public static KeyCode[] basicButtons = {KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K, KeyCode.L};

    public static float waitTimer = 0.5f;

    void Awake()
    {
        MainMenu.fadePanel = _fadePanel;

        waitTimer = 1;

        locale[0] = new LocalizedString("StringTable", "Perfect").GetLocalizedString();
        locale[1] = new LocalizedString("StringTable", "Good").GetLocalizedString();
        locale[2] = new LocalizedString("StringTable", "Normal").GetLocalizedString();
        locale[3] = new LocalizedString("StringTable", "Miss").GetLocalizedString();

        StartCoroutine(MainMenu.FadeOut(0.5f));

        if(PlayerPrefs.HasKey("MusicPreference"))
            _music.volume = PlayerPrefs.GetFloat("MusicPreference");
        if(PlayerPrefs.HasKey("vfxPreference"))
            _vfx.volume = PlayerPrefs.GetFloat("vfxPreference");

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
        if(waitTimer > 0) {
            waitTimer -= Time.deltaTime;
            return;
        }

        if(!_startPlaying) {
           if(Input.anyKeyDown) {
                _startPlaying = true;
                _bs.isStarted = true;
                _anyKey.SetActive(false);
                _music.Play();
            }
        }
        else if(!_bs.isEnded) 
        {
            _hp -= _hp > .4f ? 0.03f * _music.pitch *Time.deltaTime : 0;
            _hpSlider.value = _hp;
            _lvlPercent.text = System.Convert.ToString(Mathf.Round(BeatScroller.timer / (_bs.lastNote + 3f) * 100)) + "%";
            if(Input.GetKeyDown(KeyCode.Escape)) {
                PausedMenu();
            }
            if(Input.GetKeyDown(KeyCode.R)) {
                _bs.isEnded = true;
                LoadSceneByName("Game");
            }
        }
    }
    
    IEnumerator UnpauseWait(int amount) {
        float timer = 0;
        _pauseTimer.text = amount.ToString();
        _pauseTimer.gameObject.SetActive(true);

        for(int i = 1; i <= amount; ++i ) {
            timer = 0;

            while(timer <= 0.3f) {
                timer += Time.deltaTime;
                _pauseTimer.fontSize = (int)(144 * timer / 0.3f);
                yield return null;
            }
            yield return new WaitForSeconds(0.4f);
            
            while(timer <= 0.6f) {
                timer += Time.deltaTime;
                _pauseTimer.fontSize = (int)(144 * (1 - ((timer - 0.3f) / 0.3f)));
                
            }
            _pauseTimer.text = (amount - i).ToString();
        }

        _pauseTimer.gameObject.SetActive(false);
        _bs.isEnded = false;
        _vfx.Play();
        _music.Play();
    }

    public void PausedMenu() {
        _pauseMenu.SetActive(true);
        _bs.isEnded = true;
        _vfx.Pause();
        _music.Pause();
    }

    public void UnPausedMenu() {
        _pauseMenu.SetActive(false);
        StartCoroutine(UnpauseWait(3));
    }

    public void NoteHit() {

        if(_vfx != null) {
            _vfx.Stop();
            _vfx.PlayOneShot(_hit);
        }


        ++comboCount;
        AddHp(.04f);

        ScoreTextChange();

        if(maxCombo < comboCount) 
            maxCombo = comboCount;
    }

    public void PerfectHit() {
        Debug.Log("Perfect hit!");

        hitText.color = Color.green;
        hitText.text = locale[0];
        currentScore += (int)(_scorePerPerfectHit * (1 + comboCount / 100f) * _modsMul);
        ++_perfectCount;
        NoteHit();
    }

    public void NormalHit() {
        Debug.Log("Normal hit!");

        hitText.color = Color.yellow;
        hitText.text = locale[2];
        currentScore += (int)(_scorePerHit * (1 + comboCount / 100f) * _modsMul);
        ++_normalCount;
        NoteHit();
    }

    public void GoodHit() {
        Debug.Log("Good hit!");

        hitText.color = Color.blue;
        hitText.text = locale[1];
        currentScore += (int)(_scorePerGoodHit * (1 + comboCount / 100f) * _modsMul);
        ++_goodCount;
        NoteHit();
    }

    public void NoteMissed() {
        if(godMod)
            return;
            
        Debug.Log("Missed!");
        hitText.color = Color.red;
        hitText.text = locale[3];
        
        if(_vfx != null) {
        _vfx.Stop();
        _vfx.PlayOneShot(_miss);
        }


        comboCount = 0;
        ++_missCount;

        AddHp(-.07f);

        ScoreTextChange();
    }

    private void ScoreTextChange() {
        scoreText.text = currentScore.ToString();
        comboText.text = comboCount.ToString() + "x";
    }

    public void ShowResult() {
        resultObject.gameObject.SetActive(true);
        _accuracy = (_normalCount / 6f + _goodCount / 2f + _perfectCount) / _bs.noteCount;

        resultScoreText.text =  $"{new LocalizedString("StringTable", "Score").GetLocalizedString()}: {currentScore}" + 
        $"\n{new LocalizedString("StringTable", "Max combo").GetLocalizedString()}: {maxCombo}" +
        $"\n{new LocalizedString("StringTable", "Accuracy").GetLocalizedString()}: {Mathf.Round(_accuracy * 100)}%";

        resultHitText.text = $"{new LocalizedString("StringTable", "Perfect").GetLocalizedString()}: {_perfectCount}" + 
        $"\n{new LocalizedString("StringTable", "Good").GetLocalizedString()}: {_goodCount}" + 
        $"\n{new LocalizedString("StringTable", "Normal").GetLocalizedString()}: {_normalCount}" + 
        $"\n{new LocalizedString("StringTable", "Miss").GetLocalizedString()}: {_missCount}";

        levelRank.text = _accuracy == 1 ? "SS" :
        (_accuracy > .9f && _missCount == 0 ? "S" :
        (_accuracy > .85f ? "A" : (_accuracy > .75f ? "B" : (_accuracy > .6f ? "C" : "D"))));
        
        levelRank.color = LevelChoose.GetRankColor(levelRank.text);

        if(!PlayerPrefs.HasKey("Player") || PlayerPrefs.GetString("Player") == String.Empty)
            return;

        string recordScore = AppDataBase.ExecuteQueryWithAnswer($"SELECT Score FROM Records WHERE Username = '{PlayerPrefs.GetString("Player")}' AND id = {PlayerPrefs.GetInt("Level")}");
        Debug.Log(recordScore);

        if(recordScore != null) {
            if(System.Convert.ToUInt32(recordScore) <= currentScore)
                AppDataBase.ExecuteQueryWithoutAnswer($"UPDATE Records SET Score = {currentScore}, Perfect = {_perfectCount}, Good = {_goodCount}, Okay = {_normalCount}, Miss = {_missCount}, Rank ='{levelRank.text}' WHERE id = {PlayerPrefs.GetInt("Level")} AND Username = '{PlayerPrefs.GetString("Player")}'");
        }
        else
            AppDataBase.ExecuteQueryWithoutAnswer($"INSERT INTO Records VALUES ({PlayerPrefs.GetInt("Level")}, '{PlayerPrefs.GetString("Player")}', {currentScore}, {_perfectCount}, {_goodCount}, {_normalCount}, {_missCount}, '{levelRank.text}')");
    }

    private string GetMusicPath()
    {
        return Application.streamingAssetsPath + "/levels/"+ PlayerPrefs.GetInt("Level");
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
    }

    public void LoadSceneByName(string sceneName) {
        StartCoroutine(MainMenu.FadeIn(0.5f));
        StartCoroutine(MainMenu.SceneLoadDelay(sceneName, 0.5f));
    }

    public void AddHp(float num) {
        if(godMod)
            return;

        _hp += num;

        if(_hp > 1) 
            _hp = 1;
        else if(_hp <= 0) {
            _hp = 0;
            _bs.isEnded = true;
            
            _music.Pause();
            _loseMenu.SetActive(true);
        }
    }
}
