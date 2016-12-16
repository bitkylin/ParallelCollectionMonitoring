using System.Collections.Generic;

namespace KyInversion.jsonBean
{
   public class CollectItem
    {
        public int No { get; set; }
        public int A { get; set; }
        public int B { get; set; }
        public int M { get; set; }
        public double Elec { get; set; }

        public List<Pole> Poles { get; set; }

        public CollectItem(int no, int a, int b, int m, double elec, List<Pole> poles)
        {
            No = no;
            A = a;
            B = b;
            M = m;
            Elec = elec;
            Poles = poles;
        }
    }
}