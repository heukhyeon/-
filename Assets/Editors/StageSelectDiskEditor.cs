using System;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StageSelectDisk))]
public class StageSelectDiskEditor : ExtendEditor<StageSelectDisk>, IOnLoadEdit
{

    //로드시 현재 효과음이 없는경우 에셋폴더에서 효과음을 찾아 등록한다.
    public void OnLoad()
    {
        if (target.TurnEffect == null) target.TurnEffect = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/StageSelect/StageSelectDrag.wav");
        if (target.SelectEffect == null) target.SelectEffect = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/StageSelect/StageSelectAppSelect.wav");
    }
    protected override void OnCustomGUI()
    {
        ObjectField("Turn Effect", target.TurnEffect);
        ObjectField("Select Effect", target.SelectEffect);
    }
}