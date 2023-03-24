namespace Editor
{
    public interface IEditorElementDrawer<T>
    {
        public T Draw(T elem, MarkovSimulation2D sim);
    }
}