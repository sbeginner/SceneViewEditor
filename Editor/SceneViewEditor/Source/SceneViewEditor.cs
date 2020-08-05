using System.Linq;
using Editor.SceneViewEditor.Source.Customs;
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
        private WindowHandler _windowHandler;
        private GUISkin _defaultGuiSkin;

        public void OnEnable()
        {
            CurrentState = State.OnEnable;
            
            var camera = SceneView.GetAllSceneCameras().FirstOrDefault();
            var factory = new Window.Factory();
            _windowHandler = new WindowHandler(factory, camera);

            SceneView.beforeSceneGui += OnGUIInitialize;
            SceneView.duringSceneGui += OnSceneGUIUpdate;
            SceneView.RepaintAll();

            Selection.selectionChanged = OnSceneSelectedObjects;
        }

        public void OnDisable()
        {
            CurrentState = State.OnDisable;

            SceneView.beforeSceneGui -= OnGUIInitialize;
            SceneView.duringSceneGui -= OnSceneGUIUpdate;
            SceneView.RepaintAll();

            Selection.selectionChanged = null;
        }

        private void OnGUIInitialize(SceneView _)
        {
            _defaultGuiSkin =
                AssetDatabase.LoadAssetAtPath<GUISkin>("Assets/Scripts/Editor/SceneViewEditor/CustomGUISkin.guiskin");
            
            SceneView.beforeSceneGui -= OnGUIInitialize;
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