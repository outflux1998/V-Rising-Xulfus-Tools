using FMOD;
using System.Collections;
using UnityEngine;

namespace RetroCamera.ESP
{
    public class MinimapTweaks : MonoBehaviour
    {
        private static bool minimapFogRemoved = false;
        private static bool fullScreenEffectsRemoved = false;

        public static void RemoveMinimapFog()
        {
            GameObject obj = GameObject.Find("/HUDCanvas(Clone)/HUDClockCanvas/HUDMinimap/MiniMapParent(Clone)/Root/Panel/Mask/CurseDebuffVisualization");
            if (obj != null)
            {
                obj.transform.position = new Vector3(99999f, 99999f, 1f);
                Debug.Log("[TweakUtility] Minimap fog removed.");
                minimapFogRemoved = true;
            }
            else
            {
                Debug.LogWarning("[TweakUtility] Minimap fog object not found.");
            }
        }

        public static void RemoveFullScreenEffects()
        {
            GameObject obj = GameObject.Find("FullscreenEffects");
            if (obj != null)
            {
                obj.SetActive(false);
                Debug.Log("[TweakUtility] FullScreenEffects disabled.");
                fullScreenEffectsRemoved = true;
            }
            else
            {
                Debug.LogWarning("[TweakUtility] FullScreenEffects object not found.");
            }
        }

        public static void ToggleOnlyDay()
        {
            GameObject obj = GameObject.Find("OnlyDaylight");
            if (obj != null)
            {
                obj.SetActive(true);
                Debug.Log("[TweakUtility] OnlyDaylight mode activated.");
            }
            else
            {
                Debug.LogWarning("[TweakUtility] OnlyDaylight object not found.");
            }
        }
    }
}
