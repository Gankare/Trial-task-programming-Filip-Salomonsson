using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Image fillImage;

    public void SetHealth(float normalizedValue)
    {
        fillImage.fillAmount = Mathf.Clamp01(normalizedValue);
    }
}
