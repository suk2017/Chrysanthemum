using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Waiter : MonoBehaviour
{
    public string Name = "翠儿";
    public bool available = true;

    public delegate object fun2(object o);
    private string triggerTag;
    private NavMeshAgent m_agent;
    private Transform tr;

    private void Start()
    {
        tr = transform;
        m_agent = GetComponent<NavMeshAgent>();
    }



    /// <summary>
    /// 执行请求
    /// </summary>
    public IEnumerator _Order(Order o)
    {
        available = false;
        log(o.tag);
        while (triggerTag != o.tag)//检查是否已到达
        {
            m_agent.SetDestination(o.trf.position);//每个周期重新计算路径
            for (int i = 0; i < 12; ++i)//12帧 一周期
            {
                yield return new WaitForEndOfFrame();
                if (triggerTag == o.tag)//每帧重新判定是否已完成
                {
                    break;
                }
            }
        }
        o.fun(null);
        available = true;
    }

    public IEnumerator _Order(Transform des, fun2 fun)
    {
        available = false;
        while (true)
        {
            for (int i = 0; i < 4; ++i)//每0.05秒检查一次是否到达目的地
            {
                if (Vector3.SqrMagnitude(tr.position - des.position) < 0.5f)
                {
                    fun(null);
                    yield break;
                }
                yield return new WaitForSeconds(0.05f);
            }
        }
        available = true;
    }

    private void log(string tag)
    {
        switch (tag)
        {
            case "seat": print(Name + "前去点餐"); break;
            case "meal": print(Name + "前去取餐"); break;
            case "treat": print(Name + "前去送餐"); break;
            case "dishes": print(Name + "前去拾餐"); break;
            case "clear": print(Name + "前去收餐"); break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        triggerTag = other.tag;
    }

    /*******公有方法*******/

    /// <summary>
    /// 点餐
    /// </summary>
    public void _GetOrder(Order o)
    {
        StartCoroutine(GetOrder(o));
    }

    /// <summary>
    /// 取餐并送餐
    /// </summary>
    public void _TakeAndDeliver(Order o)
    {
        StartCoroutine(TakeAndDeliver(o));
    }

    /// <summary>
    /// 收拾桌子
    /// </summary>
    public void _ClearTableAndWashDishes(Order o)
    {
        StartCoroutine(ClearTableAndWashDishes(o));
    }

    /*******私有方法*******/

    /// <summary>
    /// 点餐
    /// </summary>
    private IEnumerator GetOrder(Order o)
    {
        available = false;
        print(Name + "前去点餐");
        while (true)
        {
            m_agent.SetDestination(o.trf.position);//设定目标
            if (Vector3.SqrMagnitude(o.trf.position - tr.position) < 1f)//如果到达目标
            {
                break;
            }
            yield return new WaitForSeconds(0.2f);
        }
        print(Name + "已点餐");
        //TODO B通知厨师做饭
        available = true;
    }

    /// <summary>
    /// 取餐并送餐
    /// </summary>
    private IEnumerator TakeAndDeliver(Order o)
    {
        available = false;
        print(Name + "前去取餐");
        while (true)
        {
            m_agent.SetDestination(o.MealPos.position);
            if (Vector3.SqrMagnitude(o.MealPos.position - tr.position) < 1f)
            {
                break;
            }
            yield return new WaitForSeconds(0.2f);
        }
        //TODO B获取餐盘
        print(Name + "已取餐");
        print(Name + "前往送餐");
        while (true)
        {
            m_agent.SetDestination(o.customer.transform.position);
            if (Vector3.SqrMagnitude(o.customer.transform.position - tr.position) < 1f)
            {
                break;
            }
            yield return new WaitForSeconds(0.2f);
        }
        //TODO B放下餐盘
        print(Name + "已送达");
        available = true;
    }

    /// <summary>
    /// 收拾桌子
    /// </summary>
    private IEnumerator ClearTableAndWashDishes(Order o)
    {
        available = false;
        print(Name + "前往拾餐");
        m_agent.SetDestination(o.MealPos.position);//餐盘固定 因此只需一次
        while (true)
        {
            if (Vector3.SqrMagnitude(o.MealPos.position - tr.position) < 1f)
            {
                break;
            }
        }
        //TODO 获取空盘（此时的MealPos是空盘的Pos）
        print(Name + "拾餐完毕");
        print(Name + "收回餐盘");
        Vector3 _pos = InnManager.current._GetRecyclePos(tr.position);
        m_agent.SetDestination(_pos);//收餐点固定
        while (true)
        {
            if (Vector3.SqrMagnitude(_pos - tr.position) < 1f)
            {
                break;
            }
        }
        //TODO B放下空盘
        print(Name + "收餐完毕");
        available = true;
    }


}




