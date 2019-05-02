using System;
using System.Collections.Generic;
using System.Text;
using System.Dynamic;

namespace YDB.Models
{
    public class DynamicDataBaseModel : DynamicObject
    {
        public string Test1 { get; set; }
        public string Test2 { get; set; }
        public string Test3 { get; set; }
    }
}
