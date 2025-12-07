using System.Collections.Generic;
using UnityEngine;

namespace RetroCamera.ESP
{
    internal class Primitives
    {
        public static Dictionary<Color, Texture2D> _hollowTextures = new Dictionary<Color, Texture2D>();

        static GUIStyle StringStyle;

        public static void Init()
        {
            if (StringStyle != null)
                return;

            StringStyle = new GUIStyle
            {
                fontSize = 19,  // Increased size for better visibility
                //fontStyle = FontStyle.Bold,  // Ensure BOLD
                alignment = TextAnchor.MiddleCenter,
                normal = new GUIStyleState
                {
                    textColor = Color.white
                },
                font = GUI.skin.font // Use game's default font
            };
        }

        public static void DrawString(Vector2 position, string label, bool centered = true)
        {
            var content = new GUIContent(label);
            var size = StringStyle.CalcSize(content);

            // Inverse position for Unity's GUI coordinates
            position.y = Screen.height - position.y;

            var upperLeft = centered ? position - size / 2f : position;

            // Use StringStyle to apply font size and bold
            GUI.Label(new Rect(upperLeft, size), content, StringStyle);
        }

        public static void DrawString(Vector2 position, string label, Color color, bool centered = true)
        {
            var oldColor = GUI.color;
            GUI.color = color;
            DrawString(position, label, centered);
            GUI.color = oldColor;
        }

        public static void DrawStringMultiline(Vector2 position, List<(string text, Color color)> lines, bool centered = true)
        {
            float lineHeight = StringStyle.lineHeight;

            for (int i = 0; i < lines.Count; i++)
            {
                var (text, color) = lines[i];
                var oldColor = GUI.color;
                GUI.color = color;

                var content = new GUIContent(text);
                var size = StringStyle.CalcSize(content);

                // Inverse position for Unity's GUI coordinates
                Vector2 linePosition = position;
                linePosition.y += i * lineHeight;

                linePosition.y = Screen.height - linePosition.y;

                var upperLeft = centered ? linePosition - size / 2f : linePosition;

                GUI.Label(new Rect(upperLeft, size), content, StringStyle);

                GUI.color = oldColor;
            }
        }


        public static void DrawLine(Vector2 start, Vector2 end, Color color, float width = 1f)
        {
            var oldColor = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(new Rect(start, end - start), Texture2D.whiteTexture);
            GUI.color = oldColor;
        }

        public static void DrawBox(Vector2 position, Vector2 size, Color color, bool invert = true)
        {
            if (invert)
            {
                position.y = Screen.height - position.y;
            }

            if (!_hollowTextures.TryGetValue(color, out Texture2D texture))
            {
                texture = CreateHollowTexture((int)size.x, (int)size.y, color);
                _hollowTextures[color] = texture;
            }

            GUI.DrawTexture(new Rect(position, size), texture);
        }

        public static Texture2D CreateHollowTexture(int width, int height, Color color)
        {
            Texture2D texture = new Texture2D(width, height);
            Color clear = new Color(0, 0, 0, 0);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool isBorder = x == 0 || x == width - 1 || y == 0 || y == height - 1;
                    texture.SetPixel(x, y, isBorder ? color : clear);
                }
            }

            texture.Apply();
            return texture;
        }
    }
}
