using UnityEditor;

namespace EcsR3.UnityEditor.Editor.EditorInputs.Basic
{
    public class IntEditorInput : SimpleEditorInput<int>
    {
        public static int TypeUI(string label, int value)
        { return EditorGUILayout.IntField(label, value); }
        
        protected override int CreateTypeUI(string label, int value)
        { return TypeUI(label, value); }
    }
}