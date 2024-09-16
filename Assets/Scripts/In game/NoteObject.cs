using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class NoteObject : MonoBehaviour
{
    [SerializeField]
    public GameObject explosionParticle;
    [SerializeField]
    public KeyCode _keyPress;

    [SerializeField]
    private bool _canBePressed = false;
    [SerializeField]
    private bool _obtained = false;
    [SerializeField]
    public static float pianoPos = -3.5f;

    private void Update() {
        if(Input.GetKeyDown(_keyPress)) {
            if(_canBePressed) {
                _obtained = true;
                GameObject buff = Instantiate(explosionParticle, transform.position, Quaternion.identity);
                Destroy(buff, 1f);
                Destroy(gameObject);
                if(Math.Abs(transform.position.y - pianoPos) > .36f) {
                    GameManager.instance.NormalHit();
                }
                else if(Math.Abs(transform.position.y - pianoPos) > .2f) {
                    GameManager.instance.GoodHit();
                }
                else {
                    GameManager.instance.PerfectHit();
                }
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
                Destroy(gameObject, 3f);
            }
        }
    }
}
