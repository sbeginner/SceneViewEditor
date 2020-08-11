using UnityEngine;

namespace Editor.SceneViewEditor.Source.Extensions
{
    public static class StringExtensions
    {
        public static void CopyToClipboard(this string text)
        {
            GUIUtility.systemCopyBuffer = text;
        }
    }
}