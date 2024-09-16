using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.Burst;
using UnityEngine;
using UnityEngine.PlayerLoop;

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
    private float _spawnColdown;

    [SerializeField]
    public bool isStarted = false, isEnded = false;

    [SerializeField]
    private float _timer;

    [SerializeField]
    private int _noteCount = 0;

    void Start() {
        _beatTempo /= 60f;
        _spawnColdown = (6 - NoteObject.pianoPos) / (_beatTempo * _speedMultipler);
        Debug.Log("Current note count: " + _hitTime.Count);
    }

    void Update() {
        if(_hitTime.Count != _noteCount) {
            if(_hitTime[_noteCount] <= _timer + _spawnColdown)  {
                NoteObject buff = Instantiate(note, new Vector3(-5 + (int)_column[_noteCount] * 2, 6, 0), Quaternion.identity);
                buff._keyPress = GameManager.basicButtons[(int)_column[_noteCount]];
                buff.transform.SetParent(this.transform);
                ++_noteCount;
            }
        }
        if(isStarted && !isEnded) {
            _timer += Time.deltaTime;
            transform.position -= new Vector3(0f, _beatTempo * _speedMultipler * Time.deltaTime , 0f);
            if(_hitTime[_hitTime.Count - 1] <= _timer - 3f) {
                isEnded = true;
                GameManager.instance.ShowResult();
            }
        }
        
    }
}
