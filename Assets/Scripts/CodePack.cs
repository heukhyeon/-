//MonoBehaviour가 필요없는 클래스, 인터페이스들의 집합

//커스텀에디터를 사용하는 경우 참조하는 네임스페이스
using System;
using System.Collections;
using System.Threading;
using UnityEngine;

namespace CustomEdit
{
    //커스텀 에디터를 사용하는경우 상속하는 인터페이스
    public interface ICustomEditable
    {

    }
}

namespace CodePack
{
    public static class SmartPhone
    {
        
    }
    public static class CorutineHelper
    {
        public static bool Wait(Func<bool> condition, Action action)
        {
            if (condition())action();
            return !condition();
        }
        public static void XYScaleSet(Transform target,float speed,float goal)
        {
            bool dir = target.localScale.x < goal;//타겟의 로컬스케일을 키우는경우 true
            target.localScale = new Vector3(target.localScale.x + speed, target.localScale.y + speed);
            if (dir && target.localScale.x > goal) target.localScale = new Vector3(goal, goal);
            else if (!dir && target.localScale.x < goal) target.localScale = new Vector3(goal, goal);
        }
    }
}

