using System;
using Newtonsoft.Json.Schema;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions; 
using System.Linq; //where

namespace PocoSchemaReview
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Write schema for creating schema and write poco to create poco:");
            string input = Console.ReadLine();

            if (input == "schema")
            {
                FromPocoToSchema();
            }
            else if (input == "poco")
            {
                FromSchemaToPoco();
            }
            else
            {
                Console.WriteLine("Enter schema or poco!");
                Console.ReadLine();
            }
           
        }

        static void FromPocoToSchema()
        {

            var jsonSchemaGenerator = new JsonSchemaGenerator();
            var myType = typeof(Car);
            var pType = typeof(Buyers);
            var schema = jsonSchemaGenerator.Generate(myType);
            Console.WriteLine(schema);
            var pschema = jsonSchemaGenerator.Generate(pType);

            schema.Title = myType.Name;
            pschema.Title = pType.Name;
            Console.WriteLine(schema.Title);
            Console.WriteLine(pschema.Title);
            Console.ReadLine();

            //write to a file 
            var writer = new StringWriter();
            var jsonTextWriter = new JsonTextWriter(writer);
            schema.WriteTo(jsonTextWriter);
            dynamic parsedJson = JsonConvert.DeserializeObject(writer.ToString());
            var prettyString = JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
            var fileWriter = new StreamWriter("MySchema.txt");
            //fileWriter.WriteLine(schema.Title);
            //fileWriter.WriteLine(new string('-', schema.Title.Length));
            Console.WriteLine("JSON Schema file created successfully!");
            Console.ReadLine();
            fileWriter.WriteLine(prettyString);
            fileWriter.Close();



            //JsonSchema schema = new JsonSchema();
            
            //schema.Type = JsonSchemaType.Object;
            //Console.WriteLine(schema.Properties);
            //schema.Properties = new Dictionary<string, JsonSchema>
            //{
            //    {"name", new JsonSchema {Type = JsonSchemaType.String}},
            //    {
            //        "hobbies", new JsonSchema
            //        {
            //            Type = JsonSchemaType.Array,
            //            Items = new List<JsonSchema> {new JsonSchema {Type = JsonSchemaType.String}}
            //        }
            //    },
            //};
            //string schemaJson = schema.ToString();
            //Console.WriteLine(schemaJson);
            //Console.ReadLine();
        }


        private const string Cyrillic = "Cyrillic";
        private const string Nullable = "?";
        static void FromSchemaToPoco()
        {
            string schemaText;
            using (var r = new StreamReader("MySchema.txt"))
            {
                schemaText = r.ReadToEnd();
            }
            var jsonSchema = JsonSchema.Parse(schemaText);
            //Console.WriteLine(jsonSchema);
            //Console.ReadLine();
            if (jsonSchema != null)
            {
                var sb = ConvertJsonSchemaToPocos(jsonSchema);
                var code = sb.ToString();
                TextWriter poco = File.CreateText(@"poco.txt");
                poco.WriteLine(code);
                Console.WriteLine("Text file on poco created!");
                poco.Close();
                Console.WriteLine(Console.Read());
            }
        }

        private static StringBuilder ConvertJsonSchemaToPocos(JsonSchema schema)
    {
        if(schema.Type == null)
            throw new Exception("Schema does not specify a type.");
 
        var sb = new StringBuilder();
 
        switch (schema.Type)
        {
            case JsonSchemaType.Object:
                sb.Append(ConvertJsonSchemaObjectToPoco(schema));
                break;
 
            case JsonSchemaType.Array:
                foreach (var item in schema.Items.Where(x => x.Type.HasValue && x.Type == JsonSchemaType.Object))
                {
                    sb.Append(ConvertJsonSchemaObjectToPoco(item));
                }
                break;
        }
 
        return sb;
    }
 
    private static StringBuilder ConvertJsonSchemaObjectToPoco(JsonSchema schema)
    {
        string className;
        return ConvertJsonSchemaObjectToPoco(schema, out className);
    }
 
    private static StringBuilder ConvertJsonSchemaObjectToPoco(JsonSchema schema, out string className)
    {
        var sb = new StringBuilder();
        sb.Append("public class ");

        if (schema.Title != null)
            className = GenerateSlug(schema.Title);
        else
            className = String.Format("Poco_{0}",Guid.NewGuid().ToString().Replace("-", string.Empty));
 
        sb.Append(className);
        sb.AppendLine(" {");
 
        foreach (var item in schema.Properties)
        {
            sb.Append("public ");
            sb.Append(GetClrType(item.Value, sb));
            sb.Append(" ");
            sb.Append(item.Key.Trim());
            sb.AppendLine(" { get; set; }");
        }
 
        sb.AppendLine("}");
        return sb;
    }
 
    private static string GenerateSlug(string phrase)
    {
        var str = RemoveAccent(phrase);
        str = Regex.Replace(str, @"[^a-zA-Z\s-]", ""); // invalid chars
        str = Regex.Replace(str, @"\s+", " ").Trim(); // convert multiple spaces into one space, trim
        str = Regex.Replace(str, @"\s", "_"); // convert spaces to underscores
        return str;
    }
 
    private static string RemoveAccent(string txt)
    {
        var bytes = Encoding.GetEncoding(Cyrillic).GetBytes(txt);
        return Encoding.ASCII.GetString(bytes);
    }
 
    private static string GetClrType(JsonSchema jsonSchema, StringBuilder sb)
    {
        switch (jsonSchema.Type)
        {
            case JsonSchemaType.Array:
                if(jsonSchema.Items.Count == 0)
                    return "IEnumerable<object>";
                if (jsonSchema.Items.Count == 1)
                    return String.Format("IEnumerable<{0}>", GetClrType(jsonSchema.Items.First(), sb));
                throw new Exception("Not sure what type this will be.");
 
            case JsonSchemaType.Boolean:
                return String.Format("bool{0}", jsonSchema.Required.HasValue && jsonSchema.Required.Value ? string.Empty : Nullable);
 
            case JsonSchemaType.Float:
                return String.Format("float{0}", jsonSchema.Required.HasValue && jsonSchema.Required.Value ? string.Empty : Nullable);
 
            case JsonSchemaType.Integer:
                return String.Format("int{0}", jsonSchema.Required.HasValue && jsonSchema.Required.Value ? string.Empty : Nullable);
 
            case JsonSchemaType.String:
                return "string";
 
            case JsonSchemaType.Object:
                    string className;
                    sb.Insert(0, ConvertJsonSchemaObjectToPoco(jsonSchema, out className));
                return className;
 
            case JsonSchemaType.None:
            case JsonSchemaType.Null:
            default:
                return "object";
        }
    }
}
    
}
