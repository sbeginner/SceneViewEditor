using UnityEngine;

namespace Editor.SceneViewEditor.Source.Interfaces
{
    public interface IWindow
    {
        int Id { get; }
        bool IsActive { get; }
        Transform Transform { get; }
        void Display();
        void Close();
    }
}