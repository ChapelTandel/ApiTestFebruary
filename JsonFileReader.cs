using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace ApiTestFebruary
{
    internal class JsonFileReader
    {
        public static string GetValueFromFile(string fileName, string key)
        {
            var allText =
                File.ReadAllText($"{Environment.CurrentDirectory}\\TestData\\{fileName}");

            var keyValuePair = JsonConvert.DeserializeObject<Dictionary<string, string>>(allText);

            return keyValuePair[key];
        }
    }
}