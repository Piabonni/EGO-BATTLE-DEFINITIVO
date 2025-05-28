using System.Collections;
using System.Collections.Generic;
// PlayerCombat.cs
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    /* ---------- Inspector ---------- */
    [Header("Entrada")]
    [SerializeField] private KeyCode attackKey = KeyCode.L;

    [Header("Golpe")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private LayerMask enemyLayers;      // ← marca “Player”

    [Header("Cadencia")]
    [SerializeField] private float attacksPerSecond = 2f;

    /* ---------- Internos ---------- */
    private float nextAttackTime;
    private Animator anim;

    private void Awake() => anim = GetComponent<Animator>();

    private void Update()
    {
        if (Time.time >= nextAttackTime && Input.GetKeyDown(attackKey))
        {
            Attack();
            nextAttackTime = Time.time + 1f / attacksPerSecond;
        }
    }

    private void Attack()
    {
        /* 1. Animación */
        if (anim) anim.SetTrigger("Attack");

        /* 2. Detectar enemigos en 3D */
        Collider[] hit = Physics.OverlapSphere(
                            attackPoint.position,
                            attackRange,
                            enemyLayers);

        /* 3. Aplicar daño */
        foreach (Collider enemy in hit)
        {
            if (enemy.transform == transform) continue;          // evita autodaño

            PlayerHealth hp = enemy.GetComponent<PlayerHealth>();
            if (hp) hp.TakeDamage(attackDamage);
        }
    }

    /* Gizmo para ver el rango en la Escena */
    private void OnDrawGizmosSelected()
    {
        if (!attackPoint) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
