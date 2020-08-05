using System;
using System.Collections;
using System.Linq;
using Editor.SceneViewEditor.Source.Customs;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Editor.SceneViewEditor.Tests
{
    public class CustomWindowHandlerTest
    {
        [Test]
        public void WindowHandler_Setup()
        {
            var camera = SceneView.GetAllSceneCameras().FirstOrDefault();
            var windowFactory = new Window.Factory();
            var mockHandler = new WindowHandler(windowFactory, camera);

            Assert.IsNotNull(mockHandler);
        }

        [Test]
        public void WindowHandler_OnSceneSelectedObjects_Method()
        {
            var camera = SceneView.GetAllSceneCameras().FirstOrDefault();
            var windowFactory = new Window.Factory();
            var mockHandler = new WindowHandler(windowFactory, camera);

            var mockTransform = new GameObject().transform;
            Selection.SetActiveObjectWithContext(mockTransform, null);

            mockHandler.OnSceneSelectedObjects();

            Assert.AreSame(mockTransform, Selection.activeTransform);
        }

        [UnityTest]
        public IEnumerator WindowHandler_OnSceneGUIUpdate_Method()
        {
            var camera = SceneView.GetAllSceneCameras().FirstOrDefault();
            var windowFactory = new Window.Factory();
            var mockHandler = new WindowHandler(windowFactory, camera);

            var mockTransform = new GameObject().transform;
            Selection.SetActiveObjectWithContext(mockTransform, null);
            mockHandler.OnSceneSelectedObjects();

            var onGUIUpdate = new Action<SceneView>(delegate { mockHandler.OnSceneGUIUpdate(); });
            SceneView.duringSceneGui += onGUIUpdate;
            yield return null;
            SceneView.duringSceneGui -= onGUIUpdate;

            Assert.AreSame(mockTransform, Selection.activeTransform);
        }

        [UnityTest]
        public IEnumerator WindowHandler_OnSceneGUIUpdate_Method_With_Missing_Transform()
        {
            var camera = SceneView.GetAllSceneCameras().FirstOrDefault();
            var windowFactory = new Window.Factory();
            var mockHandler = new WindowHandler(windowFactory, camera);

            var mockTransform = new GameObject().transform;
            Selection.SetActiveObjectWithContext(mockTransform, null);
            mockHandler.OnSceneSelectedObjects();
            
            Object.DestroyImmediate(mockTransform.gameObject);

            var onGUIUpdate = new Action<SceneView>(delegate { mockHandler.OnSceneGUIUpdate(); });
            SceneView.duringSceneGui += onGUIUpdate;
            yield return null;
            SceneView.duringSceneGui -= onGUIUpdate;

            Assert.IsNull(Selection.activeTransform);
        }
    }
}