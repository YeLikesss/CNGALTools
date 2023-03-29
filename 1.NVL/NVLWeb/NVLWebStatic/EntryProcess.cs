using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace NVLWebStatic
{
    /// <summary>
    /// 文件表
    /// </summary>
    public class FileEntry 
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string FilePath;
        /// <summary>
        /// 偏移
        /// </summary>
        public long Offset;
        /// <summary>
        /// 大小
        /// </summary>
        public uint Size;
        /// <summary>
        /// Hash值
        /// </summary>
        public string Hash;




        /// <summary>
        /// 解析文件表
        /// </summary>
        /// <param name="utf8json">utf8编码JSON流</param>
        /// <returns></returns>
        public static List<FileEntry> ParseEntry(Stream utf8json)
        {
            Dictionary<string, JsonObject> root = JsonSerializer.Deserialize<Dictionary<string, JsonObject>>(utf8json);
            return Parse(root, string.Empty);
        }

        /// <summary>
        /// 解析文件表
        /// </summary>
        /// <param name="json">JSON字符串</param>
        /// <returns></returns>
        public static List<FileEntry> ParseEntry(string json)
        {
            Dictionary<string, JsonObject> root = JsonSerializer.Deserialize<Dictionary<string, JsonObject>>(json);
            return Parse(root, string.Empty);
        }


        /// <summary>
        /// 解析JsonEntry
        /// </summary>
        /// <param name="jsonEntry"></param>
        /// <returns></returns>
        public static List<FileEntry> Parse(Dictionary<string, JsonObject> jsonEntry, string nodeName)
        {
            //文件夹处理
            if (jsonEntry.Count == 1 && jsonEntry.TryGetValue("files", out JsonObject dir))
            {
                return Parse(dir.Deserialize<Dictionary<string, JsonObject>>(), nodeName);
            }

            List<FileEntry> fileEntries = new();
            foreach (KeyValuePair<string, JsonObject> file in jsonEntry)
            {
                string path = Path.Combine(nodeName, file.Key);
                JsonObject thisObj = file.Value;

                //文件夹
                if (thisObj.TryGetPropertyValue("files", out JsonNode node))
                {
                    fileEntries.AddRange(Parse(node.Deserialize<Dictionary<string, JsonObject>>(), path));
                }
                else
                {
                    //文件
                    FileEntry entry = new()
                    {
                        FilePath = path,
                        Size = thisObj["size"].GetValue<uint>(),
                        Offset = long.Parse(thisObj["offset"].GetValue<string>()),
                        Hash = thisObj["hash"].GetValue<string>()
                    };
                    fileEntries.Add(entry);
                }
            }
            return fileEntries;
        }
    }
}
