using UnityEngine;
using TMPro; 

public class KillCounter : MonoBehaviour
{
    public static KillCounter Instance { get; private set; }

    [Header("UI")]
    public TMP_Text killCountText; 

    [Header("Kills")]
    public int currentKills = 0;
    private int maxKills;

    private const string MaxKillsKey = "MaxKills";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (!PlayerPrefs.HasKey(MaxKillsKey))
        {
            maxKills = 0;
            PlayerPrefs.SetInt(MaxKillsKey, maxKills);
        }
        else
        {
            maxKills = PlayerPrefs.GetInt(MaxKillsKey);
        }

        UpdateKillUI();
    }
    private void Start()
    {
        ResetKills();
    }

    public void AddKill()
    {
        currentKills++;
        UpdateKillUI();

        if (currentKills > maxKills)
        {
            maxKills = currentKills;
            PlayerPrefs.SetInt(MaxKillsKey, maxKills);
            PlayerPrefs.Save();
        }
    }

    private void UpdateKillUI()
    {
        if (killCountText != null)
            killCountText.text = $"Kills: {currentKills}";
    }

    public void ResetKills()
    {
        currentKills = 0;
        UpdateKillUI();
    }
    public int GetMaxKills() => maxKills;
}
