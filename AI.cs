using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{

    public Player player;
    public Projectile projectile;

    public int startingHealth;
    public int lowHealthThreshold;
    public float chasingRange;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Node rootNode = null;

    private int currentHealth;
    private Vector3 bestCoverSpot = Vector3.zero;
    private Vector3 target;
    private int availableShots = 0;
    private int difficulty;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        
        currentHealth = startingHealth;

        ConstructBehaviourTree();

        StartCoroutine("Fire");
    }

    private void ConstructBehaviourTree() {
        IsCoverAvailableNode isCoverAvailableNode = new IsCoverAvailableNode(player, this);
        GotoCoverNode gotoCoverNode = new GotoCoverNode(this);
        HealthNode healthNode = new HealthNode(this, lowHealthThreshold);
        IsCoveredNode isCoveredNode = new IsCoveredNode(player, transform);
        ChaseNode chaseNode = new ChaseNode(player, this);
        RangeNode chaseRangeNode = new RangeNode(chasingRange, player, transform);
        AvoidObstacleNode avoidObstacleNode = new AvoidObstacleNode(this, GameObject.FindGameObjectsWithTag("Boundary"));

        Sequence chaseSequence = new Sequence(new List<Node> {chaseRangeNode, chaseNode});

        Sequence gotoCoverSequence = new Sequence(new List<Node> {isCoverAvailableNode, gotoCoverNode});
        Selector findCoverSelector = new Selector(new List<Node> {gotoCoverSequence, chaseSequence});

        Selector tryToTakeCoverSelector = new Selector(new List<Node> {isCoveredNode, findCoverSelector});
        Sequence mainCoverSequence = new Sequence(new List<Node> {healthNode, tryToTakeCoverSelector});

        rootNode = new Selector(new List<Node> {avoidObstacleNode, mainCoverSequence, chaseSequence});
    }

    private void Update() {
        rootNode.Evaluate();
    }

    public void Goto(Vector3 pos) {
        this.target = pos;
        StartCoroutine("Move");
    }

    private IEnumerator Move() {
        rb.AddForce((target - this.transform.position) * 0.05f);

        yield return new WaitForSeconds(0.2f);

        if(Vector3.Distance(this.transform.position, target) > 8.0f) {
            StartCoroutine("Move");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.tag == "Projectile") 
        {
            currentHealth--;
            if(currentHealth==0) {
                FindObjectOfType<Game>().AddScore(100);
                Destroy(this.gameObject);
            }
        }
    }

    private IEnumerator Fire() {
        if (availableShots > 0) 
        {
            Vector3 projectilePos = this.transform.position + new Vector3(0.1f, -0.1f, 0.0f);

            Projectile p = Instantiate(this.projectile, projectilePos, this.transform.rotation);
            p.speed = 40.0f + (1.5f * this.difficulty);
            p.Fire(player.transform.position - this.transform.position);

            availableShots--;

            yield return new WaitForSeconds(0.2f);
        } else {
            yield return new WaitForSeconds(Mathf.Max(3.0f - (this.difficulty * 0.05f), 1.5f));
            availableShots = 3;
        }

        StartCoroutine("Fire");
    }

    public void SetDifficulty(int d) {
        this.difficulty = d;
    }

    public void SetColour(Color c) {
        sr.color = c;
    }

    public int GetCurrentHealth() {
        return currentHealth;
    }

    public void SetBestCoverSpot(Vector3 bestCoverSpot) {
        this.bestCoverSpot = bestCoverSpot;
    }

    public Vector3 GetBestCoverSpot() {
        return bestCoverSpot;
    }
}
