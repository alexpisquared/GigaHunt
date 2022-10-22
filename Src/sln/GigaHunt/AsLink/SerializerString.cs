namespace GigaHunt.AsLink;
public static partial class Serializer
{
  public static string SaveToString(object o)
  {
    var stringBuilder = new StringBuilder();
    using var stringWriter = new StringWriter(stringBuilder);
    new XmlSerializer(o.GetType()).Serialize(stringWriter, o);
    return stringBuilder.ToString();
  }

  public static object? LoadFromString<T>(string str)
  {
    using var streamReader = new StringReader(str);
    return new XmlSerializer(typeof(T)).Deserialize(streamReader); //TU: DEserialise from a stream       ||  return Activator.CreateInstance(typeof(T)); //		???		throw;  Watch it !!!!!
  }
}