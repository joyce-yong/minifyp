using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class g_PlayerHealth : MonoBehaviour
{
    public Image[] healthBars;
    public int maxHealth = 6;
    public CanvasGroup damageFlash;
    public float flashDuration = 0.3f;

    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

        if (damageFlash != null)
        {
            damageFlash.alpha = 0f;
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        UpdateHealthUI();
        PlayDamageEffect();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        for (int i = 0; i < healthBars.Length; i++)
        {
            if (healthBars[i] != null)
            {
                bool shouldShow = i < currentHealth;

                if (!shouldShow && healthBars[i].enabled)
                {
                    healthBars[i].transform.DOKill();
                    healthBars[i].DOKill();
                    healthBars[i].transform.DOPunchScale(Vector3.one * 0.5f, 0.3f, 5, 0.5f);
                    Image bar = healthBars[i];
                    healthBars[i].DOFade(0f, 0.3f).OnComplete(() => {
                        if (bar != null) bar.enabled = false;
                    });
                }
                else if (shouldShow && !healthBars[i].enabled)
                {
                    healthBars[i].DOKill();
                    healthBars[i].enabled = true;
                    healthBars[i].DOFade(1f, 0.2f);
                }
            }
        }
    }

    void PlayDamageEffect()
    {
        if (damageFlash != null)
        {
            damageFlash.DOKill();
            damageFlash.alpha = 0.8f;
            damageFlash.DOFade(0f, flashDuration);
        }

        transform.DOKill();
        transform.DOShakePosition(0.3f, 0.3f, 20, 90f, false, true);
    }

    void Die()
    {
        Debug.Log("Player Died");
    }

    void OnDestroy()
    {
        if (damageFlash != null) damageFlash.DOKill();
        transform.DOKill();
        foreach (var bar in healthBars)
        {
            if (bar != null)
            {
                bar.DOKill();
                bar.transform.DOKill();
            }
        }
    }
}
