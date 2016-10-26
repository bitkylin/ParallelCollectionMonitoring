using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using bitkyFlashresUniversal.dataExport.bean;
using Newtonsoft.Json;

namespace bitkyFlashresUniversal.dataExport
{
    internal static class DataExport
    {
        public static string FilePath = "";

        public static bool OutputJson(SummaryDataJson dataJson)
        {
            try
            {
                if (!Directory.Exists("./dataOutput/json"))
                {
                    Directory.CreateDirectory("./dataOutput/json");
                }
                var dateTimeStr = dataJson.DateTime.ToString("yyyy-MM-dd_H-mm-ss");
                FilePath = "./dataOutput/json/" + dateTimeStr + "_OutData.json";
                var fileStream = new FileStream(FilePath, FileMode.Create,
                    FileAccess.Write);
                var json = JsonConvert.SerializeObject(dataJson);
                var streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
                streamWriter.Write(json);
                streamWriter.Close();
                return true;
            }
            catch (Exception ex)
            {
                FilePath = ex.Message + ":" + ex.StackTrace;
                return false;
            }
        }
    }
}