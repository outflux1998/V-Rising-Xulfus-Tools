using System;
using System.IO;
using UnityEngine;
using BepInEx;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RetroCamera.ESP
{
    internal static class Auth
    {
        private const string FileName = "esp_key.json";
        private static DateTime nextCheck = DateTime.MinValue;
        private static readonly string KeyPath = Path.Combine(Paths.ConfigPath, FileName);

        public static bool IsAuthorized { get; private set; } = false;

        private class KeyData
        {
            public string KeyId;

            public static KeyData FromJson(string json)
            {
                var data = new KeyData();
                try
                {
                    json = json.Replace("{", "").Replace("}", "").Replace("\"", "");
                    var parts = json.Split(',');

                    foreach (var part in parts)
                    {
                        var kv = part.Split(':');
                        if (kv.Length != 2) continue;

                        var key = kv[0].Trim();
                        var value = kv[1].Trim();

                        if (key == "KeyId")
                            data.KeyId = value;
                    }
                }
                catch (Exception e)
                {
                    Core.Log.LogError($"[AUTH] Manual parse failed: {e.Message}");
                    return null;
                }
                return data;
            }
        }

        private static readonly HttpClient httpClient = new HttpClient();

        public static async void CheckAuth()
        {
            try
            {
                Core.Log.LogInfo($"[AUTH] Looking for key at: {KeyPath}");

                if (!File.Exists(KeyPath))
                {
                    Core.Log.LogWarning("[ESP] Auth key file not found.");
                    IsAuthorized = false;
                    return;
                }

                var json = File.ReadAllText(KeyPath);
                var key = KeyData.FromJson(json);

                if (key == null || string.IsNullOrEmpty(key.KeyId))
                {
                    Core.Log.LogWarning("[ESP] Invalid or missing KeyId.");
                    IsAuthorized = false;
                    return;
                }

                string hwid = GetBaseBoardSerial();

                var payload = new
                {
                    key = key.KeyId,
                    hwid = hwid
                };

                string jsonPayload = System.Text.Json.JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                //var response = await httpClient.PostAsync("http://177.177.111.231:8080/auth", content);
                //var response = await httpClient.PostAsync("http://localhost:8080/auth", content);
                var response = await httpClient.PostAsync("http://159.89.49.145:8080/auth", content);
                string responseText = await response.Content.ReadAsStringAsync();

                IsAuthorized = responseText.Trim().ToLower() == "true";

                Core.Log.LogInfo($"[ESP] Server authorization returned: {IsAuthorized}");

                if (IsAuthorized == true)
                    nextCheck = DateTime.UtcNow.AddHours(1);

            }
            catch (Exception ex)
            {
                Core.Log.LogError($"[ESP] Auth check failed: {ex.Message}");
                IsAuthorized = false;
            }
        }

        private static string GetDiskSerial()
        {
            try
            {
                var process = new System.Diagnostics.Process();
                process.StartInfo.FileName = "wmic";
                process.StartInfo.Arguments = "diskdrive get SerialNumber";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length >= 2)
                {
                    string serial = lines[1].Trim();
                    if (!string.IsNullOrEmpty(serial))
                        return serial;
                }
            }
            catch (Exception ex)
            {
                Core.Log.LogError($"[AUTH] Erro ao obter Disk ID via wmic: {ex.Message}");
            }

            return "UNKNOWN_DISK";
        }

        private static string GetBaseBoardSerial()
        {
            try
            {
                var process = new System.Diagnostics.Process();
                process.StartInfo.FileName = "wmic";
                process.StartInfo.Arguments = "baseboard get SerialNumber";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length >= 2)
                {
                    string serial = lines[1].Trim();
                    if (!string.IsNullOrEmpty(serial))
                        return serial;
                }
            }
            catch (Exception ex)
            {
                Core.Log.LogError($"[AUTH] Erro ao obter BaseBoard Serial via wmic: {ex.Message}");
            }

            return "UNKNOWN_BASEBOARD";
        }



        public static async void VerifyAuth()
        {
            if (DateTime.UtcNow < nextCheck)
                return;

            nextCheck = DateTime.UtcNow.AddHours(1);  // próxima verificação em 1 hora

            try
            {
                if (!File.Exists(KeyPath))
                {
                    IsAuthorized = false;
                    return;
                }

                var json = File.ReadAllText(KeyPath);
                var key = KeyData.FromJson(json);

                if (key == null || string.IsNullOrEmpty(key.KeyId))
                {
                    IsAuthorized = false;
                    return;
                }

                string hwid = GetBaseBoardSerial();

                var payload = new
                {
                    key = key.KeyId,
                    hwid = hwid
                };

                string jsonPayload = System.Text.Json.JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                //var response = await httpClient.PostAsync("http://177.177.111.231:8080/auth", content);
                //var response = await httpClient.PostAsync("http://localhost:8080/auth", content);
                var response = await httpClient.PostAsync("http://159.89.49.145:8080/auth", content);
                string responseText = await response.Content.ReadAsStringAsync();

                IsAuthorized = responseText.Trim().ToLower() == "true";

                Core.Log.LogInfo($"[ESP] Periodic auth check: {IsAuthorized}");
            }
            catch (Exception ex)
            {
                Core.Log.LogError($"[ESP] Periodic auth check failed: {ex.Message}");
                IsAuthorized = false;
            }
        }
    }
}
