using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Xml.Linq;

namespace ResxToJson
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var path = args.Any() ? args[0] : Directory.GetCurrentDirectory();

            foreach (var file in Directory.GetFiles(path, "*.resx"))
            {
                var obj = XElement.Parse(File.ReadAllText(file))
                    .Elements("data")
                    .ToDictionary(el => el.Attribute("name").Value, el => el.Element("value").Value.Trim());

                var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
                });

                Console.WriteLine(json);

                var fileName = Path.GetFileNameWithoutExtension(file);
                var culture = fileName.Substring(fileName.IndexOf('.') + 1).Replace(".resx", "");
                if (string.IsNullOrEmpty(culture) || fileName.IndexOf('.') == -1) culture = "en";
                File.WriteAllText(Path.Combine(Path.GetDirectoryName(file), $"{culture}.json"), json, Encoding.UTF8);
            }

            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}