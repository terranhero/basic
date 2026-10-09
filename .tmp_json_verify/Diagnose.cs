using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonVerify
{
    internal static class Diagnose
    {
        public static void Run()
        {
            string json = "{\r\n" +
                "\t\"AllowedHosts\": \"*\",\r\n" +
                "\t\"Connections\": {\r\n" +
                "\t\t\"DefaultName\": \"SJEC\",\r\n" +
                "\t\t\"Connections\": {\r\n" +
                "\t\t\t\"LOCAL\": {\r\n" +
                "\t\t\t\t\"Data Source\": \"(local)\"\r\n" +
                "\t\t\t}\r\n" +
                "\t\t}\r\n" +
                "\t},\r\n" +
                "\t\"Loggers\": {\r\n" +
                "\t\t\"Mode\": \"Monthly\" //注释\r\n" +
                "\t}\r\n" +
                "}\r\n";

            using (StringReader textReader = new StringReader(json))
            using (JsonTextReader reader = new JsonTextReader(textReader))
            {
                while (reader.Read())
                {
                    string value = reader.Value == null ? "" : reader.Value.ToString();
                    int offset = OffsetOf(json, reader.LineNumber, reader.LinePosition);
                    string at = offset >= 0 && offset < json.Length ? json[offset].ToString() : "EOF";
                    Console.WriteLine(string.Concat(
                        reader.TokenType, " | path=", reader.Path,
                        " | line=", reader.LineNumber, " pos=", reader.LinePosition,
                        " | offset=", offset, " char='", at, "'", " | value=", value));
                }
            }
            Environment.Exit(0);
        }

        private static int OffsetOf(string text, int line, int position)
        {
            if (line <= 0 || position <= 0) { return -1; }
            int offset = 0, currentLine = 1;
            while (offset < text.Length && currentLine < line)
            {
                if (text[offset] == '\n') { currentLine++; }
                offset++;
            }
            return offset + (position - 1);
        }
    }
}
