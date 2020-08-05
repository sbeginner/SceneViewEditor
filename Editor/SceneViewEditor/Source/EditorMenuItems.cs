using UnityEditor;

namespace Editor.SceneViewEditor.Source
{
    public static class EditorMenuItems
    {
        private const string Url = "SceneViewEditor/Running";
        private static readonly SceneViewEditor SceneViewEditor = new SceneViewEditor();

        [MenuItem(Url)]
        private static void NamingWindowBox()
        {
            var isActive = Menu.GetChecked(Url);
            Menu.SetChecked(Url, !isActive);

            if (Menu.GetChecked(Url))
            {
                SceneViewEditor.OnEnable();
            }
            else
            {
                SceneViewEditor.OnDisable();
            }
        }

        [MenuItem(Url, true)]
        private static bool NamingWindowBoxChecker()
        {
            var isActive = SceneViewEditor.CurrentState == SceneViewEditor.State.OnEnable;
            Menu.SetChecked(Url, isActive);
            return true;
        }
    }
}