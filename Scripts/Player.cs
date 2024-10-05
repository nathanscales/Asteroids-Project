using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private AudioSource source;

    public Sprite stationary, thrust, turnLeft, turnRight, turnLeftThrust, turnRightThrust;
    public Projectile projectile;
    public AudioClip thruster, shield, empowerShot;
    public TMP_Text textPowerups;

    public float thrustSpeed = 1;
    public float turnSpeed = 1;

    private bool thrusting, breaking;
    private float turning;
    private bool thrustSound;

    private bool invulnerable;
    private bool shielded;
    private int empoweredShots;
    
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        source = GetComponent<AudioSource>();

        textPowerups.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        // get user's keyboard inputs

        thrusting = Input.GetKey(KeyCode.W);
        breaking = Input.GetKey(KeyCode.S);

        if (Input.GetKey(KeyCode.A)) {
            turning=1.0f;
        } else if (Input.GetKey(KeyCode.D)) {
            turning=-1.0f;
        } else {
            turning=0.0f;
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            Projectile p = Instantiate(this.projectile, this.transform.position, this.transform.rotation);

            if (this.empoweredShots > 0) {
                p.EmpoweredFire(this.transform.up);
                this.empoweredShots--;
            } else {
                p.Fire(this.transform.up);
            }
        }
    }

    private void FixedUpdate() {
        // alter players velocity, drag and sprite based on keyboard inputs

        if(breaking) 
        {
            rb.drag = 5;
            sr.sprite = stationary;
        }
        else if(turning == 0 && !thrusting) 
        {
            source.Stop();
            sr.sprite = stationary;
        }
        else
        {
            rb.drag = 1;

            if(!source.isPlaying) {
                source.PlayOneShot(thruster, 1f);
            }

            if(turning != 0) {
                rb.AddTorque(turning * this.turnSpeed);

                if(turning > 0) {
                    sr.sprite = turnLeft;
                } else {
                    sr.sprite = turnRight;
                }
            }

            if(thrusting) {
                rb.AddForce(this.transform.up * this.thrustSpeed);

                if(turning==0) {
                    sr.sprite = thrust;
                } else if (turning>0) {
                    sr.sprite = turnLeftThrust;
                } else if (turning<0) {
                    sr.sprite = turnRightThrust;
                }
            }
        }

        // update powerup text based on active power ups
        textPowerups.text = "";
        if(this.invulnerable) { 
            textPowerups.text += "Invulnerable\n";
        }

        if(this.shielded) {
            textPowerups.text += "Shielded\n";
        }

        if(this.empoweredShots > 0) {
            textPowerups.text += "Empowered Shot (x" + empoweredShots + ")\n";
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        // when player collides with asteroid without a shield or invulnerability, they die and lose a life
        if (collision.gameObject.tag == "Asteroid" || collision.gameObject.tag == "AlienProjectile") {
            if (this.shielded != true && this.invulnerable != true) {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = 0.0f;

                this.gameObject.SetActive(false);

                FindObjectOfType<Game>().PlayerHit();
            } else if (this.shielded == true && this.invulnerable != true) {
                // shield is lost when player collides with asteroid while it is active
                this.shielded = false;
                sr.color = new Color(255, 255, 255);
            }
        }
    }

    public void SetShielded() {
        // enables the shielded powerup
        source.PlayOneShot(shield, 0.25f);
        this.shielded = true;
        sr.color = new Color(0, 255, 255);
    }

    public void SetEmpoweredShot() {
        // enables the empowered shot powerup
        source.PlayOneShot(empowerShot, 0.5f);
        this.empoweredShots = 5;
    }

    public void SetInvulnerable(float duration) {
        // enables the invulnerable effect
        // this triggers after a player respawns
        this.invulnerable = true;
        sr.color = new Color(1f, 1f, 1f, 0.25f);
        Invoke("RemoveInvulnerable", duration);
    }

    private void RemoveInvulnerable() {
        // removes invulnerable effect
        this.invulnerable = false;
        sr.color = new Color(1f, 1f, 1f, 1f);
    }
}
