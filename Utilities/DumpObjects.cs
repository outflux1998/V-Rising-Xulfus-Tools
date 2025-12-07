using System.IO;
using BepInEx;
using UnityEngine;

namespace RetroCamera.Utils
{
    public static class DumpObjects
    {
        /// <summary>
        /// Faz um dump de todos os GameObjects ativos na cena e salva em um arquivo .txt.
        /// </summary>
        /// <param name="outputFileName">Nome do arquivo de saída (padrão: "ObjectDump.txt").</param>
        public static void DumpAllObjects(string outputFileName = "ObjectDump.txt")
        {
            string outputPath = Path.Combine(Paths.PluginPath, outputFileName);

            using (StreamWriter writer = new StreamWriter(outputPath, false))
            {
                GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

                foreach (GameObject obj in allObjects)
                {
                    string path = GetGameObjectPath(obj);
                    string info = $"[Name]: {obj.name} | [Active]: {obj.activeSelf} | [Layer]: {LayerMask.LayerToName(obj.layer)} | [Path]: {path}";
                    writer.WriteLine(info);
                }
            }

            Debug.Log($"[ObjectDumper] Dump concluído! Arquivo salvo em: {outputPath}");
        }

        private static string GetGameObjectPath(GameObject obj)
        {
            string path = "/" + obj.name;
            Transform current = obj.transform;

            while (current.parent != null)
            {
                current = current.parent;
                path = "/" + current.name + path;
            }

            return path;
        }
    }
}
