using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private CircleCollider2D cc;

    private int size;
    private float scale;
    private float minSpeed;
    private float maxSpeed;

    public Sprite[] small;
    public Sprite[] medium;
    public Sprite[] large;

    private int frames;
    private int currentFrame;
    private float frameSpeed;

    public bool seen = false;

    private void Awake() {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CircleCollider2D>();

    }

    private void FixedUpdate() {
        // asteroid objects are destroyed when they either:
        //      have been seen before and are no longer in frame
        //      have never been seen and are not large
        if(sr.isVisible) {
            this.seen = true;
        } if ((!sr.isVisible && this.seen) || (!this.seen && this.size<3)) {
            Destroy(this.gameObject);
        }
    }

    public void SetSize(int size) {
        this.size = size;
        rb.mass = size;

        // sets asteroid sprite, scale and collision area based on its size

        if (size==1) 
        {
            frames = 59;

            scale = Random.Range(0.6f, 0.75f);

            cc.radius = 0.073f;
            cc.offset = new Vector2(0.006f, 0f);
        } 
        else if (size==2) 
        {
            frames = 59;

            scale = Random.Range(0.5f, 0.75f);

            cc.radius = 0.18f;
            cc.offset = new Vector2(0.02f, -0.07f);
        } 
        else if (size==3)   
        {
            frames = 120;

            scale = Random.Range(0.5f, 0.75f);

            cc.radius = 0.525f;
            cc.offset = new Vector2(0.06f, -0.04f);
        }

        // sets asteroid object scale based on randomised scale
        this.transform.localScale = Vector3.one * scale;

        // randomly chooses a starting frame for the sprite animation
        currentFrame = Random.Range(0, frames);

        // set sprite animation speed
        frameSpeed = (frames+1)/3000.0f;
        
        // starts sprite animation
        StartCoroutine("SwitchSprite");
    }

    public int GetSize() {
        return this.size;
    }

    public void SetTrajectory(Vector2 direction, float minSpeed, float maxSpeed) {
        this.minSpeed = minSpeed;
        this.maxSpeed = maxSpeed;

        rb.AddForce(direction * Random.Range(this.minSpeed, this.maxSpeed));
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.tag == "Projectile") 
        {
            // if asteroid isn't small, it breaks into smaller asteroids
            if (size>1) {
                Break(size-1, 3);
            }

            FindObjectOfType<Game>().AsteroidHit(this);
            Destroy(this.gameObject);
        }
    }

    private void Break(int breakSize, int count) {
        // creates the smaller asteroids that this asteroid breaks into
        for(int i=1; i<=count; i++) {
            float speed = Random.Range(this.minSpeed, this.maxSpeed);
            Vector2 position = this.transform.position;
            position += Random.insideUnitCircle * 0.3f;

            Asteroid a = Instantiate(this, position, this.transform.rotation);

            a.SetSize(breakSize);
            a.SetTrajectory(Random.insideUnitCircle.normalized, minSpeed*1.5f, maxSpeed*1.5f);
        }
    }

    private IEnumerator SwitchSprite() {
        // cycles to next frame of the sprite animation

        if (size==1) {
            sr.sprite = small[currentFrame];
        } else if (size==2) {
            sr.sprite = medium[currentFrame];
        } else if (size==3) {
            sr.sprite = large[currentFrame];
        }

        if(currentFrame < frames) {
            currentFrame++;
        } else {
            currentFrame=0;
        }

        // wait for the time per frame of the animation
        yield return new WaitForSeconds(frameSpeed);

        // advance to next frame
        StartCoroutine("SwitchSprite");
    }
}
