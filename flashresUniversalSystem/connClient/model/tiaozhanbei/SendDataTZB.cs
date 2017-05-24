using System.Collections.Generic;
using TCPServer.model;

namespace bitkyFlashresUniversal.connClient.model.tiaozhanbei
{
    public class SendDataTZB
    {
        private static FrameBuilder _frameBuilder = new FrameBuilder();

        private static List<byte> SendActivateGatherFrame()
        {
            var list = new List<byte>();
            list.AddRange(_frameBuilder.Build(0));
            list.AddRange(_frameBuilder.Build(1));
            list.AddRange(_frameBuilder.Build(2));
            list.AddRange(_frameBuilder.Build(3));
            list.AddRange(_frameBuilder.Build(4));
            return list;
        }

        public static List<byte> process(byte[] data)
        {
            if (data[0] == 0x01)
            {
                return SendActivateGatherFrame();
            }
            else
            {
                return null;
            }
        }
    }
}