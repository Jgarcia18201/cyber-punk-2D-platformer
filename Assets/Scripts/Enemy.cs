using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody2D droneRb;
    public Animator enemyAnim;
    public int maxHealth = 100;
    int currentHealth; 
    [SerializeField] public float moveSpeed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        droneRb = GetComponent<Rigidbody2D>();
    }

        void Update()
    {
        droneRb.velocity = new Vector2 (-moveSpeed, 0f);
    }

        public void TakeDamage(int damage){
        moveSpeed = 0f;
        currentHealth -= damage;
        enemyAnim.SetTrigger("Hurt");

        //play hurt animation

        if(currentHealth <= 0){
            Die();
        }
    }

    void OnTriggerExit2D(Collider2D other){
        moveSpeed = -moveSpeed;
        FlipEnemyFacing();
    }

    void FlipEnemyFacing(){
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y * 1); 
        
        //(-(Mathf.Sign(droneRb.velocity.x)), 3f);
    }

    void Die(){
        enemyAnim.SetBool("isDead", true);
        //Disable enemy
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
        moveSpeed = 0f;
        Object.Destroy(gameObject, 2.0f);
    }
}
