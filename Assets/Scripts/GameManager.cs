using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource _music;
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
    private int _scorePerPerfectHit = 300;
    [SerializeField]
    public int comboCount = 0;
    [SerializeField]
    public Text scoreText;

    public static GameManager instance;

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
    }

    public void NormalHit() {
        Debug.Log("Normal hit!");
        currentScore += (int)(_scorePerHit * (1 + comboCount / 100f));
        NoteHit();
    }

    public void GoodHit() {
        Debug.Log("Good hit!");
        currentScore += (int)(_scorePerGoodHit * (1 + comboCount / 100f));
        NoteHit();
    }
    public void PerfectHit() {
        Debug.Log("Perfect hit!");
         currentScore += (int)(_scorePerPerfectHit * (1 + comboCount / 100f));
        NoteHit();
    }

    public void NoteHit() {
        ++comboCount;
        TextChange();
    }

    public void NoteMissed() {
        comboCount = 0;
        Debug.Log("Missed!");
        TextChange();
    }

    private void TextChange() {
        scoreText.text = "Score: " + currentScore + "\nCombo: " + comboCount;
    }
}
