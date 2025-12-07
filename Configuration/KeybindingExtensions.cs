using UnityEngine;

namespace RetroCamera.Configuration
{
    internal static class KeybindingExtensions
    {
        internal static bool IsKeyDown(this Keybinding keybinding)
        {
            return Input.GetKey(keybinding.Primary);
        }

        internal static bool IsKeyPressed(this Keybinding keybinding)
        {
            return Input.GetKeyDown(keybinding.Primary);
        }

        internal static bool IsKeyUp(this Keybinding keybinding)
        {
            return Input.GetKeyUp(keybinding.Primary);
        }
    }
}
