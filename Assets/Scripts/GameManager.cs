using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource _music;    
    [SerializeField]
    private AudioSource _vfx;
    [SerializeField]
    private AudioClip _hit, _miss;

    [SerializeField]
    private BeatScroller _bs;
    [SerializeField]
    private bool _startPlaying;

    [Space(50)]
    [SerializeField]
    public int currentScore = 0;
    [SerializeField]
    private int _scorePerHit = 50;
    [SerializeField]
    private int _scorePerGoodHit = 100;
    [SerializeField]
    private int _scorePerPerfectHit = 330;

    [Space(50)]
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

    [Space(50)]
    [SerializeField]
    public Text scoreText;
    [SerializeField]
    public Text hitText;

    [Space(50)]
    [SerializeField]
    public GameObject resultObject;
    [SerializeField]
    public Text resultScoreText;
    [SerializeField]
    public Text resultHitText;

    public static GameManager instance;
    public static KeyCode[] basicButtons = {KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K, KeyCode.L};

    void Start()
    {
        instance = this;
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
        if(!_bs.isEnded) {
            currentScore += (int)(Time.deltaTime * 1000f);
            ScoreTextChange();
        }
    }

    public void NormalHit() {
        Debug.Log("Normal hit!");
        hitText.color = Color.yellow;
        hitText.text = "NORMAL";
        currentScore += (int)(_scorePerHit * (1 + comboCount / 100f));
        ++_normalCount;
        NoteHit();
    }

    public void GoodHit() {
        Debug.Log("Good hit!");
        hitText.color = new Color(109, 147, 21, 255);
        hitText.text = "GOOD";
        currentScore += (int)(_scorePerGoodHit * (1 + comboCount / 100f));
        ++_goodCount;
        NoteHit();
    }
    public void PerfectHit() {
        Debug.Log("Perfect hit!");
        hitText.color = Color.green;
        hitText.text = "PERFECT";
        currentScore += (int)(_scorePerPerfectHit * (1 + comboCount / 100f));
        ++_perfectCount;
        NoteHit();
    }

    public void NoteHit() {
        _vfx.PlayOneShot(_hit);
        ++comboCount;
        if(maxCombo < comboCount) {
            maxCombo = comboCount;
        }
    }

    public void NoteMissed() {
        Debug.Log("Missed!");
        hitText.color = Color.red;
        hitText.text = "MISS";
        _vfx.PlayOneShot(_miss);
        comboCount = 0;
        ++_missCount;
    }

    private void ScoreTextChange() {
        scoreText.text = "Score: " + currentScore + "\nCombo: " + comboCount;
    }

    public void ShowResult() {
        resultObject.gameObject.SetActive(true);
        resultScoreText.text = "Score: " + currentScore + "\nMax combo: " + maxCombo;
        resultHitText.text = "Perfect hits:" + _perfectCount + "\nGood hits:" + _goodCount + "\nNormal hits:" + _normalCount + "\nMisses:" + _missCount;
    }
}
