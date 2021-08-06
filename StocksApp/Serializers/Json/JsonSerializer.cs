using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksApp.Serializers.Json
{
    public class JsonSerializer : IJsonSerializer
    {
        public string Serialize(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            return JsonConvert.SerializeObject(obj);
        }
        public T Deserialize<T>(string serializedObject)
        { 
            if(serializedObject == null)
                throw new ArgumentNullException(nameof(serializedObject));

            return JsonConvert.DeserializeObject<T>(serializedObject);
        }
    }
}
