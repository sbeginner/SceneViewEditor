using System.Collections.Generic;
using UnityEngine;

namespace Editor.SceneViewEditor.Source.Extensions
{
    public static class TransformExtensions
    {
        private static readonly List<Transform> Transforms = new List<Transform>();
        public static List<Transform> GetAllParentsAndSelf(this Transform transformNode)
        {
            Transforms.Clear();
            
            while (transformNode.parent != null)
            {
                Transforms.Add(transformNode);
                transformNode = transformNode.parent;
            }
            
            Transforms.Add(transformNode);
            return Transforms;
        }
    }
}