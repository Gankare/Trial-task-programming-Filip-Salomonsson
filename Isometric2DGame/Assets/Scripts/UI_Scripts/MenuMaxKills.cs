using UnityEngine;
using TMPro;

public class MenuMaxKills : MonoBehaviour
{
    public TMP_Text killRecord;
    private const string MaxKillsKey = "MaxKills";

    void Start()
    {
        if (!PlayerPrefs.HasKey(MaxKillsKey))
        {
            killRecord.text = "HighScore: " + 0.ToString();
        }
        else
        {
            killRecord.text = "HighScore: " + PlayerPrefs.GetInt(MaxKillsKey).ToString();
        }
    }
}
