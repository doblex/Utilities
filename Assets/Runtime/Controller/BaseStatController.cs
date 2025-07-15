using UnityEngine;
using utilities.controllers.stats.Structs;


namespace utilities.controllers.stats
{
    /// <summary>
    /// Base class for stat controllers.
    /// </summary>

    public abstract class BaseStatController : MonoBehaviour
    {
        [SerializeField] protected Stat Stat;

        [SerializeField] protected bool customValue = false;

        [Header("Stat Settings")]
        [SerializeField] double maxValue;
        [SerializeField] double minValue;
        [SerializeField] double startValue;

        [Header("Current Value")]
        [SerializeField] double currentValue;

        protected virtual void Awake()
        {
            if (Stat == null)
            {
                Debug.LogError($"Stat not found on {gameObject.name}");
                return;
            }

            if (customValue)
            {
                if (maxValue == minValue)
                {
                    Debug.LogError($"Max value and min value cannot be the same on {gameObject.name}");
                    return;
                }

                if (maxValue < minValue)
                {
                    Debug.LogError($"Max value cannot be less than min value on {gameObject.name}");
                    return;
                }

                if (startValue < minValue || startValue > maxValue)
                {
                    Debug.LogError($"Start value must be between min value and max value on {gameObject.name}");
                    return;
                }
            }

            Init();
        }

        protected virtual void OnValidate()
        {
            if (Stat == null)
            {
                maxValue = 0;
                minValue = 0;
                startValue = 0;
                SetValue(0);
            }
            else
            {
                if (customValue) return;

                maxValue = Stat.MaxValue;
                minValue = Stat.MinValue;
                startValue = Stat.StartValue;
                SetValue(currentValue);
            }
        }

        /// <summary>
        /// Initializes the stat controller with the stat's start value, max value, and min value.
        /// </summary>
        protected virtual void Init()
        {
            currentValue = Stat.StartValue;
            startValue = Stat.StartValue;
            maxValue = Stat.MaxValue;
            minValue = Stat.MinValue;
        }

        protected virtual StatData GetStatData()
        {
            return new StatData(Stat.name, maxValue, minValue, startValue);
        }

        /// <summary>
        /// Sets the current value of the stat, clamping it between min and max values.
        /// </summary>
        /// <param name="value"></param>
        protected void SetValue(double value)
        {
            if (value < minValue)
                currentValue = minValue;
            else if (value > maxValue)
                currentValue = maxValue;
            else
                currentValue = value;
        }

        /// <summary>
        /// Increases the current value of the stat by a specified amount, clamping it to the max value if necessary.
        /// </summary>
        /// <param name="amount"></param>
        protected void IncreaseValue(double amount)
        {
            SetValue(currentValue + amount);
        }

        /// <summary>
        /// Decreases the current value of the stat by a specified amount, clamping it to the min value if necessary.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        protected bool DecreaseValue(double amount)
        {
            SetValue(currentValue - amount);
            return currentValue == minValue;
        }

        /// <summary>
        /// Resets the current value of the stat to its start value.
        /// </summary>
        public void Reset()
        {
            currentValue = startValue;
        }

        /// <summary>
        /// Returns a string representation of the stat controller, including the current value, max value, and min value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{gameObject.name}: {currentValue}/{maxValue} (Min: {minValue})";
        }
    }
}

