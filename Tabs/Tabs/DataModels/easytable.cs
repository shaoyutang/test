using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tabs.DataModels
{
public class easytable
{
    [JsonProperty(PropertyName = "Id")]
    public string ID { get; set; }

    [JsonProperty(PropertyName = "Tag")]
    public string Tag { get; set; }
}
}
