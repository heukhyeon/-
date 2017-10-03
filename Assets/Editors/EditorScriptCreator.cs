using UnityEngine;
using UnityEditor;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using CustomEdit;

public class EditorScriptCreator
{
    // 현재 선택된 파일이 커스텀 에디터를 작성할 수 있는 파일인지 확인
    [MenuItem("Assets/CreateEditorScript", true)]
    static bool CustomEditorableCheck()
    {
        UnityEngine.Object obj = Selection.activeObject;
        if (obj == null) return false; //선택된 객체가 없는경우 false.
        if (Path.GetExtension(AssetDatabase.GetAssetPath(obj)) != ".cs") return false;//객체의 확장명이 .cs가 아닌경우(=스크립트가 아닌경우)에도 false 반환.
        Assembly asm = typeof(ICustomEditable).Assembly; //CustomEditableClass에 대한 어셈블리 인스턴스 생성.
        System.Type type = asm.GetType(obj.name);//어셈블리로부터, 유니티의 Monobehaviour는 파일이름과 클래스이름이 같은걸 이용해 파일이름을 매개변수로해 현재 객체의 타입을 가져온다.
        foreach (System.Type ifc in type.GetInterfaces()) if (ifc.Equals(typeof(ICustomEditable))) return true; //ICustomEditabble을 상속하는경우 true 반환
        return false; //ICustomEditable을 상속하지 않는경우 false 반환.
    }
    //커스텀 에디터 파일 작성 클릭
    [MenuItem("Assets/CreateEditorScript", false)]
    static void CreateEditorScript()
    {
        Object target = Selection.activeObject;
        string name = target.name; // 유니티의 Script Component로서 삽입될수있는 .cs파일은 모두 파일명과 클래스명이 같다. 
        string path = Application.dataPath+"/Editors/"+ GetEdtiorClassName(name) + ".cs";
        MonoScript exist = AssetDatabase.LoadAssetAtPath<MonoScript>("Assets/Editors/"+ GetEdtiorClassName(name) + ".cs");
        //이미 파일이 있는경우. 기존의 파일을 보여주고 나머지 내용을 생략.
        if (exist!=null)
        {
            EditorGUIUtility.PingObject(exist);
            return;
        }
        string source = CreateEditorSource(name); //컴파일할 소스
        //컴파일 에러가 없는경우 파일을 작성한다.
        if(CompileErrorCheck(source))
        {
            File.WriteAllText(path, source,System.Text.Encoding.UTF8);
            //AssetDatabase를 갱신해 새로운 파일을 확인한다.
            AssetDatabase.Refresh();
            //Asset앞의 경로를 지운다.
            path = "Assets/Editors/" + GetEdtiorClassName(name) + ".cs";
            //만들어진 파일을 강조한다.
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<MonoScript>(path));
        }
    }
    //만들려는 소스가 컴파일 에러가 있는지 체크
    static bool CompileErrorCheck(string source)
    {
        CodeDomProvider provider = new CSharpCodeProvider(); //컴파일할 provider 생성
        CompilerParameters parameter = new CompilerParameters(); //컴파일 파라미터 생성
        parameter.GenerateExecutable = false; //결과 파일을 생성하지 않음.
        parameter.GenerateInMemory = false;//메모리에 컴파일결과를 올리지 않음.
        //어셈블리 추가
        parameter.ReferencedAssemblies.AddRange(new string[]
        {
            typeof(Transform).Assembly.Location, //UnityEngine 어셈블리
            typeof(Editor).Assembly.Location, //UnityEditor 어셈블리
            typeof(ICustomEditable).Assembly.Location, //Assets/Scripts의 어셈블리
            typeof(ExtendEditor<>).Assembly.Location, //Assets/Editors의 어셈블리
        });
        CompilerResults result = provider.CompileAssemblyFromSource(parameter, source);
        if (result.Errors.Count > 0)
        {
            foreach (CompilerError err in result.Errors)
                Debug.LogError("Line:" + err.Line + "\n Message:" + err.ErrorText);
            return false;
        }
        else return true;
    }
    //만들어진 파일의 코드 내용.
    static string CreateEditorSource(string target)
    {
        string code = @"
using System;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof("+target+@"))]
public class "+GetEdtiorClassName(target)+@" : ExtendEditor<"+target+@">
{
    protected override void OnCustomGUI()
    {

    }
}";
        return code.Remove(0, 2);//가독성을 위해 코딩부분에서 처음 엔터친 부분을 제거한다.
    }
    //클래스 이름+Editor 를 반환
    static string GetEdtiorClassName(string target)
    {
        return target + "Editor";
    }
}
