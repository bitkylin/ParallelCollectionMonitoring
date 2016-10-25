using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bitkyFlashresUniversal.dataExport.bean;
using Newtonsoft.Json;

namespace bitkyFlashresUniversal.dataExport
{
    class DataExport
    {
        public static void Output(SummaryDataJson dataJson)
        {
            if (!Directory.Exists("./json"))
            {
                Directory.CreateDirectory("./json");
            }

            var fileStream = new FileStream("./json/outJson.json", FileMode.Create, FileAccess.Write);
            var json = JsonConvert.SerializeObject(dataJson);
            var streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
            streamWriter.Write(json);
            streamWriter.Close();
        }
    }
}