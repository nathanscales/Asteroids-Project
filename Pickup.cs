using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    private AudioSource source;
    public AudioClip extraLife;

    private int effect;

    private void Awake() {
        source = GetComponent<AudioSource>();
    }

    public void SetEffect() {
        // randomly determine the effect provided by the pickup
        this.effect = Random.Range(1,3);

        // destroy pickup after 25 seconds
        Destroy(this.gameObject, 25.0f);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        // player can touch the pickup to gain its randomly determined effect
        if (collision.gameObject.tag == "Player") {
            if (effect==1) {
                // grants a shield which allows the player to ignore the next collision with an asteroid
                FindObjectOfType<Player>().SetShielded();
            } else if (effect==2) {
                // grants an extra life
                source.PlayOneShot(extraLife, 1f);
                FindObjectOfType<Game>().gainLife();
            } else if (effect==3) {
                // causes player's next five shots to pierce through asteroids
                FindObjectOfType<Player>().SetEmpoweredShot();
            }

            Destroy(this.gameObject);
        }
    }
}
