using UnityEngine;

namespace RetroCamera.ESP
{
    internal static class Overlay
    {
        private static readonly string EspVersion = "v0.8";

        public static void DrawOverlay()
        {
            Vector2 position = new Vector2(80f, Screen.height - 25f); // Extra padding left + top
            string label = $"Xulfus ({EspVersion})";

            Primitives.DrawString(position, label, Color.cyan);
        }
    }
}
