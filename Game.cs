using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Game : MonoBehaviour
{
    private AudioSource source;
    public AudioClip newLevel, death, explode;

    public TMP_Text textTitle, textScore, textLives, textLevel, textGameOver, textFinalScore;
    public Image imgControls;
    public Button btnStartGame, btnNewGame, btnMainMenu;

    public Player player;
    public float respawnTime = 3.0f;
    public int lives = 3;
    public int score = 0;

    public Asteroid asteroid;
    public float spawnDistance = 2.5f;
    public int spawnAmount;
    private float minSpeed;
    private float maxSpeed;

    public AI ai;
    public int alienSpawnChance;

    public Pickup pickup;
    public int dropChance;

    public ParticleSystem explosion;

    private int level;

    private void Awake() {
        source = GetComponent<AudioSource>();
    }

    void Start() {
        // add the effect for each button
        btnStartGame.onClick.AddListener(StartGame);
        btnNewGame.onClick.AddListener(StartGame);
        btnMainMenu.onClick.AddListener(MainMenu);

        MainMenu();
    }

    private void MainMenu() {
        // disable unneeded objects
        textScore.enabled = false;
        textLives.enabled = false;
        textLevel.enabled = false;
        textGameOver.enabled = false;
        textFinalScore.enabled = false;
        btnNewGame.gameObject.SetActive(false);
        btnMainMenu.gameObject.SetActive(false);
        this.player.gameObject.SetActive(false);

        // enable menu objects
        textTitle.enabled = true;
        imgControls.enabled = true;
        btnStartGame.gameObject.SetActive(true);
    }

    private void StartGame() {
        DestroyAll("Asteroid");
        DestroyAll("Alien");

        // disable unneeded objects
        textTitle.enabled = false;
        textGameOver.enabled = false;
        textFinalScore.enabled = false;
        imgControls.enabled = false;
        btnNewGame.gameObject.SetActive(false);
        btnMainMenu.gameObject.SetActive(false);
        btnStartGame.gameObject.SetActive(false);

        // enable and reset objects needed for game
        textScore.enabled = true;
        textLives.enabled = true;
        textScore.text = "Score: " + score;
        textLives.text = "Lives: " + lives;

        // reset game variables and activate player
        this.level = 1;
        this.score = 0;
        this.lives = 3;
        this.minSpeed = 9.0f;
        this.maxSpeed = 11.0f;
        this.spawnAmount = 3;
        this.dropChance = 20;
        this.player.transform.position = Vector3.zero;
        this.player.gameObject.SetActive(true);

        // start game loop
        StartCoroutine("GameLoop");
    }

    private void DestroyAll(string tag) 
    {
        // destroy all objects with passed tag
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        foreach(GameObject obj in objects)
        {
            Destroy(obj);
        }
    }

    private IEnumerator GameLoop() {
        // wait until there are no asteroids in frame
        if(GameObject.FindGameObjectsWithTag("Asteroid").Length == 0) {

            // start new level
            textLevel.text = "Level " + level;
            textLevel.enabled = true;
            source.PlayOneShot(newLevel, 0.5f);

            Invoke("DisableLevelText", 3.0f);

            // spawn asteroids equal to the spawn amount
            for(int i=1; i<=spawnAmount; i++) {
                SpawnAsteroid();
            }

            // prepare variables for next level
            this.level++;

            // every 5 levels, increase the number of asteroids that spawn per level
            // each level that isn't a multiple of 5 increases the speed of the asteroids
            if (level % 5 == 0) {
                spawnAmount++;
            } else {
                minSpeed += 0.1f;
                maxSpeed += 0.2f;
            }
        }

        yield return new WaitForSeconds(4.0f);

        // continue to next iteration of loop if player still have lives remaining
        if(lives>0) {
            StartCoroutine("GameLoop");
        }
    }

    private void DisableLevelText() {
        textLevel.enabled = false;
    }

    private void GameOver() {
        textFinalScore.text = "You Scored:\n" + score;

        // disable unneeded objects
        textTitle.enabled = false;
        textScore.enabled = false;
        textLives.enabled = false;
        imgControls.enabled = false;
        btnStartGame.gameObject.SetActive(false);
        
        // enable game over screen objects
        textGameOver.enabled = true;
        textFinalScore.enabled = true;
        btnNewGame.gameObject.SetActive(true);
        btnMainMenu.gameObject.SetActive(true);
    }

    private void SpawnAsteroid() {
        // spawns a new large asteroid 

        // determines initial position of asteroid
        // this will be within a circle at the set spawnDistance
        Vector3 direction = Random.insideUnitCircle.normalized * this.spawnDistance;
        Vector3 position = this.transform.position + direction;

        // determines initial rotation of asteroid
        float variance = Random.Range(-15.0f, 15.0f);
        Quaternion rotation = Quaternion.AngleAxis(variance, Vector3.forward);

        Asteroid a = Instantiate(this.asteroid, position, rotation);
            
        a.SetSize(3);
        a.SetTrajectory(rotation * -direction, minSpeed, maxSpeed);
    }

    public void AsteroidHit(Asteroid hit) {
        // called whenever a projectile hits an asteroid

        int size = hit.GetSize();

        source.PlayOneShot(explode, 0.3f*size);        

        // play an explosion particle effect at the asteroid's location
        this.explosion.transform.position = hit.transform.position;
        this.explosion.Play();

        // rolls for chance for pickup to drop or AI enemy to spawn
        if(size==3) {
            if(Random.Range(0,100) <= dropChance) {
                // pickup object is created and placed at asteroid's location
                Pickup p = Instantiate(this.pickup);
                p.SetEffect();
                p.transform.position = hit.transform.position;

                // drop chance resets to initial chance
                dropChance = 20;
            } else {
                // if pickup doesn't drop, chance to drop increases
                dropChance += 20;
            }

            if(Random.Range(0,100) <= alienSpawnChance) {
                AI a = Instantiate(this.ai);
                a.player = player;
                a.SetDifficulty(level);
                a.transform.position = hit.transform.position;

                alienSpawnChance = 10;
            } else {
                alienSpawnChance += 10;
            }
        }

        // increase score based on size of asteroid hit
        // small = 100, medium = 50, large = 25
        AddScore(100 / (int)Mathf.Pow(2, size-1));
    }

    public void AddScore(int i) {
        this.score += i;
        textScore.text = "Score: " + score;
    }

    public void PlayerHit() {
        // called when player is hit by asteroid

        source.PlayOneShot(death, 0.25f);

        // play explosion particle effect at player's location
        this.explosion.transform.position = this.player.transform.position;
        this.explosion.Play();

        // player loses a life
        this.lives--;
        textLives.text = "Lives: " + lives;

        // if player has lives, they will respawn after respawn time has passed
        // otherwise the game over screen is shown
        if (this.lives <= 0) {
            GameOver();
        } else {
            Invoke(nameof(Respawn), this.respawnTime);
        }
    }

    private void Respawn() {
        // player returns to centre of screen and gains invulnerability for 2.5 seconds
        this.player.transform.position = Vector3.zero;
        this.player.SetInvulnerable(2.5f);
        this.player.gameObject.SetActive(true);
    }

    public void gainLife() {
        // player gains 1 life
        this.lives++;
        textLives.text = "Lives: " + lives;
    }
}
