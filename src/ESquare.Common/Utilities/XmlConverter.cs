using System.IO;
using System.Xml;
using Newtonsoft.Json;

namespace ESquare.Common.Utilities
{
    /// <summary>
    /// Convert from XML string to various types
    /// </summary>
    public static class XmlConverter
    {
        /// <summary>
        /// Convert from json string to xml string
        /// </summary>
        /// <param name="jsonString">json string</param>
        /// <param name="rootElement">root element to be appended to xml string</param>
        /// <returns>return xml string</returns>
        public static string FromJsonString(string jsonString, string rootElement = "root")
        {
            string xml = string.Empty;
            if (!string.IsNullOrEmpty(jsonString))
            {
                // Add root element for xml string
                XmlDocument xmlDocument = JsonConvert.DeserializeXmlNode(jsonString, rootElement, true);
                if (xmlDocument != null)
                {
                    // Ignore XML version declaration
                    XmlWriterSettings settings = new XmlWriterSettings() { OmitXmlDeclaration = true };

                    // Build XML string
                    using (StringWriter strWriter = new StringWriter())
                    using (XmlWriter xmlWriter = XmlWriter.Create(strWriter, settings))
                    {
                        xmlDocument.WriteTo(xmlWriter);
                        xmlWriter.Flush();
                        xml = strWriter.GetStringBuilder().ToString();
                    }
                }
            }

            return xml;
        }

        /// <summary>
        /// Convert from xml string to json string
        /// </summary>
        /// <param name="xmlString">xml string</param>
        /// <returns>json string</returns>
        public static string ToJsonString(string xmlString)
        {
            string json = string.Empty;
            if (!string.IsNullOrEmpty(xmlString))
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xmlString);
                json = JsonConvert.SerializeXmlNode(xmlDocument, Newtonsoft.Json.Formatting.None, true);
            }

            return json;
        }
    }
}