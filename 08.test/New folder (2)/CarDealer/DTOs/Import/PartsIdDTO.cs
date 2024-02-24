using System.Xml.Serialization;

namespace CarDealer.DTOs.Import
{
    [XmlType ("partId")]
    public class PartsIdDTO
    {
        [XmlAttribute("id")]
        public int Id { get; set; }
    }
}