using System;
using System.IO;
using UnityEngine;

namespace ImageSearch.Bootstrap
{
    public static class ApiKeyProvider
    {
        private const string EnvironmentVariableName = "PIXABAY_API_KEY";
        private const string LocalKeyFileRelativePath = "Secrets~/pixabay-api-key.txt";

        public static bool TryGetPixabayApiKey(out string apiKey)
        {
            var fromEnv = Environment.GetEnvironmentVariable(EnvironmentVariableName);
            if (!string.IsNullOrWhiteSpace(fromEnv))
            {
                apiKey = fromEnv.Trim();
                return true;
            }

#if UNITY_EDITOR
            var path = Path.Combine(Application.dataPath, LocalKeyFileRelativePath);
            if (File.Exists(path))
            {
                var fromFile = File.ReadAllText(path).Trim();
                if (!string.IsNullOrWhiteSpace(fromFile))
                {
                    apiKey = fromFile;
                    return true;
                }
            }
#endif

            apiKey = null;
            return false;
        }
    }
}
