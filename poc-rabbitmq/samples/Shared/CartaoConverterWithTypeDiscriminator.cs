using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared
{
    public class CartaoConverterWithTypeDiscriminator : JsonConverter<CartaoMessage>
    {
        enum TypeDiscriminator
        {
            Credicard = 1,
            Visa = 2
        }

        public override bool CanConvert(Type typeToConvert) =>
            typeof(CartaoMessage).IsAssignableFrom(typeToConvert);

        public override CartaoMessage Read(
            ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            string? propertyName = reader.GetString();
            if (propertyName != "TypeDiscriminator")
            {
                throw new JsonException();
            }

            reader.Read();
            if (reader.TokenType != JsonTokenType.Number)
            {
                throw new JsonException();
            }

            TypeDiscriminator typeDiscriminator = (TypeDiscriminator)reader.GetInt32();
            CartaoMessage cartao = typeDiscriminator switch
            {
                TypeDiscriminator.Credicard => new CredicardMessage(),
                TypeDiscriminator.Visa => new VisaMessage(),
                _ => throw new JsonException()
            };

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return cartao;
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    propertyName = reader.GetString();
                    reader.Read();
                    switch (propertyName)
                    {
                        case "Description":
                            string? description = reader.GetString();
                            ((VisaMessage)cartao).Description = description;
                            break;
                        case "Title":
                            string? title = reader.GetString();
                            ((CredicardMessage)cartao).Title = title;
                            break;
                        case "Cvv":
                            string? name = reader.GetString();
                            cartao.Cvv = name;
                            break;
                        case "Number":
                            string? Number = reader.GetString();
                            cartao.Number = Number;
                            break;
                    }
                }
            }

            throw new JsonException();
        }

        public override void Write(
            Utf8JsonWriter writer, CartaoMessage cartao, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            if (cartao is CredicardMessage credicard)
            {
                writer.WriteNumber("TypeDiscriminator", (int)TypeDiscriminator.Credicard);
                writer.WriteString("Title", credicard.Title);
            }
            else if (cartao is VisaMessage visa)
            {
                writer.WriteNumber("TypeDiscriminator", (int)TypeDiscriminator.Visa);
                writer.WriteString("Description", visa.Description);
            }

            writer.WriteString("Cvv", cartao.Cvv);
            writer.WriteString("Number", cartao.Number);

            writer.WriteEndObject();
        }
    }
}
