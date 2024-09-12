using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockClick : MonoBehaviour
{
    private SpriteRenderer _sp;
    private Sprite _deffaultImage;
    [SerializeField]
    private Sprite _pressedImage;

    [SerializeField]
    private KeyCode _pressKey;

    void Start() {
        _sp = GetComponent<SpriteRenderer>();
        _deffaultImage = GetComponent<SpriteRenderer>().sprite;
    }

    void Update() {
        if(Input.GetKeyDown(_pressKey)) {
            _sp.sprite = _pressedImage;
        }
        else if(Input.GetKeyUp(_pressKey)) {
            _sp.sprite = _deffaultImage;
        }
    }
}
