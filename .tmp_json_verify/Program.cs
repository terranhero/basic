using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonVerify
{
    // 与 PersistentService.database.cs 中实现的算法副本，用于验证
    internal static class Program
    {
        private static int failures = 0;

        private static void Main()
        {
            Diagnose.Run();
            // 用例 1：完整结构（含行内注释与其它配置节）
            string sample = "{\r\n" +
                "\t\"AllowedHosts\": \"*\",\r\n" +
                "\t\"Connections\": {\r\n" +
                "\t\t\"DefaultName\": \"SJEC\",\r\n" +
                "\t\t\"Connections\": {\r\n" +
                "\t\t\t\"LOCAL\": {\r\n" +
                "\t\t\t\t\"ConnectionType\": \"NewSqlConnection\",\r\n" +
                "\t\t\t\t\"Version\": 12,\r\n" +
                "\t\t\t\t\"Data Source\": \"(local)\"\r\n" +
                "\t\t\t},\r\n" +
                "\t\t\t\"SJEC\": {\r\n" +
                "\t\t\t\t\"ConnectionType\": \"NewSqlConnection\",\r\n" +
                "\t\t\t\t\"Version\": 12,\r\n" +
                "\t\t\t\t\"Data Source\": \"(local)\"\r\n" +
                "\t\t\t}\r\n" +
                "\t\t}\r\n" +
                "\t},\r\n" +
                "\t\"Loggers\": {\r\n" +
                "\t\t\"Mode\": \"Monthly\", //表示日志文件记录级别分(Daily / Weekly / Monthly)\r\n" +
                "\t\t\"Error\": {\r\n" +
                "\t\t\t\"SaveType\": \"LocalFile\", //日志保存类型\r\n" +
                "\t\t\t\"Enabled\": true //该级别日志配置信息是否有效\r\n" +
                "\t\t}\r\n" +
                "\t}\r\n" +
                "}\r\n";

            var fields = NewFields("SqlConnection");
            Run("用例1-完整结构", sample, "Connection_2", fields, true, "Connection_2");

            // 用例 2：DefaultName 已存在（不需要设置）
            Run("用例2-已存在DefaultName", sample, "Connection_2", fields, false, "SJEC");

            // 用例 3：空文件
            Run("用例3-空文件", "{}", "Connection_0", fields, true, "Connection_0");

            // 用例 4：只有 Connections 节点，缺内层
            Run("用例4-缺内层Connections", "{\r\n\t\"Connections\": {\r\n\t\t\"DefaultName\": \"\"\r\n\t}\r\n}", "Connection_0", fields, true, "Connection_0");

            // 用例 5：无 Connections 节点
            Run("用例5-无Connections", "{\r\n\t\"AllowedHosts\": \"*\"\r\n}", "Connection_0", fields, true, "Connection_0");

            // 用例 6：值中含引号/反斜杠/等号、换行
            var tricky = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("ConnectionType", "SqlConnection"),
                new KeyValuePair<string, string>("Version", "10"),
                new KeyValuePair<string, string>("Password", "ab\"c\\d=e"),
                new KeyValuePair<string, string>("Note", "行1\r\n行2")
            };
            Run("用例6-转义", sample, "Connection_9", tricky, false, "SJEC");

            Console.WriteLine(failures == 0 ? "\n全部用例通过" : string.Concat("\n失败用例数：", failures));
            Environment.Exit(failures == 0 ? 0 : 1);
        }

        private static List<KeyValuePair<string, string>> NewFields(string connectionType)
        {
            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("ConnectionType", connectionType),
                new KeyValuePair<string, string>("Version", "10"),
                new KeyValuePair<string, string>("Application Name", "HRMS-TEST"),
                new KeyValuePair<string, string>("Data Source", "(local)"),
                new KeyValuePair<string, string>("User ID", "sa"),
                new KeyValuePair<string, string>("Password", "C/1Shp55C14b0TtEhs87bg==")
            };
        }

        private static void Run(string title, string json, string connectionName,
            List<KeyValuePair<string, string>> fields, bool setDefaultName, string defaultName)
        {
            Console.WriteLine("================ " + title + " ================");
            string updated = SaveJson(json, connectionName, fields, setDefaultName, defaultName);
            if (updated == null)
            {
                Console.WriteLine("[FAIL] 写入返回 null");
                failures++;
                return;
            }

            // 校验 JSON 合法性（允许 JSONC 注释）
            try
            {
                var options = new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true };
                using (JsonDocument doc = JsonDocument.Parse(updated, options))
                {
                    JsonElement root = doc.RootElement;
                    JsonElement connections = root.GetProperty("Connections");
                    if (setDefaultName && connections.GetProperty("DefaultName").GetString() != defaultName)
                    {
                        Console.WriteLine("[FAIL] DefaultName 不正确");
                        failures++;
                    }
                    JsonElement item = connections.GetProperty("Connections").GetProperty(connectionName);
                    if (item.GetProperty("ConnectionType").GetString() != "SqlConnection")
                    {
                        Console.WriteLine("[FAIL] ConnectionType 不正确");
                        failures++;
                    }
                    string expectedPassword = null;
                    foreach (KeyValuePair<string, string> pair in fields)
                    {
                        if (pair.Key == "Password") { expectedPassword = pair.Value; }
                    }
                    if (expectedPassword != null && item.GetProperty("Password").GetString() != expectedPassword)
                    {
                        Console.WriteLine(string.Concat("[FAIL] Password 不正确：", item.GetProperty("Password").GetString()));
                        failures++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[FAIL] 输出不是合法 JSON：" + ex.Message);
                failures++;
                Console.WriteLine(updated);
                return;
            }

            // 校验原文件其它内容保持原样
            if (json.Contains("Loggers") && !updated.Contains("Loggers"))
            {
                Console.WriteLine("[FAIL] 其它配置节丢失");
                failures++;
            }
            if (json.Contains("//日志保存类型") && !updated.Contains("//日志保存类型"))
            {
                Console.WriteLine("[FAIL] 注释丢失");
                failures++;
            }

            Console.WriteLine(updated);
            Console.WriteLine("[OK]");
        }

        // ==================== 以下为被测算法副本 ====================

        private static string SaveJson(string json, string connectionName,
            List<KeyValuePair<string, string>> fields, bool setDefaultName, string defaultName)
        {
            JObject root = LoadJsonObject(json);
            JObject connections = GetOrCreateObject(root, "Connections");
            JObject items = GetOrCreateObject(connections, "Connections");

            HashSet<string> connectionNames = new HashSet<string>(
                items.Properties().Select(p => p.Name), StringComparer.OrdinalIgnoreCase);
            // 副本中直接使用传入名称，便于验证（真实代码此处生成 Connection_{index}）

            JObject token = new JObject();
            foreach (KeyValuePair<string, string> pair in fields)
            {
                if (string.Equals(pair.Key, "Version", StringComparison.OrdinalIgnoreCase) && int.TryParse(pair.Value, out int version))
                {
                    token[pair.Key] = version;
                }
                else
                {
                    token[pair.Key] = pair.Value;
                }
            }
            items[connectionName] = token;

            if (setDefaultName && string.IsNullOrWhiteSpace(connections.Value<string>("DefaultName")))
            {
                connections.AddFirst(new JProperty("DefaultName", defaultName));
            }

            string newLine = json.Contains("\r\n") ? "\r\n" : Environment.NewLine;
            return SerializeJson(root, newLine);
        }

        private static JObject LoadJsonObject(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) { return new JObject(); }
            JsonLoadSettings settings = new JsonLoadSettings { CommentHandling = CommentHandling.Load };
            using (StringReader textReader = new StringReader(json))
            using (JsonTextReader jsonReader = new JsonTextReader(textReader) { DateParseHandling = DateParseHandling.None })
            {
                return JObject.Load(jsonReader, settings);
            }
        }

        private static JObject GetOrCreateObject(JObject parent, string name)
        {
            if (parent[name] is JObject existing) { return existing; }
            JObject created = new JObject();
            parent[name] = created;
            return created;
        }

        private static string SerializeJson(JObject root, string newLine)
        {
            StringBuilder buffer = new StringBuilder();
            using (StringWriter textWriter = new StringWriter(buffer))
            {
                using (JsonTextWriter jsonWriter = new JsonTextWriter(textWriter))
                {
                    jsonWriter.Formatting = Formatting.Indented;
                    jsonWriter.Indentation = 1;
                    jsonWriter.IndentChar = '\t';
                    root.WriteTo(jsonWriter);
                }
            }
            string normalized = buffer.ToString().Replace("\r\n", "\n").Replace("\r", "\n");
            if (newLine == "\r\n") { normalized = normalized.Replace("\n", "\r\n"); }
            return string.Concat(normalized.TrimEnd('\r', '\n'), newLine);
        }
    }
}
