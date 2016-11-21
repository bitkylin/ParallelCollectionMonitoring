using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using collectedReading.jsonBean;
using collectedReading.produceData.bean;

namespace collectedReading.produceData
{
    public class ProducePresenter
    {
        private static ProducePresenter _presenter;
        private SummaryDataJson _dataJson;
        private PoleLocation[] _poleLocations;

        public static ProducePresenter Builder()
        {
            if (_presenter == null)
            {
                _presenter = new ProducePresenter();
            }
            return _presenter;
        }


        public ProducePresenter SetDataJson(SummaryDataJson dataJson)
        {
            _dataJson = dataJson;
            return this;
        }

        public ProducePresenter SetPoleLocationArray(PoleLocation[] poleLocations)
        {
            _poleLocations = poleLocations;
            return this;
        }

        public void ProduceOutputData()
        {
            var streamWriterSet = StreamWriterSet.Builder(_dataJson.DateTime.ToString("yyyy-MM-dd_H-mm-ss"),
                _poleLocations);
            var collectResult = _dataJson.PoleResult;
            collectResult.ForEach(item =>
            {
                if (_poleLocations[item.A] != null && _poleLocations[item.B] != null && _poleLocations[item.M] != null)
                {
                    var a = item.A;
                    var b = item.B;
                    var m = item.M;

                    item.Poles.ForEach(pole =>
                    {
                        if (_poleLocations[pole.Id] != null && pole.Id != a && pole.Id != b && pole.Id != m)
                        {
                            if (pole.Value < 0)
                            {
                                var anode = pole.Id;
                                var cathode = m;
                                var value = Math.Abs(pole.Value);
                                streamWriterSet.WriteData(a, b, anode, cathode, value);
                            }
                            else
                            {
                                var anode = m;
                                var cathode = pole.Id;
                                var value = Math.Abs(pole.Value);
                                streamWriterSet.WriteData(a, b, anode, cathode, value);
                            }
                        }
                    });
                }
            });
            streamWriterSet.WriteMrefFile();

            streamWriterSet.Close();
        }


        private class StreamWriterSet
        {
            private static StreamWriterSet _streamWriterSet;

            private PoleLocation[] _poleLocations;

            public StreamWriter dxStreamWriter { get; set; }
            public StreamWriter dzStreamWriter { get; set; }
            public StreamWriter srcFileStreamWriter { get; set; }
            public StreamWriter recFileStreamWriter { get; set; }
            public StreamWriter dataFileStreamWriter { get; set; }
            public StreamWriter mrefFileStreamWriter { get; set; }


            public static StreamWriterSet Builder(string folderName, PoleLocation[] poleLocations)
            {
                if (_streamWriterSet == null)
                {
                    _streamWriterSet = new StreamWriterSet()
                        .SetPoleLocations(poleLocations)
                        .InitDataOutputFile(folderName);
                }
                return _streamWriterSet;
            }

            private StreamWriterSet InitDataOutputFile(string folderName)
            {
                var dateTimeStr = folderName;
                var filePath = "./dataOutput/RESINVM/" + dateTimeStr + "_OutData/";
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                var dxStream = new FileStream(filePath + "dx.txt", FileMode.Create, FileAccess.Write);
                var dzStream = new FileStream(filePath + "dz.txt", FileMode.Create, FileAccess.Write);
                var srcFileStream = new FileStream(filePath + "SrcFile.txt", FileMode.Create, FileAccess.Write);
                var recFileStream = new FileStream(filePath + "RecFile.txt", FileMode.Create, FileAccess.Write);
                var dataFileStream = new FileStream(filePath + "DataFile.txt", FileMode.Create, FileAccess.Write);
                var mrefFileStream = new FileStream(filePath + "MrefFile.txt", FileMode.Create, FileAccess.Write);

                var utf8Encoding = new UTF8Encoding(false);
                return this.SetsrcFileStreamWriter(new StreamWriter(srcFileStream, utf8Encoding))
                    .SetrecFileStreamWriter(new StreamWriter(recFileStream, utf8Encoding))
                    .SetdataFileStreamWriter(new StreamWriter(dataFileStream, utf8Encoding))
                    .SetmrefFileStreamWriter(new StreamWriter(mrefFileStream, utf8Encoding))
                    .SetDxStreamWriter(new StreamWriter(dxStream, utf8Encoding))
                    .SetDzStreamWriter(new StreamWriter(dzStream, utf8Encoding));
            }

            private StreamWriterSet SetPoleLocations(PoleLocation[] poleLocations)
            {
                _poleLocations = poleLocations;
                return this;
            }

