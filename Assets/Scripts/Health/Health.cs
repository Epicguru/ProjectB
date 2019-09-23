
using JNetworking;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectB
{
    public class Health : NetBehaviour
    {
        private HealthPart[] parts;
        private Dictionary<Collider2D, HealthPart> c2h;
        private Dictionary<HealthPartID, HealthPart[]> id2h;

        public bool ExplodeUponDeath = true;

        private void Awake()
        {
            parts = GetComponentsInChildren<HealthPart>();
            if (parts != null)
            {
                c2h = new Dictionary<Collider2D, HealthPart>();
                foreach (var item in parts)
                {
                    if (item == null)
                        continue;

                    if (item.Collider == null)
                    {
                        Debug.LogError($"Collider for health part {item.Name} is null. Part is ignored.");
                        continue;
                    }

                    c2h.Add(item.Collider, item);
                }

                id2h = new Dictionary<HealthPartID, HealthPart[]>();
                List<HealthPart> tempParts = new List<HealthPart>();
                foreach (HealthPartID e in System.Enum.GetValues(typeof(HealthPartID)))
                {
                    foreach (var item in parts)
                    {
                        if (item == null)
                            continue;
                        if (item.Collider == null)
                            continue;

                        if (item.ID == e)
                        {
                            tempParts.Add(item);
                        }
                    }
                    id2h.Add(e, tempParts.ToArray());
                    tempParts.Clear();
                }
            }
        }

        public HealthPart GetHealthPart(Collider2D collider)
        {
            if (collider == null)
                return null;

            if (!c2h.ContainsKey(collider))
                return null;

            return c2h[collider];
        }

        public HealthPart GetHealthPart(HealthPartID id)
        {
            var parts = GetHealthParts(id);
            return parts.Length > 0 ? parts[0] : null;
        }

        public HealthPart[] GetHealthParts(HealthPartID id)
        {
            return id2h[id];
        }

        public HealthPart[] GetAllHealthParts()
        {
            return parts;
        }

        public float GetSumHealth()
        {
            float sum = 0f;

            foreach (var item in parts)
            {
                if (item != null && item.Collider != null)
                {
                    sum += item.Health;
                }
            }

            return sum;
        }

        public float GetSumMaxHealth()
        {
            float sum = 0f;

            foreach (var item in parts)
            {
                if (item != null && item.Collider != null)
                {
                    sum += item.MaxHealth;
                }
            }

            return sum;
        }

        public void ChangeHealth(Collider2D collider, float change)
        {
            if (!JNet.IsServer)
            {
                Debug.LogError("Cannot change health when not on server.");
                return;
            }

            if (change == 0f)
                return;

            var part = GetHealthPart(collider);
            if (part != null)
            {
                float old = part.Health;
                part.Health += change;
                part.Health = Mathf.Clamp(part.Health, 0f, part.MaxHealth);

                if (old != part.Health)
                    UponPartHealthChanged(part, change);
            }
        }

        private void UponPartHealthChanged(HealthPart part, float change)
        {
            if (part.IsVital)
            {
                if (part.Health == 0f)
                {
                    Debug.Log($"Damn we dead. Died because {part.Name} was destroyed.");

                    var spawned = PoolObject.Spawn(Spawnables.Get<PoolObject>("ExplosionEffect"));
                    spawned.transform.position = this.transform.position;
                    spawned.GetComponent<AutoDestroy>().NetSpawn();

                    Destroy(this.gameObject);
                }
            }
        }
    }

    public enum HealthPartID
    {
        HULL,
        ENGINE,
        BRIDGE,
        OTHER
    }
}
