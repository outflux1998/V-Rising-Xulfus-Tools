using ProjectM;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

namespace RetroCamera.ESP
{
    internal static class WhitelistManager
    {
        public static readonly HashSet<string> WhitelistNames = new();
        private static readonly HttpClient httpClient = new();

        public static async void LoadWhitelist()
        {
            try
            {
                //string url = "http://177.177.111.231:8080/whitelist"; // Endpoint do seu servidor
                //string url = "http://localhost:8080/whitelist"; // Endpoint do seu servidor
                string url = "http://159.89.49.145:8080/whitelist"; // Endpoint do seu servidor

                var response = await httpClient.GetStringAsync(url);

                // Parse manual do JSON: ["Outflux","Xulfus","Outlaw"]
                string trimmed = response.Trim()
                                         .Replace("[", "")
                                         .Replace("]", "")
                                         .Replace("\"", "");

                string[] names = trimmed.Split(',');

                WhitelistNames.Clear();
                foreach (var name in names)
                {
                    string clean = name.Trim();
                    if (!string.IsNullOrEmpty(clean))
                        WhitelistNames.Add(clean);
                }
            }
            catch (Exception ex)
            {
                Core.Log.LogError($"[ESP] Erro ao carregar lista de players: {ex.Message}");
            }
        }
    }
}
