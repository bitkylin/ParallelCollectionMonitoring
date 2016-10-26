using System.IO;
using System.Text;
using bitkyFlashresUniversal.dataExport.bean;
using Newtonsoft.Json;

namespace bitkyFlashresUniversal.dataExport
{
    internal static class DataExport
    {
        public static void OutputJson(SummaryDataJson dataJson)
        {
            if (!Directory.Exists("./dataOutput/json"))
            {
                Directory.CreateDirectory("./dataOutput/json");
            }
            var dateTimeStr = dataJson.DateTime.ToString("yyyy-MM-dd_H-mm-ss");
            var fileStream = new FileStream("./dataOutput/json/" + dateTimeStr + "_OutData.json", FileMode.Create, FileAccess.Write);
            var json = JsonConvert.SerializeObject(dataJson);
            var streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
            streamWriter.Write(json);
            streamWriter.Close();
        }
        public static void OutputXml(SummaryDataJson dataJson)
        {
            if (!Directory.Exists("./dataOutput/xml"))
            {
                Directory.CreateDirectory("./dataOutput/xml");
            }
            var dateTimeStr = dataJson.DateTime.ToString("yyyy-MM-dd_H-mm-ss");
            var fileStream = new FileStream("./dataOutput/xml/" + dateTimeStr + "_OutData.xml", FileMode.Create, FileAccess.Write);
            var json = JsonConvert.SerializeObject(dataJson);
            var streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
            streamWriter.Write(json);
            streamWriter.Close();
        }
    }
}