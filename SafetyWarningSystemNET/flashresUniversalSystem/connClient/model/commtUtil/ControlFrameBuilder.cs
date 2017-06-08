using System.Collections.Generic;
using System.Diagnostics;
using bitkyFlashresUniversal.connClient.model.bean;

namespace bitkyFlashresUniversal.connClient.model.commtUtil
{
    /// <summary>
    ///     控制帧生成器，用于发送至下位机
    /// </summary>
    internal class ControlFrameBuilder
    {
        /// <summary>
        ///     构建数据帧
        /// </summary>
        /// <param name="frameData">帧数据信息集合</param>
        /// <returns>构建完毕的数据帧</returns>
        public byte[] DataFrameBuild(FrameData frameData)
        {
            //   var poleList = BuildElectrodesDemo(); //演示用，演示完毕放回成员变量
            var frame = new List<byte>();
            frame.AddRange(CommMsg.DataFrameHeader);
            frame.AddRange(IntToBytes(340));
            foreach (var bytes in Build(frameData.Type, frameData.PoleList))
                frame.AddRange(bytes);
            frame.AddRange(CommMsg.ActivateGatherFrameSubFrame);
            var checkBytes = CheckFrame(frame);
            frame.AddRange(checkBytes);
            return frame.ToArray();
        }

        private byte[] CheckFrame(List<byte> bytes)
        {
            short checkNum = 0;
            bytes.ForEach(b => { checkNum += b; });
            return IntToBytes(checkNum);
        }

        /// <summary>
        ///     构建需发送的的子帧
        /// </summary>
        /// <param name="type">子帧的类型</param>
        /// <param name="poleList">特定电极集合</param>
        /// <returns>返回构建的子帧集合</returns>
        private List<List<byte>> Build(FrameType type, List<Electrode> poleList)
        {
            var frameHeadBytes = new byte[0];
            switch (type)
            {
                case FrameType.ControlGather:
                    frameHeadBytes = CommMsg.ControlGatherFrameHeader;
                    break;
                case FrameType.ActivateGather:
                    frameHeadBytes = CommMsg.ActivateGatherFrameHeader;
                    break;
                default:
                    Debug.WriteLine("类型选择错误");
                    break;
            }
            var returnList = new List<List<byte>>();
            var data = new List<byte>();
            data.AddRange(frameHeadBytes);

            for (var i = 0; i < 64; i++)
            {
                data.Add((byte) i);
                var temp = (byte[]) CommMsg.DataFrameTypeN.Clone();

                poleList.ForEach(pole =>
                {
                    if (pole.IdOrigin == i)
                        if (pole.Mode == PoleMode.A)
                            temp = (byte[]) CommMsg.DataFrameTypeA.Clone();
                        else if (pole.Mode == PoleMode.B)
                            temp = (byte[]) CommMsg.DataFrameTypeB.Clone();
                        else if (pole.Mode == PoleMode.M)
                            temp = (byte[]) CommMsg.DataFrameTypeM.Clone();
                });
                data.AddRange(temp);

                if ((i + 1)%16 == 0)
                {
                    returnList.Add(data);
                    data = new List<byte>();
                    data.AddRange(frameHeadBytes);
                }
            }
            return returnList;
        }


        /// <summary>
        ///     将int数值转换为占两个字节的byte数组，本方法适用于(int数字低位在前，高位在后)的顺序。 和bytesToInt（）配套使用
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static byte[] IntToBytes(short value)
        {
            var src = new byte[2];
            src[1] = (byte) ((value >> 8) & 0xFF);
            src[0] = (byte) (value & 0xFF);
            return src;
        }

        /// <summary>
        ///     byte数组中取int数值，本方法适用于(int数字低位在前，高位在后)的顺序，和和intToBytes（）配套使用
        /// </summary>
        /// <param name="src">byte数组</param>
        /// <returns>int数值</returns>
        public static int BytesToInt(byte[] src)
        {
            var value = (src[0] & 0xFF)
                        | ((src[1] & 0xFF) << 8);
            return value;
        }

        /// <summary>
        ///     byte数组中取int数值，本方法适用于(int数字低位在后，高位在前)的顺序，和和intToBytes（）配套使用
        /// </summary>
        /// <param name="src">byte数组</param>
        /// <returns>int数值</returns>
        public static int BytesToInt3(byte[] src)
        {
            var value = ((src[0] & 0xFF) << 16)
                        | ((src[1] & 0xFF) << 8)
                        | (src[2] & 0xFF);
            return value;
        }
    }
}