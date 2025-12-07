using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace RetroCamera.Utilities
{
    public static class SpamServerManager
    {
        private static Process spamServerProcess;

        private const string ServerAddress = "127.0.0.1";
        private const int ServerPort = 5000;

        public static void StartSpamServer()
        {
            if (spamServerProcess != null && !spamServerProcess.HasExited)
            {
                Plugin.LogInstance.LogInfo("[SpamServerManager] Spam server já está rodando.");
                return;
            }

            string exePath = Path.Combine(BepInEx.Paths.PluginPath, "MouseSpammerServer.exe");

            if (!File.Exists(exePath))
            {
                Plugin.LogInstance.LogError($"[SpamServerManager] Spam server exe não encontrado: {exePath}");
                return;
            }

            spamServerProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    WorkingDirectory = BepInEx.Paths.PluginPath,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            spamServerProcess.Start();

            Plugin.LogInstance.LogInfo("[SpamServerManager] Spam server iniciado com sucesso.");
        }

        public static void StopSpamServer()
        {
            if (spamServerProcess != null && !spamServerProcess.HasExited)
            {
                spamServerProcess.Kill();
                spamServerProcess.Dispose();
                spamServerProcess = null;
                Plugin.LogInstance.LogInfo("[SpamServerManager] Spam server encerrado.");
            }
        }

        private static void SendCommand(string command)
        {
            try
            {
                using (TcpClient client = new TcpClient(ServerAddress, ServerPort))
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] data = Encoding.UTF8.GetBytes(command);
                    stream.Write(data, 0, data.Length);
                }
            }
            catch (SocketException ex)
            {
                Plugin.LogInstance.LogError($"[SpamServerManager] Erro ao enviar comando '{command}': {ex.Message}");
            }
        }

        public static void StartSpam()
        {
            SendCommand("start");
        }

        public static void StopSpam()
        {
            SendCommand("stop");
        }

        public static void PressQ()
        {
            SendCommand("scrollClick");
        }
    }
}
