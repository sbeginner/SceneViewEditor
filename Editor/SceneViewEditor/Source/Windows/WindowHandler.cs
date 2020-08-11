using System.Collections.Generic;
using System.Linq;
using Editor.SceneViewEditor.Source.Interfaces;
using UnityEditor;
using UnityEngine;

namespace Editor.SceneViewEditor.Source.Windows
{
    public class WindowHandler
    {
        private readonly List<IWindow> _customWindows = new List<IWindow>();
        private readonly Window.Factory _factory;
        private readonly Camera _camera;
        private readonly Vector2 _defaultWindowSize = new Vector2(160, 160);

        public WindowHandler(Window.Factory factory, Camera camera)
        {
            _factory = factory;
            _camera = camera;
        }

        public void OnSceneSelectedObjects()
        {
            var transforms = Selection.GetTransforms(SelectionMode.Unfiltered);
            for (var i = 0; i < transforms.Length; i++)
            {
                var id = transforms[i].GetInstanceID();

                var target = _customWindows.Find(window => window.Id == id);
                if (target == null)
                {
                    CreateWindow(transforms[i]);
                }
            }

            Selection.SetActiveObjectWithContext(transforms.LastOrDefault(), null);
        }

        public void OnSceneGUIUpdate()
        {
            for (var i = 0; i < _customWindows.Count; i++)
            {
                if (!_customWindows[i].IsActive)
                {
                    if (_customWindows[i].Transform == Selection.activeTransform)
                    {
                        var window = GetNextHandleWindow(_customWindows[i]);
                        Selection.SetActiveObjectWithContext(window?.Transform, null);
                    }
            
                    _customWindows.RemoveAt(i);
                    continue;
                }
            
                _customWindows[i].Display();
            }
        }

        private IWindow GetNextHandleWindow(IWindow ignoreWindow)
        {
            return _customWindows.FindLast(window
                => window != ignoreWindow && window.IsActive);
        }

        private void CreateWindow(Transform transform)
        {
            var position = FindTopRightCornerPositionInTransform(transform);
            var windowSize = new Rect(position, _defaultWindowSize);
            var scrollPosition = new Vector2(0, int.MaxValue);
            var settings = new Window.Settings(transform.GetInstanceID(),
                true, windowSize, scrollPosition, transform);

            var customWindow = _factory.Create(settings);
            _customWindows.Add(customWindow);
        }

        private Vector2 FindTopRightCornerPositionInTransform(Transform transform)
        {
            var position = transform.position;

            var isRectTransformExist = transform.TryGetComponent<RectTransform>(out var rectTransform);
            if (isRectTransformExist)
            {
                var corner = new Vector3[4];
                rectTransform.GetWorldCorners(corner); // Unity api: out variable.
                return WorldToScreenPoint(corner[2]);
            }

            var isRendererExist = transform.TryGetComponent<Renderer>(out var renderer);
            if (isRendererExist)
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
            var offset = 21; // The fixed upper offset in scene view.
            var position = _camera.WorldToScreenPoint(transformPosition);
            var height = _camera.pixelHeight - position.y;

            var newX = Mathf.Clamp(position.x, offset, _camera.pixelWidth - offset);
            var newY = Mathf.Clamp(height + offset, offset, _camera.pixelHeight - offset);

            return new Vector2(newX, newY);
        }
    }
}