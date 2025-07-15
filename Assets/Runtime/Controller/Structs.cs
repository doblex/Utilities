namespace utilities.controllers.stats.Structs
{

    public struct StatData
    {
        public string Name;
        public double MaxValue;
        public double MinValue;
        public double StartValue;

        public StatData(string name, double maxValue, double minValue, double startValue)
        {
            Name = name;
            MaxValue = maxValue;
            MinValue = minValue;
            StartValue = startValue;
        }
    }
}
