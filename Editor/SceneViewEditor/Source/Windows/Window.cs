using System;
using Editor.SceneViewEditor.Source.Extensions;
using Editor.SceneViewEditor.Source.Interfaces;
using UnityEditor;
using UnityEngine;

namespace Editor.SceneViewEditor.Source.Windows
{
    public partial class Window : IWindow
    {
        public bool IsActive
        {
            get => _settings.IsActive;
            set => _settings.IsActive = value;
        }

        public bool IsDestroyable => _settings.Transform == null;
        public int Id => _settings.Id;
        public Transform Transform => _settings.Transform;

        private static readonly Rect CloseButtonPosition = new Rect(140, 5, 15, 15);
        private static bool _isCloseWindowExecuted;
        private static bool _isEscapeKeyPressed;
        private readonly Settings _settings;
        private readonly Action<IWindow> _closeCallBackFunction;

        private Window(Settings settings, Action<IWindow> closeCallBackFunction)
        {
            _settings = settings;
            _closeCallBackFunction = closeCallBackFunction;
        }

        public void Display()
        {
            if (!_settings.IsActive)
            {
                return;
            }

            _settings.WindowSize = GUI.Window(_settings.Id,
                _settings.WindowSize,
                WindowCallBackFunction, "");
        }

        public void SetWindowSize(Rect windowSize)
        {
            _settings.WindowSize = windowSize;
        }

        public void Close()
        {
            _settings.IsActive = false;
            _closeCallBackFunction?.Invoke(this);
        }

        private void WindowCallBackFunction(int transformId)
        {
            // Handle Window Event
            HandleWindowEvents();

            // Layout
            WindowGUILayout();
        }

        private void WindowGUILayout()
        {
            if (GUI.Button(CloseButtonPosition, "", GUI.skin.customStyles[0]))
            {
                Close();
            }

            if (_settings.IsFocused)
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

            if (_settings.IsFocused)
            {
                GUI.FocusWindow(_settings.Id);
                GUI.BringWindowToFront(_settings.Id);
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

        private void HandleWindowEvents()
        {
            IsFocusedFlagUpdate();
            UseEscapeKeyCloseWindow();

            SelectedWindow();
            InputEventUpdate();
        }

        private void IsFocusedFlagUpdate()
        {
            if (Event.current.type != EventType.Layout)
            {
                return;
            }

            if (_settings.IsFocused && Selection.activeTransform == null)
            {
                return;
            }

            _settings.IsFocused = Selection.activeTransform != null &&
                                  Selection.activeTransform.GetInstanceID() == _settings.Id;
        }

        private void UseEscapeKeyCloseWindow()
        {
            if (Event.current.type != EventType.Layout)
            {
                return;
            }

            if (!_isEscapeKeyPressed)
            {
                _isCloseWindowExecuted = false;
                return;
            }

            if (!_isCloseWindowExecuted)
            {
                _isCloseWindowExecuted = true;
                Close();
            }
        }

        private void SelectedWindow()
        {
            if (Event.current.type != EventType.MouseDown)
            {
                return;
            }

            Selection.SetActiveObjectWithContext(_settings.Transform, null);
        }

        private static void InputEventUpdate()
        {
            if (Event.current.type != EventType.KeyUp)
            {
                _isEscapeKeyPressed = false;
                return;
            }

            _isEscapeKeyPressed = Event.current.keyCode == KeyCode.Escape;
        }


        public class Factory : IFactory<Settings, IWindow>
        {
            private WindowHandler _handler;

            public IWindow Create(Settings settings, Action<IWindow> closeCallback)
            {
                return new Window(settings, closeCallback);
            }
        }
    }
}