using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProductShop.DTOs.Export
{
    [XmlType ("User")]
    public class ExportSoldProductsWithUsers
    {
        [XmlElement ("firstName")]
        public string FirstName { get; set; }
        [XmlElement("lastName")]
        public string LastName { get; set; }
        [XmlArray ("soldProducts")]
        public ExportSoldProducts [] SoldProducts { get; set; }
    }
}
