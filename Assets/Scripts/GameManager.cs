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
    private int _scorePerHit = 100;
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

    public void NoteHit() {
        Debug.Log("Hit in time!");
        currentScore += (int)(_scorePerHit * (1 + comboCount / 100f));
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
