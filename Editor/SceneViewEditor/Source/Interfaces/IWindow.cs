using UnityEngine;

namespace Editor.SceneViewEditor.Source.Interfaces
{
    public interface IWindow
    {
        int Id { get; }
        Transform Transform { get; }

        bool IsActive { get; set; }
        bool IsDestroyable { get; }

        void Display();
        void SetWindowSize(Rect windowSize);
        void Close();
    }
}