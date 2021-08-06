using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksApp.Serializers.Сsv
{
    public interface ICsvSerializer
    {
        string Serialize(IEnumerable<object> obj);
        IList<T> Deserialize<T>(string serializedObject);
    }
}
