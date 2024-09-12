using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class NoteObject : MonoBehaviour
{
    [SerializeField]
    private KeyCode _keyPress;

    [SerializeField]
    private bool _canBePressed = false;
    [SerializeField]
    private bool _obtained = false;

    private void Update() {
        if(Input.GetKeyDown(_keyPress)) {
            if(_canBePressed) {
                _obtained = true;
                gameObject.SetActive(false);
                GameManager.instance.NoteHit();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "piano") {
            _canBePressed = true;
        }
    }

      private void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "piano") {
            _canBePressed = false;
            if(!_obtained) {
                GameManager.instance.NoteMissed();
            }
        }
    }
}
