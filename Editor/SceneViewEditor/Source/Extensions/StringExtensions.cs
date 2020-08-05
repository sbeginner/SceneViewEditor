using UnityEngine;

namespace Editor.SceneViewEditor.Source.Extensions
{
    public static class StringExtensions
    {
        public static void CopyToClipboard(this string text)
        {
            var textEditor = new TextEditor();
            textEditor.text = text;
            textEditor.SelectAll();
            textEditor.Copy();
        }
    }
}