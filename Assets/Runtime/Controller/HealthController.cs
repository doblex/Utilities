using UnityEngine;
using utilities.controllers.stats.Structs;

namespace utilities.controllers.stats
{
    public class HealthController : BaseStatController
    {
        public delegate void OnHealthChanged(StatData data);
        public delegate void OnHealthDepleted();

        public event OnHealthChanged HealthChanged;
        public event OnHealthDepleted HealthDepleted;

        [Header("Stat Additional Options")]
        [SerializeField] bool invincible;

        public void SetInvicible(bool invincible) { this.invincible = invincible; }
        public void IncreaseHealth(float amount)
        {
            IncreaseValue(amount);
            HealthChanged?.Invoke(GetStatData());
        }
        public void DecreaseHealth(float amount)
        {
            if (invincible) return;

            if (DecreaseValue(amount))
            {
                HealthDepleted?.Invoke();
            }
            else
            {
                HealthChanged?.Invoke(GetStatData());
            }
        }
        public void ResetHealth()
        {
            Reset();
            HealthChanged?.Invoke(GetStatData());
        }
    }
}