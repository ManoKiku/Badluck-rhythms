using System;
using System.Collections.Generic;
using UnityEngine;

public class BeatScroller : MonoBehaviour
{
    enum notesSelect {one, two, three, four, five, six}


    [SerializeField]
    private float _beatTempo;
    [SerializeField]
    private float _speedMultipler = 1f;

    [SerializeField]
    private NoteObject note;
    [SerializeField]
    private List<float> _hitTime;
    [SerializeField]
    private List<notesSelect> _column;

    [SerializeField]
    public bool isStarted = false, isEnded = false, isSpeed = false;

    [SerializeField]
    public float lastNote;
    [SerializeField]
    static public float timer;

    void Start() {
        timer = 0;
        _beatTempo /= 60f;
        isSpeed = PlayerPrefs.GetInt("Speed") != 0;
        if(isSpeed)
            _speedMultipler *= 1.5f;
        for(int i = 0; i < _hitTime.Count; ++i) {
            if(isSpeed) 
                _hitTime[i] /= 1.5f;
            NoteObject buff = Instantiate(note, new Vector3(-2.5f + (int)_column[i], NoteObject.pianoPos + _beatTempo * _speedMultipler * _hitTime[i], 0), Quaternion.identity);
            buff._keyPress = GameManager.basicButtons[(int)_column[i]];
            buff.transform.SetParent(this.transform);
            buff.hitTime = _hitTime[i];
        }
        lastNote = _hitTime[_hitTime.Count - 1];
        Debug.Log("Current note count: " + _hitTime.Count);
        Debug.Log(Math.Pow(_hitTime[_hitTime.Count - 1] / 60, 0.2f));
        Debug.Log(Math.Round((Math.Pow(_speedMultipler, 0.6) * Math.Sqrt(_beatTempo * 60)  * _hitTime.Count / _hitTime[_hitTime.Count - 1] / 30), 2));
    }

    void Update() {
        if(isStarted && !isEnded) {
            timer += Time.deltaTime;
            transform.position -= new Vector3(0f, _beatTempo * _speedMultipler * Time.deltaTime , 0f);
            if(lastNote <= timer - 3f) {
                isEnded = true;
                GameManager.instance.ShowResult();
            }
        }
        
    }
}
