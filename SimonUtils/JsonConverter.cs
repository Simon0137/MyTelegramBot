namespace SimonUtils
{
    using Newtonsoft.Json;

    public static class JsonConverter
    {
        /// <summary>
        /// Converts object value to a json-file 
        /// </summary>
        /// <param name="obj">Current object</param>
        /// <param name="path">Path to the json-file</param>
        /// <param name="createNewFile">If the json-file does not exist, create new one?</param>
        public static void ToJsonFile(this object obj, string path)
        {
            string json = ToJson(obj);
            if (!File.Exists(path))
            {
                File.Create(path);
            }
            File.WriteAllText(path, json);
        }
        /// <summary>
        /// Converts object value to string
        /// </summary>
        /// <param name="obj">Current object</param>
        /// <returns>Returns a json-string</returns>
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        /// <summary>
        /// Converts value into a json-file to a specified type
        /// </summary>
        /// <typeparam name="T">Current specified type</typeparam>
        /// <param name="path">Path to the json-file</param>
        /// <returns>Returns value from the json-file</returns>
        public static T? FromJsonFile<T>(string path, T? defaultValue = default(T))
        {
            if (!File.Exists(path))
            {
                return defaultValue;
            }
            else
            {
                string json = File.ReadAllText(path);
                return FromJson<T>(json);
            }
        }
        /// <summary>
        /// Converts value into a json-string to a specified type
        /// </summary>
        /// <typeparam name="T">Current specified type</typeparam>
        /// <param name="json">Current json-string</param>
        /// <returns>Returns value from the json-string</returns>
        public static T? FromJson<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}