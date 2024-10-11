using System;
using System.Collections;
using UnityEngine;

public class NoteObject : MonoBehaviour
{
    [SerializeField]
    public GameObject explosionParticle;

    [SerializeField]
    private bool _isFading = false, isClicked = false;
    [SerializeField]
    public static float pianoPos = -3.5f;
    [SerializeField]
    public float hitTime;

    private void Start() {
        _isFading = PlayerPrefs.GetInt("Invisible") == 0;
    }

    private void Update() {
        if(GameManager.instance._bs.isEnded)
            return;

        if(BeatScroller.timer >= hitTime - 0.6f && !_isFading) {
            _isFading = true;
            StartCoroutine(fadeOut());
        }
    }

    public void Hit(BlockClick b) {
        GameObject buff = Instantiate(explosionParticle, transform.position, Quaternion.identity);
        isClicked = true;
        if(Math.Abs(transform.position.y - pianoPos) > .4f) 
            GameManager.instance.NormalHit();
        else if(Math.Abs(transform.position.y - pianoPos) > .25f) 
            GameManager.instance.GoodHit();
        else 
            GameManager.instance.PerfectHit();

        b.notes.Remove(this);
        Destroy(buff, 1f);
        Destroy(gameObject);        
    }

    IEnumerator fadeOut() 
    {
        for (float i = 0.5f; i >= 0; i -= Time.deltaTime)
        {
            while(GameManager.instance._bs.isEnded);
            // set color with i as alpha
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, i * 2);
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "piano") {
            other.gameObject.GetComponent<BlockClick>().notes.Add(this);
        }
    }

      private void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "piano" && !isClicked) {
            GameManager.instance.NoteMissed();
            other.gameObject.GetComponent<BlockClick>().notes.Remove(this);
            Destroy(gameObject, 3f);
        }
    }
}
