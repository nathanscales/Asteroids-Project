using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private AudioSource source;
    
    public AudioClip laser;

    public float speed = 400.0f;
    public float duration = 10.0f;

    private bool empowered;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        source = GetComponent<AudioSource>();
    }

    public void Fire(Vector2 direction) {
        source.PlayOneShot(laser, 0.05f);
        rb.AddForce(direction * this.speed);
        Destroy(this.gameObject, this.duration);
    }
    
    public void EmpoweredFire(Vector2 direction) {
        this.empowered = true;
        this.Fire(direction);
    }

    // called when projectile collides with another object
    public void OnCollisionEnter2D(Collision2D collision) {
        if (this.empowered) {
            Destroy(this.gameObject, 5.0f);
        } else {
            Destroy(this.gameObject);
        }
    }

}
