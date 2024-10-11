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
    [SerializeField]
    public List<NoteObject> notes = new List<NoteObject>();

    void Start() {
        _sp = GetComponent<SpriteRenderer>();
        _deffaultImage = GetComponent<SpriteRenderer>().sprite;
    }

    void Update() {
        if(GameManager.waitTimer > 0 || GameManager.instance._bs.isEnded)
            return;

        if(Input.GetKeyDown(_pressKey)) {
            _sp.sprite = _pressedImage;
            if(notes.Count == 0)
                GameManager.instance.AddHp(-0.03f);
            else 
                for(int i = notes.Count - 1; i >= 0; --i)
                    notes[i].Hit(this);
        }
        else if(Input.GetKeyUp(_pressKey)) 
            _sp.sprite = _deffaultImage;
        
    }
}
