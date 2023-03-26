using System;
using MarkovEditor;
using MarkovTest;

namespace Editor
{
    public interface IEditorElementDrawer<TElementDraw, in TSimulationElement> where TSimulationElement : IMarkovSimulationDrawer
    {
        public TElementDraw Draw(TElementDraw elem, TSimulationElement sim);
    }
}