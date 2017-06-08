namespace KyInversion.produceData.bean
{
    public class PoleLocation
    {
        public PoleLocation(int id, int xAxis, int zAxis)
        {
            ID = id;
            this.xAxis = xAxis;
            this.zAxis = zAxis;
        }

        public int ID { get; set; }
        public int xAxis { get; set; }
        public int zAxis { get; set; }
        public bool Enabled { get; set; } = false;
    }
}