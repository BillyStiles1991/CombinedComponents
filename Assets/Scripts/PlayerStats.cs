using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private int level = 1;

    [Header("EXP Growth Settings")]
    [SerializeField] private int baseExpRequirement = 20;       // EXP for level 1 -> 2
    [SerializeField] private float baseGrowthPerLevel = 1.80f;   // gentle starting curve

    [Tooltip("Bonus added to growth at the first 5-level tier (levels 6–10).")]
    [SerializeField] private float firstTierBonus = 0.1f;

    [Tooltip("How much stronger each new 5-level tier bonus is compared to the previous one. >1 = accelerating tiers.")]
    [SerializeField] private float tierBonusMultiplier = 0.2f;

    [Space]
    public int currentHealth;
    public int maxHealth;

    [Space]
    public int currentMagic;
    public int maxMagic;

    [Space]
    public int currentExp;
    public int maxExp;

    [Space]
    public int currentStatPoints;

    [Space]
    public int strength;
    public int speed;
    public int stamina;
    public int dexterity;
    public int agility;

    public Slider healthBar;
    public Slider magicBar;
    public Slider expBar;

    public TextMeshProUGUI healthSliderDisplay;
    public TextMeshProUGUI magicSliderDisplay;
    public TextMeshProUGUI expSliderDisplay;
    [SerializeField] private TextMeshProUGUI levelText;

    public Image levelUpIcon;

    void Start()
    {
        if (levelUpIcon != null)
            levelUpIcon.gameObject.SetActive(false);

        maxExp = CalculateMaxExpForLevel(level);

        if (levelText != null)
            levelText.text = "Lvl : " + level.ToString();
    }

    void Update()
    {
        // Handle level ups (with EXP carry-over)
        bool leveledUp = false;

        while (currentExp >= maxExp)
        {
            // Use up the EXP needed for this level, keep the overflow
            currentExp -= maxExp;

            level += 1;
            currentStatPoints += 5;

            // Recalculate requirement for the *next* level
            maxExp = CalculateMaxExpForLevel(level);

            leveledUp = true;
        }

        // Only trigger visuals / text if we actually leveled up
        if (leveledUp)
        {
            if (levelUpIcon != null)
            {
                // make sure icon is fully visible first
                Color c = levelUpIcon.color;
                c.a = 1f;
                levelUpIcon.color = c;
                levelUpIcon.gameObject.SetActive(true);
            }

            if (levelText != null)
                levelText.text = "Lvl : " + level.ToString();

            StartCoroutine(FadeOutLevelUpIcon());
        }

        // Update UI sliders afterwards so they reflect new level / exp
        ChangeSliderUI();
    }



    /// Curved EXP:
    /// - baseGrowthPerLevel handles early smooth curve
    /// - every 5 levels is a "tier"
    /// - each tier's bonus grows faster than the last (geometric series)

    private int CalculateMaxExpForLevel(int lvl)
    {
        if (lvl < 1) lvl = 1;

        // Tier 0 = levels 1–5, Tier 1 = 6–10, Tier 2 = 11–15, etc.
        int tier = (lvl - 1) / 5;

        float tierBonus = 0f;

        if (tier > 0)
        {
            // We want:
            //  Tier 1 bonus: firstTierBonus
            //  Tier 2 bonus: firstTierBonus * tierBonusMultiplier
            //  Tier 3 bonus: firstTierBonus * tierBonusMultiplier^2
            //  ...
            // Total bonus up to current tier (geometric series):
            //
            //  total = firstTierBonus * (multiplier^tier - 1) / (multiplier - 1)
            //
            // If multiplier ≈ 1, fall back to linear to avoid division by zero.
            if (Mathf.Approximately(tierBonusMultiplier, 1f))
            {
                // degenerate case: behaves like linear tiers
                tierBonus = tier * firstTierBonus;
            }
            else
            {
                tierBonus = firstTierBonus * (Mathf.Pow(tierBonusMultiplier, tier) - 1f) / (tierBonusMultiplier - 1f);
            }
        }

        float growthPerLevel = baseGrowthPerLevel + tierBonus;

        // Exponential-style scaling using the final growth factor
        float requiredExp = baseExpRequirement * Mathf.Pow(growthPerLevel, lvl - 1);

        return Mathf.RoundToInt(requiredExp);
    }

    public void ChangeSliderUI()
    {
        healthBar.value = currentHealth;
        magicBar.value = currentMagic;
        expBar.value = currentExp;

        healthBar.maxValue = maxHealth;
        magicBar.maxValue = maxMagic;
        expBar.maxValue = maxExp;

        healthSliderDisplay.text = currentHealth + " / " + maxHealth;
        magicSliderDisplay.text = currentMagic + " /  " + maxMagic;
        expSliderDisplay.text = currentExp + " / " + maxExp;
    }

    private IEnumerator FadeOutLevelUpIcon()
    {
        //Fades Level Up Icon

        if (levelUpIcon == null)
            yield break;

        float duration = 3f;
        float elapsed = 0f;

        Color startColor = levelUpIcon.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            float newAlpha = Mathf.Lerp(1f, 0f, t);
            levelUpIcon.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);

            yield return null;
        }

        levelUpIcon.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        levelUpIcon.gameObject.SetActive(false);
    }
}
