using UnityEngine;

namespace Editor.SceneViewEditor.Source.Windows
{
    public partial class Window
    {
        public class Settings
        {
            public int Id => Transform.GetInstanceID();
            public bool IsActive { get; set; }
            public bool IsFocused { get; set; }
            public Rect WindowSize { get; set; }
            public Vector2 ScrollPosition { get; set; }
            public Transform Transform { get; }
            public string TransformName => _transformNameCache ?? (_transformNameCache = Transform.name);
            private string _transformNameCache;

            private Settings(bool isActive, Rect windowSize, Vector2 scrollPosition, Transform transform)
            {
                IsActive = isActive;
                WindowSize = windowSize;
                ScrollPosition = scrollPosition;
                Transform = transform;
            }

            public Settings(Rect windowSize, Vector2 scrollPosition, Transform transform)
                : this(true, windowSize, scrollPosition, transform)
            {
            }
        }
    }
}