            private StreamWriterSet SetsrcFileStreamWriter(StreamWriter srcFileStreamWriter)
            {
                this.srcFileStreamWriter = srcFileStreamWriter;
                srcFileStreamWriter.NewLine = "\r\n";
                return this;
            }

            private StreamWriterSet SetrecFileStreamWriter(StreamWriter recFileStreamWriter)
            {
                this.recFileStreamWriter = recFileStreamWriter;
                recFileStreamWriter.NewLine = "\r\n";
                return this;
            }

            private StreamWriterSet SetdataFileStreamWriter(StreamWriter dataFileStreamWriter)
            {
                this.dataFileStreamWriter = dataFileStreamWriter;
                dataFileStreamWriter.NewLine = "\r\n";
                return this;
            }

            private StreamWriterSet SetmrefFileStreamWriter(StreamWriter mrefFileStreamWriter)
            {
                this.mrefFileStreamWriter = mrefFileStreamWriter;
                mrefFileStreamWriter.NewLine = "\r\n";
                return this;
            }

            private StreamWriterSet SetDxStreamWriter(StreamWriter dxStreamWriter)
            {
                this.dxStreamWriter = dxStreamWriter;
                dxStreamWriter.NewLine = "\r\n";
                return this;
            }

            private StreamWriterSet SetDzStreamWriter(StreamWriter dzStreamWriter)
            {
                this.dzStreamWriter = dzStreamWriter;
                dzStreamWriter.NewLine = "\r\n";
                return this;
            }

            public void WriteData(int a, int b, int anode, int cathode, double value)
            {
//                var srcFile = string.Format("Ax:{0} Az:{1} Bx:{2} Bz:{3}", _poleLocations[a].xAxis,
//                    _poleLocations[a].zAxis,
//                    _poleLocations[b].xAxis, _poleLocations[b].zAxis);
//                srcFileStreamWriter.WriteLine("A:" + _poleLocations[a].ID + " B:" + _poleLocations[b].ID + "  .  " +
//                                              srcFile);
//
//                var recFile = string.Format("Mx:{0} Mz:{1} Nx:{2} Nz:{3}", _poleLocations[anode].xAxis,
//                    _poleLocations[anode].zAxis,
//                    _poleLocations[cathode].xAxis, _poleLocations[cathode].zAxis);
//                recFileStreamWriter.WriteLine("M:" + _poleLocations[anode].ID + " N:" + _poleLocations[cathode].ID +
//                                              "  .  " + recFile);

                srcFileStreamWriter.WriteLine(_poleLocations[a].xAxis + " " + _poleLocations[a].zAxis + " " +
                                              _poleLocations[b].xAxis + " " + _poleLocations[b].zAxis);
                recFileStreamWriter.WriteLine(_poleLocations[anode].xAxis + " " + _poleLocations[anode].zAxis + " " +
                                              _poleLocations[cathode].xAxis + " " + _poleLocations[cathode].zAxis);
                dataFileStreamWriter.WriteLine(value + " " + 1);
            }

            public void Close()
            {
                srcFileStreamWriter.Close();
                recFileStreamWriter.Close();
                dataFileStreamWriter.Close();
                mrefFileStreamWriter.Close();
                dxStreamWriter.Close();
                dzStreamWriter.Close();
                _streamWriterSet = null;
            }

            public void WriteMrefFile()
            {
                var maxX = 0;
                var maxZ = 0;

                foreach (var poleLocation in _poleLocations)
                {
                    if (poleLocation != null)
                    {
                        Debug.WriteLine(poleLocation.ID + ":" + poleLocation.xAxis + "," + poleLocation.zAxis);

                        if (poleLocation.xAxis > maxX)
                        {
                            maxX = poleLocation.xAxis;
                        }
                        if (poleLocation.zAxis > maxZ)
                        {
                            maxZ = poleLocation.zAxis;
                        }
                    }
                }
                var length = maxX*maxZ*2;
                for (var i = 0; i < length; i++)
                {
                    mrefFileStreamWriter.Write(100 + " ");
                }

                var scaleX = maxX*2 + 10;
                var scaleZ = maxZ + 10;
                Debug.WriteLine("scaleX:" + scaleX + "scaleZ:" + scaleZ);

                for (var i = 0; i < scaleX; i++)
                {
                    dxStreamWriter.WriteLine(1);
                }
                for (var i = 0; i < scaleZ; i++)
                {
                    dzStreamWriter.WriteLine(1);
                }
            }
        }
    }
}