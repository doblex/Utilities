using UnityEngine;

namespace utilities.controllers.stats
{
    [CreateAssetMenu(fileName = "Stat", menuName = "Stats")]
    public class Stat : ScriptableObject
    {
        [SerializeField] new string name;
        [SerializeField][TextArea(0, 10)] string description;
        [SerializeField] double maxValue;
        [SerializeField] double minValue;
        [SerializeField] double startValue;

        public Stat()
        {
            name = "Default Stat";
            maxValue = 100;
            minValue = 0;
            startValue = 100;
        }

        public string Name { get => name; }
        public double MaxValue { get => maxValue; }
        public double MinValue { get => minValue; }
        public double StartValue { get => startValue; }
    }
}
