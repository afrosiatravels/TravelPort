using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travelport
{
    class Setting
    {
        public String Key { get; set; }
        public String Value { get; set; }
        public Setting(String key, String value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
}
