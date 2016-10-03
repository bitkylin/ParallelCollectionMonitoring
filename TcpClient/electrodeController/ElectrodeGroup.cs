using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpClientBitky.electrodeController
{
    /// <summary>
    /// 一组电极，这组电极包括类型A、B、M的编号
    /// </summary>
    class ElectrodeGroup
    {
        int typeA = -1;
        int typeB = -1;
        int typeM = -1;

        public ElectrodeGroup()
        {

        }

        public ElectrodeGroup(int typeA, int typeB, int typeM)
        {
            this.typeA = typeA;
            this.typeB = typeB;
            this.typeM = typeM;
        }

        public bool isReady()
        {
            return TypeA != -1 && TypeB != -1 && TypeM != -1;
        }

        public int TypeA
        {
            get
            {
                return typeA;
            }

            set
            {
                typeA = value;
            }
        }

        public int TypeB
        {
            get
            {
                return typeB;
            }

            set
            {
                typeB = value;
            }
        }

        public int TypeM
        {
            get
            {
                return typeM;
            }

            set
            {
                typeM = value;
            }
        }
    }
}
