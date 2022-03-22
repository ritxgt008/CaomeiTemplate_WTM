namespace System.Text.Json.Serialization
{
    /// <summary>
    /// StringIgnoreLTGTConvert
    /// 忽略客户端提交的 &lt;及&gt;字符
    /// </summary>
    public class StringIgnoreLTGTConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString().Replace("<", string.Empty).Replace(">", string.Empty);
            }

            return null;
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStringValue(value);
            }
        }
    }
}