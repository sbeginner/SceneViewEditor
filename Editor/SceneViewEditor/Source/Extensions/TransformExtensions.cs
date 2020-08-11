using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor.SceneViewEditor.Source.Extensions
{
    public static class TransformExtensions
    {
        private static readonly Dictionary<int, List<Transform>> TransformsCache =
            new Dictionary<int, List<Transform>>();

        private static readonly Dictionary<int, List<string>> TransformNamesCache =
            new Dictionary<int, List<string>>();

        static TransformExtensions()
        {
            EditorApplication.hierarchyChanged -= DestroyTransformsCache;
            EditorApplication.hierarchyChanged += DestroyTransformsCache;
        }

        public static IList<Transform> GetAllParentsAndSelf(this Transform transformNode)
        {
            var id = transformNode.GetInstanceID();
            if (TransformsCache.TryGetValue(id, out var result))
            {
                return result;
            }

            Core(transformNode);

            return TransformsCache[id];
        }

        public static IList<string> GetAllParentNamesAndSelf(this Transform transformNode)
        {
            var id = transformNode.GetInstanceID();
            if (TransformNamesCache.TryGetValue(id, out var result))
            {
                return result;
            }

            Core(transformNode);

            return TransformNamesCache[id];
        }

        private static void Core(Transform transformNode)
        {
            var id = transformNode.GetInstanceID();
            var transforms = new List<Transform>();
            var transformNames = new List<string>();

            while (transformNode.parent != null)
            {
                transforms.Add(transformNode);
                transformNames.Add(transformNode.name);

                transformNode = transformNode.parent;
            }

            transforms.Add(transformNode);
            transformNames.Add(transformNode.name);

            TransformsCache[id] = transforms;
            TransformNamesCache[id] = transformNames;
        }

        private static void DestroyTransformsCache()
        {
            TransformsCache.Clear();
            TransformNamesCache.Clear();
        }
    }
}