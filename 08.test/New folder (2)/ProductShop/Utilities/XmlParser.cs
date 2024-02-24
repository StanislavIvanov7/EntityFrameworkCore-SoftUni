using ProductShop.DTOs.Import;
using System;
using System.Collections.Generic;
using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Xml.Serialization;

//namespace ProductShop.Utilities
//{
//    public class XmlParser
//    {


//        public T DeserializeObject<T>(string inputXml,string rootName)
//        {
//            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), new XmlRootAttribute(rootName));
//            using StringReader stringReader = new StringReader(inputXml);
//            T userDtos = (T)xmlSerializer.Deserialize(stringReader);
//            return userDtos;
//        }
//        public T SerializeObject<T>(string inputXml, string rootName)
//        {

//        }
//    }
//}

using System.Text;
using System.Xml.Serialization;

namespace Boardgames.Utilities;

public class XmlParser
{
    //Deserializer
    public  T Deserialize<T>(string inputXml, string rootName)
    {
        XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);
        XmlSerializer xmlSerializer =
            new XmlSerializer(typeof(T), xmlRoot);

        using StringReader reader = new StringReader(inputXml);
        T deserializedDtos =
            (T)xmlSerializer.Deserialize(reader);

        return deserializedDtos;
    }

    //Serializer
    public static string Serialize<T>(T obj, string rootName)
    {
        StringBuilder sb = new StringBuilder();

        XmlRootAttribute xmlRoot =
            new XmlRootAttribute(rootName);
        XmlSerializer xmlSerializer =
            new XmlSerializer(typeof(T), xmlRoot);

        XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
        namespaces.Add(string.Empty, string.Empty);

        using StringWriter writer = new StringWriter(sb);
        xmlSerializer.Serialize(writer, obj, namespaces);

        return sb.ToString().TrimEnd();
    }
}
