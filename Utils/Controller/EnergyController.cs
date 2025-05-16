using UnityEngine;

namespace utilities.Controllers
{
    public class EnergyController : MonoBehaviour
    {
        public delegate void OnEnergyChange(float maxEnergy, float currentEnergy);
        public OnEnergyChange onEnergyChange;
        [SerializeField] public float maxEnergy = 100f;
        [SerializeField] public float currentEnergy;

        private void Start()
        {
            currentEnergy = maxEnergy;
            onEnergyChange?.Invoke(maxEnergy, currentEnergy);
        }

        public bool UseEnergy(float amount)
        {
            if (currentEnergy >= amount)
            {
                currentEnergy -= amount;
                onEnergyChange?.Invoke(maxEnergy, currentEnergy);
                return true;
            }
            return false;
        }

        public void RestoreEnergy(float amount)
        {
            currentEnergy += amount;
            currentEnergy = Mathf.Min(currentEnergy, maxEnergy);
            onEnergyChange?.Invoke(maxEnergy, currentEnergy);
        }

        public void SetMaxEnergy(float amount)
        {
            maxEnergy = amount;
            currentEnergy = Mathf.Min(currentEnergy, maxEnergy);
            onEnergyChange?.Invoke(maxEnergy, currentEnergy);
        }
    }
}