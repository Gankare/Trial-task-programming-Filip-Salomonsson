using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class HealthSystem : MonoBehaviour
{
    [Header("Script Info")]
    [TextArea]
    [Tooltip("This is just an informational note.")]
    public string info = "This script works for both player and enemy, depending on what controller script is attatched. Update isPlayer acordingly (:";

    public int maxHealth = 100;
    public bool isPlayer;
    public GameObject fadeEffectOnDeath;

    private int currentHealth;
    private SpriteRenderer sr;
    private PlayerAnimationController playerAnim;
    private EnemyAnimationController enemyAnim;
    private bool isDead;
    public bool IsDead => isDead;

    void Awake()
    {
        currentHealth = maxHealth;
        sr = GetComponentInChildren<SpriteRenderer>();
        playerAnim = GetComponent<PlayerAnimationController>();
        enemyAnim = GetComponent<EnemyAnimationController>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        StartCoroutine(FlashRed());

        if (playerAnim) playerAnim.TriggerTakeDamage();
        if (enemyAnim) enemyAnim.TriggerTakeDamage();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator FlashRed()
    {
        if (sr != null)
        {
            Color original = sr.color;
            sr.color = Color.red;
            yield return new WaitForSeconds(0.5f);
            sr.color = original;
        }
    }

    void Die()
    {
        isDead = true;
        if (playerAnim) playerAnim.TriggerDeath();
        if (enemyAnim) enemyAnim.TriggerDeath();

        if (isPlayer)
        {
            if (fadeEffectOnDeath) fadeEffectOnDeath.SetActive(true);
            StartCoroutine(ReloadSceneAfterDelay(3f));
        }
        else
        {
            Destroy(gameObject, 1.5f); 
        }
    }

    IEnumerator ReloadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
