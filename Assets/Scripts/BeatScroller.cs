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
    private NoteObject[] notes;
    [SerializeField]
    private List<float> _hitTime;
    [SerializeField]
    private List<notesSelect> _column;
    [SerializeField]
    private float _spawnColdown;

    [SerializeField]
    public bool isStarted = false;

    [SerializeField]
    private float _timer;

    [SerializeField]
    private int _noteCount = 0;

    void Start() {
        _beatTempo /= 60f;
        _spawnColdown = (6 - NoteObject.pianoPos) / (_beatTempo * _speedMultipler);
    }

    void Update() {
        if(_hitTime[_noteCount] <= _timer + _spawnColdown && _hitTime.Count - 1 != _noteCount)  {
            NoteObject buff = Instantiate(notes[(int)_column[_noteCount]], new Vector3(notes[(int)_column[_noteCount]].transform.position.x, 6, 0), Quaternion.identity);
            buff.transform.SetParent(this.transform);
            ++_noteCount;
        }
        if(isStarted) {
            _timer += Time.deltaTime;
            transform.position -= new Vector3(0f, _beatTempo * _speedMultipler * Time.deltaTime , 0f);
        }
    }
}
