using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProductShop.DTOs.Export
{
    [XmlType ("Category")]
    public class ExportCategories
    {
        [XmlElement ("name")]
        public string Name { get; set; }
        [XmlElement ("count")]
        public int Count { get; set; }
        [XmlElement ("averagePrice")]
        public decimal Price { get; set; }
        [XmlElement ("totalRevenue")]
        public decimal Revenue { get; set; }
    }
}
