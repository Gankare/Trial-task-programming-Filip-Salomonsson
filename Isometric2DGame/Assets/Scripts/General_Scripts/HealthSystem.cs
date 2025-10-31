using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class HealthSystem : MonoBehaviour
{
    [Header("Script Info")]
    [TextArea]
    [Tooltip("This is just an informational note.")]
    public string info = "This script works for both player and enemy, depending on what controller script is attatched. Update isPlayer acordingly (:";

    public HealthBarUI playerHealthBar;

    public int maxHealth = 100;
    public bool isPlayer;
    public GameObject fadeEffectOnDeath;
    public int currentHealth;
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

        if (isPlayer && playerHealthBar != null)
        {
            playerHealthBar.SetHealth(1f);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        if (playerHealthBar != null && isPlayer)
            playerHealthBar.SetHealth((float)currentHealth / maxHealth);

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
            sr.color = Color.red;
            yield return new WaitForSeconds(0.5f);
            sr.color = new Color(1f, 1f, 1f, 1f);
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
