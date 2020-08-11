using Editor.SceneViewEditor.Source.Extensions;
using Editor.SceneViewEditor.Source.Interfaces;
using UnityEditor;
using UnityEngine;

namespace Editor.SceneViewEditor.Source.Windows
{
    public class Window : IWindow
    {
        public int Id => _settings.Id;

        public bool IsActive => _settings.IsActive && Transform != null;

        public Transform Transform => _settings.Transform;

        private static readonly Rect CloseButtonPosition = new Rect(140, 5, 15, 15);
        private static bool _isCloseWindowExecuted;
        private static bool _isEscapeKeyPressed;
        private readonly Settings _settings;
        private bool _isFocused;

        private Window(Settings settings)
        {
            _settings = settings;
        }

        public void Display()
        {
            if (!_settings.IsActive)
            {
                return;
            }

            _settings.WindowSize = GUI.Window(_settings.Id,
                _settings.WindowSize,
                WindowCallBackFunction,
                new GUIContent("", _settings.TransformName));
        }

        public void Close()
        {
            _settings.IsActive = false;
        }

        private void WindowCallBackFunction(int transformId)
        {
            // Handle Window Event
            HandleWindowEvents();
            HandleFocusedWindowEvents();

            // Layout
            WindowGUILayout();

            // Key pressed Update
            InputEventUpdate();
        }

        private void HandleWindowEvents()
        {
            if (Event.current.type == EventType.MouseDown)
            {
                Selection.SetActiveObjectWithContext(_settings.Transform, null);
            }
        }

        private void HandleFocusedWindowEvents()
        {
            if (Event.current.type != EventType.Layout)
            {
                return;
            }

            _isFocused = IsFocusedFlagUpdate(_isFocused);
            if (!_isFocused)
            {
                return;
            }

            GUI.FocusWindow(_settings.Id);
            GUI.BringWindowToFront(_settings.Id);

            UseEscapeKeyCloseWindow();
        }

        private bool IsFocusedFlagUpdate(bool isFocused)
        {
            // TODO: The rules are too complex
            if (!(isFocused && Selection.activeTransform == null))
            {
                isFocused = Selection.activeTransform != null &&
                            Selection.activeTransform.GetInstanceID() == _settings.Id;
            }

            return isFocused;
        }

        private void UseEscapeKeyCloseWindow()
        {
            if (!_isEscapeKeyPressed || _isCloseWindowExecuted)
            {
                if (!_isEscapeKeyPressed)
                {
                    _isCloseWindowExecuted = false;
                }

                return;
            }

            _isCloseWindowExecuted = true;

            Close();
        }

        private void WindowGUILayout()
        {
            if (GUI.Button(CloseButtonPosition, "", GUI.skin.customStyles[0]))
            {
                Close();
            }

            if (_isFocused)
            {
                using (var scrollViewScope = new GUILayout.ScrollViewScope(_settings.ScrollPosition))
                {
                    _settings.ScrollPosition = scrollViewScope.scrollPosition;
                    DisplayScrollViewContent();
                }
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (_settings.TransformName.Length > 15)
                {
                    var name = string.Format(
                        $"{_settings.TransformName.Substring(0, 15)}...");
                    GUILayout.Label(name);
                }
                else
                {
                    GUILayout.Label(_settings.TransformName);
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            GUI.DragWindow();
        }

        private void DisplayScrollViewContent()
        {
            var transforms = _settings.Transform.GetAllParentsAndSelf();
            var transformNames = _settings.Transform.GetAllParentNamesAndSelf();

            for (var i = transforms.Count - 1; i >= 0; i--)
            {
                using (new GUILayout.HorizontalScope())
                {
                    ScrollViewElement(transforms[i], transformNames[i], i == 0);
                }
            }
        }

        private static void ScrollViewElement(Transform transform, string transformName, bool isEditable)
        {
            if (GUILayout.Button("✄", GUILayout.Width(20)))
            {
                transformName.CopyToClipboard();
                Selection.SetActiveObjectWithContext(transform, null);
                GUI.FocusWindow(transform.GetInstanceID());

                Debug.LogFormat($"[#{transformName}] copy success!");
            }

            if (isEditable)
            {
                var text = GUILayout.TextField(transformName);
                if (transformName != text)
                {
                    transform.name = text;
                }
            }
            else
            {
                GUILayout.Label(transformName);
            }
        }

        private void InputEventUpdate()
        {
            var e = Event.current;
            _isEscapeKeyPressed = e.type == EventType.KeyDown &&
                                  e.keyCode == KeyCode.Escape;
        }


        public class Settings
        {
            public int Id { get; }
            public bool IsActive { get; set; }
            public Rect WindowSize { get; set; }
            public Vector2 ScrollPosition { get; set; }
            public Transform Transform { get; }
            public string TransformName => _transformNameCache ?? (_transformNameCache = Transform.name);
            private string _transformNameCache;

            public Settings(int id, bool isActive, Rect windowSize, Vector2 scrollPosition, Transform transform)
            {
                Id = id;
                IsActive = isActive;
                WindowSize = windowSize;
                ScrollPosition = scrollPosition;
                Transform = transform;
            }
        }


        public class Factory : IFactory<Settings, IWindow>
        {
            public IWindow Create(Settings settings)
            {
                return new Window(settings);
            }
        }
    }
}