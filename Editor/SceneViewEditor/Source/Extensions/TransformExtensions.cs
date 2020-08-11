using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor.SceneViewEditor.Source.Extensions
{
    public static class TransformExtensions
    {
        private static readonly Dictionary<int, List<Transform>> TransformsCache =
            new Dictionary<int, List<Transform>>();

        private static Dictionary<int, List<string>> TranformNamesCache = new Dictionary<int, List<string>>();

        static TransformExtensions()
        {
            EditorApplication.hierarchyChanged -= DestroyTransformsCache;
            EditorApplication.hierarchyChanged += DestroyTransformsCache;
        }

        private static void DestroyTransformsCache()
        {
            TransformsCache.Clear();
            TranformNamesCache.Clear();
        }

        public static IList<Transform> GetAllParentsAndSelf(this Transform transformNode)
        {
            var id = transformNode.GetInstanceID();
            if (TransformsCache.ContainsKey(id))
            {
                return TransformsCache[id];
            }

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
            TranformNamesCache[id] = transformNames;

            return transforms;
        }

        public static IList<string> GetAllParentNamesAndSelf(this Transform transformNode)
        {
            return TranformNamesCache[transformNode.GetInstanceID()];
        }

    }
}