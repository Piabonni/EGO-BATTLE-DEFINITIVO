using System.Collections;
using System.Collections.Generic;
// PlayerHealth.cs
using UnityEngine;
using UnityEngine.UI;          // ← para manejar la UI

public class PlayerHealth : MonoBehaviour
{
    [Header("Valores de vida")]
    [SerializeField] private int maxHealth = 5;
    private int currentHealth;

    [Header("Referencias a la UI")]
    [SerializeField] private Slider healthSlider;   // barra completa (background+fill)
    [SerializeField] private Image fillImage;      // solo la parte que “se vacía”

    [Header("Opcional")]
    [SerializeField] private Animator animator;     // anima la muerte si quieres

    /*----------- CICLO -----------*/
    private void Awake()
    {
        currentHealth = maxHealth;

        // -- Configura la barra al valor máximo sólo una vez
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    /*----------- API PÚBLICA -----------*/
    public void TakeDamage(int damage)
    {
        // 1. Resta vida y clámala entre 0 y maxHealth
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);

        // 2. Actualiza la UI
        if (healthSlider != null)
            healthSlider.value = currentHealth;

        // 3. Para un color de “vacío / lleno” (opcional)
        if (fillImage != null)
            fillImage.color = Color.Lerp(Color.red, Color.green,
                                         (float)currentHealth / maxHealth);

        // 4. Log y muerte
        Debug.Log($"{gameObject.name} recibió {damage}  ➜  Vida: {currentHealth}");
        if (currentHealth == 0)
            Die();
    }

    public void Heal(int amount)                // por si luego quieres curar
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        if (healthSlider != null)
            healthSlider.value = currentHealth;
    }

    /*----------- MÉTODO PRIVADO -----------*/
    private void Die()
    {
        Debug.Log($"{gameObject.name} murió");

        if (animator != null)
            animator.SetTrigger("Die");         // supone que existe el trigger “Die”

        // Si usas animación, espera al evento AnimationEvent para desactivar:
        // gameObject.SetActive(false);

        // Si NO usas animación:
        Destroy(gameObject);                    // quita al chibi de la escena
    }
}


