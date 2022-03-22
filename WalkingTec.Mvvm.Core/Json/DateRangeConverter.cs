using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WalkingTec.Mvvm.Core.Json
{
    public class DateRangeConverter : JsonConverter<DateRange>
    {
        public override DateRange Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            try
            {
                if (reader.TokenType == JsonTokenType.StartArray)
                {
                    reader.Read();
                    string[] ds = new string[2];
                    ds[0] = reader.GetString();
                    reader.Read();
                    ds[1] = reader.GetString();
                    reader.Read();
                    if (DateRange.TryParse(ds, out var dateRange))
                    {
                        return dateRange;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        public override void Write(Utf8JsonWriter writer, DateRange value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
            }
            else
            {
                //writer.WriteStringValue(JsonSerializer.Serialize(value),);
                writer.WriteStartArray();
                writer.WriteStringValue(value.GetStartTime().ToString());
                writer.WriteStringValue(value.GetEndTime().ToString());
                writer.WriteEndArray();
            }
        }
    }
}