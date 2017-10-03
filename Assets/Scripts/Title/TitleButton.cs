#pragma warning disable 0649 //SerializeField 할당 경고 무시
using System.Collections;
using UnityEngine;
using CustomEdit;
using static CodePack.CorutineHelper;
public class TitleButton : MonoBehaviour,ICustomEditable{

    SpriteRenderer image;
    [SerializeField]
    AudioClip EnterEffect; //마우스 오버 효과음
    [SerializeField]
    AudioClip ClickEffect; //클릭 효과음
    [SerializeField]
    AudioSource BackgroundAu; //배경음악 출력. Main Camera에 담겨있음.
    [SerializeField]
    Transform EyeSight; //배경 어둡게할 효과. Scale을 줄여서 화면을 어둡게 한다.
    AudioSource Au; //버튼 효과음 출력, TitleButton 클래스를 가진 객체에 함께 있음.
    //세이브 파일이 없는등, '클릭 불가'상태의 색깔
    Color Disable => new Color(0.69f, 0.69f, 0.69f, 1f);
    //클릭은 가능하지만 포인터가 접근하지 않은 상태의 색깔
    Color NotPointerEnter = new Color(0.69f, 0.69f, 0.69f, 0.4f);
    //포인터가 접근, 클릭된 상태의 색깔
    Color PointerEnter = new Color(0, 1, 0, 1);
    bool isClick = false; //버튼 중복클릭 방지
    private void Start()
    {
        Au = GetComponent<AudioSource>();
        image = GetComponent<SpriteRenderer>();
        image.color = Disable;
    }
    private void OnMouseEnter()
    {
        if (isClick) return; //클릭이후 다시 발생하지 않게함.
        Au.PlayOneShot(EnterEffect);
        image.color = PointerEnter;
    }
    private void OnMouseExit()
    {
        if (isClick) return; //클릭 이후 다시 발생하지 않게함.
        image.color = NotPointerEnter;
    }
    private void OnMouseDown()
    {
        if (isClick) return;
        isClick = true;
        Au.PlayOneShot(ClickEffect);
        StartCoroutine(MoveEvent());
    }
    IEnumerator MoveEvent()
    {
        Camera camera = BackgroundAu.GetComponent<Camera>();
        float size = camera.orthographicSize;
        float speed = Time.deltaTime;
        float time = 0f;
        yield return new WaitUntil(() => Wait(condition: () => time < 1f, action: () =>
        {
            Color color = image.color;
            color.a = color.a == 1 ? 0 : 1;
            image.color = color;
            time += Time.deltaTime;
        }));
        image.color = PointerEnter;
        yield return new WaitUntil(() => Wait(condition: () => EyeSight.localScale.x > 0f, action: () =>
                 {
                     XYScaleSet(EyeSight, -speed, 0f);
                     speed += speed / 10f;
                 }));

    }
}
