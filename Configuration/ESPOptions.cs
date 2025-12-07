using RetroCamera.Configuration;

namespace RetroCamera.ESP
{
    internal static class ESPOptions
    {
        public static Toggle ESPEnabled;
        public static Toggle ShowPlayers;
        public static Toggle ShowContainers;
        public static Toggle ShowBloodCarriers;
        public static Slider MinBloodQualitySlider;
        public static Slider AimbotSmoothingSlider;
        public static Toggle AntiCounter;
        public static Toggle AntiCounterMethod2;
        public static float MinBloodQuality => MinBloodQualitySlider?.Value ?? 0f;

        public static float AimbotSmoothing => AimbotSmoothingSlider?.Value ?? 0f;
    }
}
