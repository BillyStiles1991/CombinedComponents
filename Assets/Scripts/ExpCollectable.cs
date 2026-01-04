using UnityEngine;

public class ExpCollectable : MonoBehaviour
{
    public GameObject player;

    [Header("Movement")]
    public float timer;
    public bool moveToPlayer;
    public float speed = 5f;
    public float attractionRange = 5f; // how close the player needs to be
    public Rigidbody2D rb;

    [Header("EXP Settings")]
    public int expValue = 1; // set this per orb / prefab

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // Delay before orb can start homing
        if (!moveToPlayer)
        {
            if (timer < 1f)
            {
                timer += Time.fixedDeltaTime;
            }
            else
            {
                moveToPlayer = true;
                rb.gravityScale = 0;
            }
        }

        if (moveToPlayer && player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            if (distanceToPlayer <= attractionRange)
            {
                Vector2 direction = (player.transform.position - transform.position).normalized;
                rb.linearVelocity = direction * speed; // or rb.velocity if you're using older Unity
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerStats>().currentExp += expValue;
            Destroy(gameObject);
        }
    }

    void Update()
    {
    }
}
