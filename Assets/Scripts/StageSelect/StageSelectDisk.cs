#pragma warning disable 0649
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static CodePack.CorutineHelper;
using CustomEdit;
public class StageSelectDisk : MonoBehaviour,ICustomEditable
{
    public AudioClip TurnEffect; //회전시 출력할 효과음
    public AudioClip SelectEffect; //앱 선택시 출력할 효과음
    [SerializeField]
    Text AppTitle; //선택한 앱 이름 출력.
    AudioSource Au; //현재 클래스를 가진 객체가 가진 AudioSource
    Vector3 standard; //드래그시 방향을 잡을 기준점
    Vector3 AppPos => new Vector3(213, 1598);//앱 선택시 목적지점
    bool isPlayEffect = false; //한번의 드래그시 한번만 효과음을 출력하게끔 하기위한 bool 변수
    public bool isPlayCorutine = false; //앱 선택으로 회전하는동안 다른 앱이 선택되지 않게끔하는 bool 변수 

    private void Start()
    {
        Au = GetComponent<AudioSource>();
        AppPrepare();
    }
    //8개 앱을 기준으로, 각 앱들의 위치를 정렬한다.
    void AppPrepare()
    {
        foreach (Transform tr in this.transform)
        {
            Vector3 rot = this.transform.rotation.eulerAngles;
            rot.z += 35;
            this.transform.rotation = Quaternion.Euler(rot);
            tr.position = AppPos;
        }
        AppsIdentify();
    }
    public void DragStart(BaseEventData basedata)
    {
        PointerEventData data = basedata as PointerEventData;
        standard = data.position;
        isPlayEffect = false;
    }
    public void Drag(BaseEventData basedata)
    {
        if (!isPlayEffect)
        {
            isPlayEffect = true;
            Au.PlayOneShot(TurnEffect);
        }
        PointerEventData data = basedata as PointerEventData;
        Vector3 pos = data.position;
        float dis = pos.y - standard.y;
        Vector3 rot = this.transform.rotation.eulerAngles;
        rot.z += dis / 10;
        this.transform.rotation = Quaternion.Euler(rot);
        standard = pos;
        AppsIdentify();
    }
    //원반 회전에 따른 앱 아이콘의 각도 재수정.
    void AppsIdentify()
    {
        foreach (Transform tr in this.transform) tr.rotation = Quaternion.identity;
    }
    //앱 선택시 해당 앱이 특정위치에 존재하도록 원반을 회전시키는 코루틴.
    IEnumerator AppSelect(Transform app)
    {
        Au.PlayOneShot(SelectEffect);
        //현재 앱의 좌표와 기준 좌표사이의 각도가 1도 이하가 될때까지 회전시킨다.
        yield return new WaitUntil(() => Wait(condition: () => Vector3.Angle(app.position, AppPos) > 1, action: () =>
        {
            //방향을 구하고, 해당 방향으로 원반의 z축을 회전시킨다
            int dir = (app.position - AppPos).normalized.y > 0 ? -3 : 3;
            Vector3 rot = this.transform.rotation.eulerAngles;
            rot.z += dir;
            this.transform.rotation = Quaternion.Euler(rot);
            AppsIdentify();
            //같은 조건 연산자를 사용해 현재 앱이 목표지점을 지나쳤는지를 판정.
            int after = (app.position - AppPos).normalized.y > 0 ? -3 : 3;
            //지나친 경우, 지나친 각도만큼 뺌
            if (dir != after)
            {
                rot.z -= Vector3.Angle(app.position, AppPos);
                this.transform.rotation = Quaternion.Euler(rot);
                AppsIdentify();
            }
        }));
        AppTitle.text = app.name;
        isPlayCorutine = false;
    }
}
