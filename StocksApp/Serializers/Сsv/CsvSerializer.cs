using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksApp.Serializers.Сsv
{
    public class CsvSerializer : ICsvSerializer
    {
        public string Serialize(IEnumerable<object> obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            using (var writer = new StringWriter())
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture, true))
            {
                csv.WriteRecords(obj);
                csv.Flush();
                var serialized = writer.ToString();
                return serialized;
            }
        }

        public IList<T> Deserialize<T>(string serializedObject)
        {
            if (serializedObject == null)
                throw new ArgumentNullException(nameof(serializedObject));

            // convert string to stream
            byte[] byteArray = Encoding.ASCII.GetBytes(serializedObject);
            using MemoryStream stream = new MemoryStream(byteArray);
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                IList<T> obj = csv.GetRecords<T>().ToList();
                return obj;
            }
        }
    }
}
