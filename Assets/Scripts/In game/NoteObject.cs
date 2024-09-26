using System;
using System.Collections;
using UnityEngine;

public class NoteObject : MonoBehaviour
{
    [SerializeField]
    public GameObject explosionParticle;
    [SerializeField]
    public KeyCode _keyPress;

    [SerializeField]
    private bool _canBePressed = false, _obtained = false, _isFirstTime = true, _isFading = false;
    [SerializeField]
    public static float pianoPos = -3.5f;
    [SerializeField]
    public float hitTime;

    private void Start() {
        _isFading = PlayerPrefs.GetInt("Invisible") == 0;
    }

    private void Update() {
        if(Input.GetKeyDown(_keyPress)) {
            if(_canBePressed) {
                _obtained = true;
                GameObject buff = Instantiate(explosionParticle, transform.position, Quaternion.identity);
                Destroy(buff, 1f);
                Destroy(gameObject);
                if(Math.Abs(transform.position.y - pianoPos) > .35f) {
                    GameManager.instance.NormalHit();
                }
                else if(Math.Abs(transform.position.y - pianoPos) > .2f) {
                    GameManager.instance.GoodHit();
                }
                else {
                    GameManager.instance.PerfectHit();
                }
            }
            else {
                if(transform.position.y - pianoPos > .35f && transform.position.y - pianoPos < 1 && _isFirstTime) {
                    GameManager.instance.NoteMissed();
                    _isFirstTime = false;
                }
            }
        }
        if(BeatScroller.timer >= hitTime - 0.6f && !_isFading) {
            _isFading = true;
            StartCoroutine(fadeOut());
        }
    }

    IEnumerator fadeOut() 
    {
        for (float i = 0.5f; i >= 0; i -= Time.deltaTime)
        {
            // set color with i as alpha
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, i * 2);
            yield return null;
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
