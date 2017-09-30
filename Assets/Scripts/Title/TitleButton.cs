using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class TitleButton : CustomEditableClass{

    SpriteRenderer image;
    Vector3 origin;
    [SerializeField]
    AudioSource BackgroundAu;
    //세이브 파일이 없는등, '클릭 불가'상태의 색깔
    Color Disable => new Color(0.69f, 0.69f, 0.69f, 1f);
    //클릭은 가능하지만 포인터가 접근하지 않은 상태의 색깔
    Color NotPointerEnter = new Color(0.69f, 0.69f, 0.69f, 0.4f);
    //포인터가 접근, 클릭된 상태의 색깔
    Color PointerEnter = new Color(0, 1, 0, 1);
    private void Start()
    {
        image = GetComponent<SpriteRenderer>();
        image.color = Disable;
        origin = FindObjectOfType<Camera>().transform.position;
    }
    private void OnMouseOver()
    {
        image.color = PointerEnter;
    }
    private void OnMouseExit()
    {
        image.color = NotPointerEnter;
    }
}
