using System.Net.Sockets;
using System.Text;

namespace RetroCamera.Utilities
{
    public static class SpamClient
    {
        public static void SendSpamCommand(string command)
        {
            try
            {
                using (TcpClient client = new TcpClient("127.0.0.1", 5000))
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] data = Encoding.UTF8.GetBytes(command);
                    stream.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                Plugin.LogInstance.LogError($"[SpamClient] Failed to send command: {ex.Message}");
            }
        }
    }
}
