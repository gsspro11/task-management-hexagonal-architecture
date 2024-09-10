using System.Xml.Serialization;

namespace Poc.ContasAtualizacaoCadastralConsumer.Domain.Extensions.v1
{
    public static class StringExtensions
    {
        public static string Serialize<T>(this T dataToSerialize)
        {
            var stringwriter = new StringWriter();
            var serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(stringwriter, dataToSerialize);
            return stringwriter.ToString();
        }

        public static T Deserialize<T>(this string xmlText)
        {
            var stringReader = new StringReader(xmlText);
            var serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(stringReader);
        }
    }
}
