using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StageAppIcon : MonoBehaviour, IPointerDownHandler
{
    StageSelectDisk disk; //부모의 StageSelectDisk

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!disk.isPlayCorutine)
        {
            disk.isPlayCorutine = true;
            disk.StartCoroutine("AppSelect", this.transform);
        }
    }

    // Use this for initialization
    void Start()
    {
        disk = GetComponentInParent<StageSelectDisk>();
    }
}

