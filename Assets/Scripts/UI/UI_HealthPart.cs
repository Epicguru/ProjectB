using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_HealthPart : MonoBehaviour
{
#pragma warning disable CS0649
    [SerializeField]
    private TextMeshProUGUI nameText;
    [SerializeField]
    private TextMeshProUGUI detailsText;
    [SerializeField]
    private Image bar;
#pragma warning restore CS0649

    public void SetInfo(string name, float health, float maxHealth)
    {
        nameText.text = name?.Trim();
        detailsText.text = $"{health:F0}/{maxHealth:F0}";

        float p = Mathf.Clamp01(health / maxHealth);
        bar.fillAmount = p;
    }
}
