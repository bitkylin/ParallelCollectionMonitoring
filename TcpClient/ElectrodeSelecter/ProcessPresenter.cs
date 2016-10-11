using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using bitkyFlashresUniversal.connClient.model.bean;

namespace bitkyFlashresUniversal.ElectrodeSelecter
{
    internal class ProcessPresenter
    {
        private readonly List<Electrode> _list;

        public ProcessPresenter(List<Electrode> list)
        {
            _list = list;
        }

        public List<List<Electrode>> Process()
        {
            var listReturn = new List<List<Electrode>>();

            //获取奇数电极和偶数电极集合
            var oddList = new List<Electrode>();
            var evenList = new List<Electrode>();

            _list.ForEach(pole =>
            {
                if (pole.IdCurrent%2 == 0)
                    evenList.Add(pole);
                else if (pole.IdCurrent%2 == 1)
                    oddList.Add(pole);
                else throw new Exception("电极当前值异常");
            });

            //遍历奇数集合和偶数集合
            oddList.ForEach(oddPole =>
            {
                evenList.ForEach(evenPole =>
                {
                    //获得A,B,M的值
                    var oddNum = oddPole.IdCurrent;
                    var evenNum = evenPole.IdCurrent;
                    var mNum = SelectElectrodeM(oddNum, evenNum, _list[_list.Count - 1].IdCurrent);

                    Electrode a = null;
                    Electrode b = null;
                    Electrode m = null;

                    //根据A,B,M的值得到电极
                    _list.ForEach(pole =>
                    {
                        if (pole.IdCurrent == oddNum)
                            a = pole.Clone(PoleMode.A);

                        if (pole.IdCurrent == evenNum)
                            b = pole.Clone(PoleMode.B);
                        if (pole.IdCurrent == mNum)
                            m = pole.Clone(PoleMode.M);
                    });
                    if ((a == null) || ((b == null) | (m == null)))
                        throw new Exception();
                    var listFinal = new List<Electrode>(3) {a, b, m};
                    listReturn.Add(listFinal);
                });
            });
            return listReturn;
        }

        /// <summary>
        ///     选定A,B的情况下，选取M。
        ///     原则：选取A，B的中点并靠近A的位置
        /// </summary>
        /// <param name="odd">选定的A，奇数</param>
        /// <param name="even">选定的B，偶数</param>
        /// <param name="max">电极序号的最大值</param>
        /// <returns>选定的M</returns>
        private int SelectElectrodeM(int odd, int even, int max)
        {
            if (odd > even)
                if (odd - even > 1)
                    return (odd + even)/2 + 1;
                else if (odd < max)
                    return odd + 1;
                else
                    return even - 1;


            if (odd < even)
                if (even - odd > 1)
                    return (even + odd)/2;
                else if (even < max)
                    return even + 1;
                else
                    return odd - 1;
            throw new Exception("selectElectrodeM方法出错");
        }
    }
}