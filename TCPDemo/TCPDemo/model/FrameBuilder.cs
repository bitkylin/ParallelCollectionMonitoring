using System;
using System.Collections.Generic;
using System.Diagnostics;
using TCPDemo.util;

namespace TCPDemo.model
{
    public class FrameBuilder
    {
        Random _random = new Random();

       public byte[] Build(int num)
        {
            var i = num*16;
            var end = i + 16;
            var data = new List<byte>();
            data.AddRange(CommMsg.ControlGatherFrameHeaderReturn);

            for (; i < end; i++)
            {
                data.Add((byte) i);
                data.AddRange(IntToBytes());
            }
            return data.ToArray();
        }


        /// <summary>
        ///     将int数值转换为占三个字节的byte数组，本方法适用于(int数字低位在后，高位在前)的顺序。 和bytesToInt（）配套使用
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private  byte[] IntToBytes()
        {
            var value = _random.Next(330000);

            Debug.WriteLine(value);
            var src = new byte[3];
            src[0] = (byte) ((value >> 16) & 0xFF);
            src[1] = (byte) ((value >> 8) & 0xFF);
            src[2] = (byte) (value & 0xFF);
            return src;
        }
    }

}