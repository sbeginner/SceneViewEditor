using System.Linq;
using System.Collections.Generic;
using Editor.SceneViewEditor.Source.Interfaces;
using UnityEditor;
using UnityEngine;

namespace Editor.SceneViewEditor.Source.Windows
{
    public class WindowHandler
    {
        private readonly Camera _camera;
        private readonly Window.Factory _factory;
        private static readonly List<IWindow> CustomWindows = new List<IWindow>();
        private readonly Vector2 _defaultWindowSize = new Vector2(160, 160);

        public WindowHandler(Window.Factory factory, Camera camera)
        {
            _factory = factory;
            _camera = camera;
        }

        public void OnSceneSelectedObjects()
        {
            var transforms = Selection.GetTransforms(SelectionMode.Unfiltered);
            foreach (var transform in transforms)
            {
                var id = transform.GetInstanceID();

                var targetWindow = CustomWindows.Find(window => window.Id == id);
                if (targetWindow != null)
                {
                    targetWindow.IsActive = true;
                    continue;
                }

                CreateWindow(transform);
            }

            Selection.SetActiveObjectWithContext(transforms.LastOrDefault(), null);
        }

        public void OnSceneGUIUpdate()
        {
            for (var i = CustomWindows.Count - 1; i >= 0; i--)
            {
                if (CustomWindows[i].IsDestroyable)
                {
                    CustomWindows.RemoveAt(i);
                    continue;
                }

                CustomWindows[i].Display();
            }
        }

        private void CreateWindow(Transform transform)
        {
            var position = FindTopRightCornerPositionInTransform(transform);
            var windowSize = new Rect(position, _defaultWindowSize);
            var scrollPosition = new Vector2(0, int.MaxValue);
            var settings = new Window.Settings(windowSize, scrollPosition, transform);

            var customWindow = _factory.Create(settings, HandleNextWindow);
            CustomWindows.Add(customWindow);
        }

        private Vector2 FindTopRightCornerPositionInTransform(Transform transform)
        {
            var position = transform.position;

            if (transform.TryGetComponent<RectTransform>(out var rectTransform))
            {
                var corner = new Vector3[4];
                rectTransform.GetWorldCorners(corner); // Unity api: out variable.
                return WorldToScreenPoint(corner[2]);
            }

            if (transform.TryGetComponent<Renderer>(out var renderer))
            {
                var halfBoundsSize = renderer.bounds.size * .5f;
                var newPosition = transform.position + halfBoundsSize;
                return WorldToScreenPoint(newPosition);
            }

            return WorldToScreenPoint(position);
        }

        private Vector2 WorldToScreenPoint(Vector2 transformPosition)
        {
            // GUI Y origin is at the screen top, while screen coordinates start at the bottom.
            var offset = new Vector2(5, 21); // The fixed offset-Y is for the upper toolbar in scene view.
            var position = _camera.WorldToScreenPoint(transformPosition);
            var height = _camera.pixelHeight - position.y;

            var guiX = Mathf.Clamp(position.x,
                offset.x, _camera.pixelWidth - offset.x);
            var guiY = Mathf.Clamp(height + offset.y,
                offset.y, _camera.pixelHeight - offset.y);

            return new Vector2(guiX, guiY);
        }

        private void HandleNextWindow(IWindow ignoreWindow)
        {
            var position = FindTopRightCornerPositionInTransform(ignoreWindow.Transform);
            var windowSize = new Rect(position, _defaultWindowSize);
            ignoreWindow.SetWindowSize(windowSize);

            var window = GetNextHandleWindow(ignoreWindow);
            Selection.SetActiveObjectWithContext(window?.Transform, null);
        }

        private static IWindow GetNextHandleWindow(IWindow ignoreWindow)
        {
            return CustomWindows.FindLast(window => window != ignoreWindow &&
                                                    window.IsActive &&
                                                    !window.IsDestroyable);
        }
    }
}