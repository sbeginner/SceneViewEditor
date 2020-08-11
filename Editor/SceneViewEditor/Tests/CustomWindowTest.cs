using System.Linq;
using Editor.SceneViewEditor.Source.Windows;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Editor.SceneViewEditor.Tests
{
    public class CustomWindowTest
    {
        private Window.Factory _windowFactory;
        private WindowHandler _mockHandler;

        [SetUp]
        public void Common_Setup()
        {
            _windowFactory = new Window.Factory();

            var camera = SceneView.GetAllSceneCameras().FirstOrDefault();
            _mockHandler = new WindowHandler(_windowFactory, camera);

            Assert.IsNotNull(_windowFactory);
            Assert.IsNotNull(_mockHandler);
        }

        [Test]
        public void Window_Setup_With_All_Default_Settings()
        {
            var mockTransform = new GameObject().transform;
            var settings = new Window.Settings(
                Rect.zero,
                Vector2.zero,
                mockTransform);

            var window = _windowFactory.Create(settings, null);

            Assert.IsTrue(window.IsActive);
            Assert.IsTrue(window.Id == mockTransform.GetInstanceID());
            Assert.AreSame(window.Transform, mockTransform);
        }

        [Test]
        public void Window_Setup_With_Missing_Transform()
        {
            var settings = new Window.Settings(
                Rect.zero,
                Vector2.zero,
                null);

            var window = _windowFactory.Create(settings, null);

            Assert.IsTrue(window.IsDestroyable);
            UnityEngine.Assertions.Assert.IsNull(window.Transform);
        }

        [Test]
        public void Window_Close()
        {
            var mockTransform = new GameObject().transform;
            var settings = new Window.Settings(
                Rect.zero,
                Vector2.zero,
                mockTransform);

            var window = _windowFactory.Create(settings, null);

            window.Close();

            Assert.IsFalse(window.IsActive);
        }
    }
}