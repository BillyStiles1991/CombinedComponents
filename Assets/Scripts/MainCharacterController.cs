using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class MainCharacterController : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float jumpForce = 7f;
    public float acceleration = 15f;
    public float deceleration = 10f;
    public float knockbackForce = 5f; // Knockback force when hit
    public float invincibilityDuration = 0.5f; // Duration of invincibility after taking damage
    public Vector2 knockbackDistance = new Vector2(3f, 2f); // Fixed knockback distance
    

    public Transform spawnPoint; // Spawn point for the player
    public Transform groundCheck; // Ground check position
    public float groundCheckRadius = 0.2f; // Radius for ground check
    public LayerMask groundLayer; // Ground layer

    public GameObject attackHitbox; // Melee attack hitbox
 

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded;
    private bool isFacingRight = true;
    private bool isAttacking = false;
    private bool isInvincible = false;
  

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        
    }



    private void Update()
    {
       

        // Input and movement handling
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        Debug.Log($"Input: {horizontalInput}, Grounded: {isGrounded}, VelX: {rb.linearVelocity.x}");


        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if (isGrounded && !isAttacking)
        {
            Move(horizontalInput);
        }

        // Flip character based on direction
        if (horizontalInput < 0 && isFacingRight || horizontalInput > 0 && !isFacingRight)
        {
            Flip();
        }

        /// Attack input
        if (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Fire1"))
   
            {
               Attack();
            }
        

     

       
    }

    private void Move(float horizontalInput)
    {
        float targetSpeed = horizontalInput * moveSpeed;
        rb.linearVelocity = new Vector2(Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, acceleration * Time.deltaTime), rb.linearVelocity.y);
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        
    }

    void Attack()
    {
        if (isAttacking) return;
        isAttacking = true;

        attackHitbox.SetActive(true);

        // End the attack after a short time
        Invoke(nameof(DisableAttack), 0.3f);   
    }

    private void DisableAttack()
    {
        attackHitbox.SetActive(false);
        isAttacking = false;
    }


    private void Flip()
    {
        isFacingRight = !isFacingRight;
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    public void TakeDamage(Collider2D enemyCollider)
    {
        
        
            // Apply knockback and invincibility
            Vector2 knockbackDirection = (transform.position - enemyCollider.transform.position).normalized;
            rb.linearVelocity = new Vector2(knockbackDirection.x * knockbackDistance.x, knockbackDistance.y);
         
        
    }

    


    
}
