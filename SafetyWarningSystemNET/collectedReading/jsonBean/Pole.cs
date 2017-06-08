namespace KyInversion.jsonBean
{
    public class Pole
    {
        public int Id { get; set; }
        public double Value { get; set; }

        public Pole(int id, double value)
        {
            Id = id;
            Value = value;
        }
    }
}