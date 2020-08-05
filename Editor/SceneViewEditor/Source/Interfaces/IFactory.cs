namespace Editor.SceneViewEditor.Source.Interfaces
{
    public interface IFactory<in T2, out T1>
    {
        T1 Create(T2 data);
    }
}