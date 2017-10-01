using UnityEditor;
using UnityEngine;
public class ExtendEditor<T>:Editor where T:MonoBehaviour
{
    new T target;
    private void OnEnable()
    {
        target = (T)base.target;
    }
}