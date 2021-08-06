using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksApp.Serializers
{
    public interface ISerilizer
    {
        string Serialize(object obj);
        T Deserialize<T>(string serializedObject);
    }
}
