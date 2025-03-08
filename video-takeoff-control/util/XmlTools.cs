using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace video_takeoff_control.util
{
    public class XmlTools
    {
        public static string SerializeToXml<T>(T obj)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StringWriter stringWriter = new StringWriter())
            {
                serializer.Serialize(stringWriter, obj);
                return stringWriter.ToString();
            }
        }

        public static T DeserializeFromXml<T>(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StringReader stringReader = new StringReader(xml))
            {
                return (T)serializer.Deserialize(stringReader);
            }
        }

    }
}
