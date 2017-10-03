using UnityEditor;
using UnityEngine;
public abstract class ExtendEditor<T>:Editor where T:MonoBehaviour
{
    protected new T target;
    Font font;
    GUISkin Skin;
    GUIStyle textField;
    private void OnEnable()
    {
        target = (T)base.target;
        IOnLoadEdit load = this as IOnLoadEdit;
        if (load != null) load.OnLoad();
        font = AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/GODOM.TTF");
        Skin= AssetDatabase.LoadAssetAtPath<GUISkin>("Assets/Editors/EditorSkin.guiskin");
    }
    //GUI Skin 폰트 변경을 위해 하위클래스에서는 더 이상 OnInspectorGUI를 재정의하지못하고 대신 OnCustomGUI를 사용한다.
    public sealed override void OnInspectorGUI()
    {
        GUI.skin = Skin;
        GUI.skin.font = font;
        GUIStyle label = new GUIStyle(GUI.skin.GetStyle("Label"));
        OnCustomGUI();
    }
    protected void TextField(string name,ref string content)
    {
        if(textField==null)textField = new GUIStyle(GUI.skin.GetStyle("TextField"));
        textField.font = font;
        content = EditorGUILayout.TextField(name, content, textField);
    }
    protected void ObjectField<T1>(string name, T1 obj) where T1 : UnityEngine.Object
    {
        obj = (T1)EditorGUILayout.ObjectField(name, obj, typeof(T1), true);
    }
    protected abstract void OnCustomGUI();
}

internal interface IOnLoadEdit
{
    void OnLoad();
}
