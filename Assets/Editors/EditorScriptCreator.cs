using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.CodeDom;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Text;
using System.IO;
using System.Reflection;

public class EditorScriptCreator : EditorWindow
{
    [MenuItem("설정/에디터 코드 생성")]
    static void Open()
    {
        GetWindow<EditorScriptCreator>().Show();
    }
    [MenuItem("Assets/CreateEditorScript", true)]
    static bool CustomEditorableCheck()
    {
        UnityEngine.Object obj = Selection.activeObject;
        if (obj == null) return false; //선택된 객체가 없는경우 false.
        if (Path.GetExtension(AssetDatabase.GetAssetPath(obj)) != ".cs") return false;//객체의 확장명이 .cs가 아닌경우(=스크립트가 아닌경우)에도 false 반환.
        Assembly asm = typeof(CustomEditableClass).Assembly; //CustomEditableClass에 대한 어셈블리 인스턴스 생성.
        System.Type type = asm.GetType(obj.name);//어셈블리로부터, 유니티의 Monobehaviour는 파일이름과 클래스이름이 같은걸 이용해 파일이름을 매개변수로해 현재 객체의 타입을 가져온다.
        return type.IsSubclassOf(typeof(CustomEditableClass)); //현재 객체가 CustomEditableClass를 상속하는지를 판단한다.
    }
    [MenuItem("Assets/CreateEditorScript", false)]
    static void CreateEditorScript()
    {
        Object target = Selection.activeObject;
        string classname = target.name;
        string path = Application.dataPath + "/Editors/" + GetEdtiorClassName(classname) + ".cs";
        CSharpCodeProvider provider = new CSharpCodeProvider();
        // Build the parameters for source compilation.
        CompilerParameters cp = new CompilerParameters();
        // Add an assembly reference.
        cp.ReferencedAssemblies.Add("System.dll");
        cp.ReferencedAssemblies.Add(typeof(Transform).Assembly.Location);//UnityEngine.dll 참조
        cp.ReferencedAssemblies.Add(typeof(Editor).Assembly.Location);//UnityEditor.dll 참조
        // Generate an executable instead of
        // a class library.
        cp.GenerateExecutable = false;
        // Set the assembly file name to generate.
        cp.OutputAssembly = path;
        // Save the assembly as a physical file.
        cp.GenerateInMemory = false;
        CompilerResults cr = provider.CompileAssemblyFromSource(cp, CreateClass(classname));
        if (cr.Errors.Count > 0)
        {
            foreach (CompilerError err in cr.Errors)
            {
                Debug.Log("Line:" + err.Line + "\n Message:" + err.ErrorText);
            }
        }
    }
    static string CreateClass(string target)
    {
        StringBuilder sb = new StringBuilder("using System;\n");
        sb.Append("using UnityEngine;\n");
        sb.Append("using UnityEditor;\n\n\n");
        //sb.AppendFormat("[CustomEditor(typeof({0}))]\n", target);
        sb.AppendFormat("public class {0} : Editor", GetEdtiorClassName(target));
        sb.Append("\n{\n\n}");
        return sb.ToString();
    }
    static string GetEdtiorClassName(string target)
    {
        return target + "Editor";
    }
}
