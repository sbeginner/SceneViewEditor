using Editor.SceneViewEditor.Source.Extensions;
using Editor.SceneViewEditor.Source.Interfaces;
using UnityEditor;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Editor.SceneViewEditor.Source.Customs
{
    public class Window : IWindow
    {
        public int Id => _settings.Id;

        public bool IsActive
        {
            get => _settings.IsActive && Transform != null;
            set => _settings.IsActive = value;
        }

        public Transform Transform => _settings.Transform;

        private static bool _isCloseWindowExecuted;
        private static bool _isEscapeKeyPressed;
        private readonly Settings _settings;

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
                _settings.Id.ToString(),
                GUI.skin.window);
        }

        public void Close()
        {
            _settings.IsActive = false;
        }

        private void WindowCallBackFunction(int transformId)
        {
            // The execute order is important.
            // Handle Window Event
            HandleWindowEvents();
            HandleFocusedWindowEvents(transformId);

            // Layout
            WindowGUILayout();

            // Key pressed Update
#if ENABLE_INPUT_SYSTEM
            _isEscapeKeyPressed = Keyboard.current.escapeKey.isPressed;
#else
            _isEscapeKeyPressed = Input.GetKey(KeyCode.Escape);
#endif
        }

        private void HandleWindowEvents()
        {
            if (Event.current.type != EventType.MouseDown)
            {
                return;
            }

            Selection.SetActiveObjectWithContext(_settings.Transform, null);
        }

        private void HandleFocusedWindowEvents(int transformId)
        {
            var isFocused = Selection.activeTransform != null &&
                            Selection.activeTransform.GetInstanceID() == transformId;
            if (!isFocused)
            {
                return;
            }

            GUI.FocusWindow(transformId);
            GUI.BringWindowToFront(transformId);

            UseEscapeKeyCloseWindow();
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
            using (new GUILayout.AreaScope(new Rect(130, 0, 20, 20)))
            {
                DisplayCloseButton();
            }

            GUILayout.Space(5);

            using (var scrollViewScope = new GUILayout.ScrollViewScope(_settings.ScrollPosition))
            {
                _settings.ScrollPosition = scrollViewScope.scrollPosition;
                DisplayScrollViewContent();
            }

            GUI.DragWindow();
        }

        private void DisplayScrollViewContent()
        {
            var transforms = _settings.Transform.GetAllParentsAndSelf();
            for (var i = transforms.Count - 1; i >= 0; i--)
            {
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("✄", GUILayout.Width(20)))
                {
                    transforms[i].name.CopyToClipboard();
                    Selection.SetActiveObjectWithContext(transforms[i], null);
                    GUI.FocusWindow(transforms[i].GetInstanceID());

                    Debug.Log($"[#{transforms[i].name}] copy success!");
                }

                if (i == 0)
                {
                    transforms[i].name = GUILayout.TextField(transforms[i].name, GUI.skin.textField);
                }
                else
                {
                    GUILayout.Label(transforms[i].name, GUI.skin.label);
                }

                GUILayout.EndHorizontal();
            }
        }

        private void DisplayCloseButton()
        {
            if (GUILayout.Button("X", GUI.skin.button))
            {
                Close();
            }
        }


        public class Settings
        {
            public int Id => Transform.GetInstanceID();
            public bool IsActive { get; set; }
            public Rect WindowSize { get; set; }
            public Vector2 ScrollPosition { get; set; }
            public Transform Transform { get; }

            public Settings(bool isActive, Rect windowSize, Vector2 scrollPosition, Transform transform)
            {
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