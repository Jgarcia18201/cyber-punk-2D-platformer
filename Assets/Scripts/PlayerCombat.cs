using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Animator myAnimator;

    public Transform attackPoint;
    public LayerMask Enemies;

    public float attackRange = 0.5f;
    public int attackDamage = 40;
    public float attackRate = 2f;
    float nextAttackTime = 0f;
    // Update is called once per frame
    void Update()
    {
        if(Time.time >= nextAttackTime){
            if(Input.GetMouseButtonDown(0)){
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }      
    }

    void Attack(){
        //play an attack anim
        myAnimator.SetTrigger("Attack");
        //detect enemies in range
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, Enemies);
        //apply damage
        foreach(Collider2D enemy in hitEnemies){
             enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
         }
    }

    void OnDrawGizmosSelected() {
        if(attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}