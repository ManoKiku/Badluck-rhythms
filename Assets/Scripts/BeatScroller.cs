using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatScroller : MonoBehaviour
{
    [SerializeField]
    private float _beatTempo;

    [SerializeField]
    public bool isStarted = false;

    void Start() {
        _beatTempo /= 60;
    }

    void Update() {
        if(isStarted) {
            transform.position -= new Vector3(0f, _beatTempo * Time.deltaTime, 0f);
        }
    }
}
