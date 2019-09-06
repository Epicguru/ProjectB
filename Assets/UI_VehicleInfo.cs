using System.Text;
using TMPro;
using UnityEngine;

public class UI_VehicleInfo : MonoBehaviour
{
    public Vehicle TrackedVehicle;

#pragma warning disable CS0649
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI details;
    [SerializeField] private TextMeshProUGUI healthSummary;
    [SerializeField] private GameObject healthDetails;
    [SerializeField] private UI_HealthPart healthPartPrefab;
    [SerializeField] private TextMeshProUGUI healthToggleButtonText;
#pragma warning restore CS0649

    private const int MAX_PARTS = 20;
    private UI_HealthPart[] parts;

    private void Awake()
    {
        parts = new UI_HealthPart[MAX_PARTS];
        for (int i = 0; i < MAX_PARTS; i++)
        {
            var spawned = Instantiate(healthPartPrefab);
            spawned.transform.SetParent(healthDetails.transform);

            spawned.gameObject.SetActive(false);
            parts[i] = spawned;
        }
    }

    private void Update()
    {
        if (TrackedVehicle != null)
        {
            UpdateFor(TrackedVehicle);
        }
    }

    public void UpdateFor(Vehicle v)
    {
        title.text = v.Name.Trim();
        details.text = MakeDetails(v);
        healthSummary.text = MakeHealthSummary(v);

        if (healthDetails.activeSelf)
        {
            healthDetails.SetActive(false);
            var realParts = v.Health.GetAllHealthParts();
            for (int i = 0; i < MAX_PARTS; i++)
            {
                var part = parts[i];
                if(i < realParts.Length)
                {
                    var rp = realParts[i];
                    part.SetInfo(rp.Name, rp.Health, rp.MaxHealth);
                    part.gameObject.SetActive(true);
                }
                else
                {
                    part.gameObject.SetActive(false);
                }
            }
            healthDetails.SetActive(true);
        }
    }

    public static string MakeDetails(Vehicle v)
    {
        float mps = v.Movement.Body.velocity.magnitude * 10f;
        float kph = mps * 3.6f;
        return $"Speed: {kph:F1} kph\n";
    }

    public static string MakeHealthSummary(Vehicle v)
    {
        StringBuilder str = new StringBuilder();
        str.Append("\nMissing: ");
        bool first = true;
        int count = 0;
        foreach (var part in v.Health.GetAllHealthParts())
        {
            if(part.Health == 0f)
            {
                if (!first)
                    str.Append(", ");

                str.Append(part.Name.Trim());

                first = false;
                count++;
            }
        }
        return $"Health: {v.Health.GetSumHealth():F0}/{v.Health.GetSumMaxHealth():F0}{(count != 0 ? str.ToString() : string.Empty)}";
    }

    public void ToggleExpandedHealth()
    {
        healthDetails.SetActive(!healthDetails.activeSelf);
        healthToggleButtonText.text = healthDetails.activeSelf ? "Hide Health" : "Show Health";
    }
}
