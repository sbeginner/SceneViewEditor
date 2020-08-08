using System.IO;
using System.Linq;
using Editor.SceneViewEditor.Source.Windows;
using UnityEditor;
using UnityEngine;

namespace Editor.SceneViewEditor.Source
{
    public class SceneViewEditor
    {
        public enum State
        {
            OnEnable,
            OnDisable,
            None
        }

        public State CurrentState { get; private set; } = State.None;

        private readonly string _customGUISkinUri = Path.Combine("Assets",
            "Scripts/Editor/SceneViewEditor/Skins",
            "CustomGUISkin.guiskin");

        private WindowHandler _windowHandler;
        private GUISkin _defaultGuiSkin;

        public void OnEnable()
        {
            CurrentState = State.OnEnable;

            var camera = SceneView.GetAllSceneCameras().FirstOrDefault();
            var factory = new Window.Factory();
            _windowHandler = new WindowHandler(factory, camera);

            _defaultGuiSkin = GetSkin();

            SceneView.duringSceneGui += OnSceneGUIUpdate;
            SceneView.RepaintAll();

            Selection.selectionChanged = OnSceneSelectedObjects;
        }

        public void OnDisable()
        {
            CurrentState = State.OnDisable;

            SceneView.duringSceneGui -= OnSceneGUIUpdate;
            SceneView.RepaintAll();

            Selection.selectionChanged = null;
        }

        private GUISkin GetSkin()
        {
            return AssetDatabase.LoadAssetAtPath<GUISkin>(_customGUISkinUri);
        }

        private void OnSceneSelectedObjects()
        {
            _windowHandler.OnSceneSelectedObjects();
        }

        private void OnSceneGUIUpdate(SceneView _)
        {
            GUI.skin = _defaultGuiSkin;

            _windowHandler.OnSceneGUIUpdate();
        }
    }
}