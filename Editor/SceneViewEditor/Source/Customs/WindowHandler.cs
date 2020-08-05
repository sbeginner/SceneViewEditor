using System.Collections.Generic;
using System.Linq;
using Editor.SceneViewEditor.Source.Interfaces;
using UnityEditor;
using UnityEngine;

namespace Editor.SceneViewEditor.Source.Customs
{
    public class WindowHandler
    {
        private readonly List<IWindow> _customWindows = new List<IWindow>();
        private readonly Window.Factory _factory;
        private readonly Camera _camera;

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
                    CreateCustomWindow(transforms[i]);
                }
            }

            Selection.SetActiveObjectWithContext(transforms.LastOrDefault(), null);
        }

        public void OnSceneGUIUpdate()
        {
            if (Event.current.type != EventType.Layout)
            {
                return;
            }

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

        private void CreateCustomWindow(Transform transform)
        {
            var position = _camera.WorldToScreenPoint(transform.position);
            var windowSize = new Rect(position, new Vector2(150, 160));
            var scrollPosition = new Vector2(0, int.MaxValue);
            var settings = new Window.Settings(true, windowSize, scrollPosition, transform);

            var customWindow = _factory.Create(settings);
            _customWindows.Add(customWindow);
        }
    }
}