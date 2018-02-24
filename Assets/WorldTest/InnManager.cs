/**********************************************************************
 * 本类用于客栈中各种机构的组织管理
 * 顾客类（Customer） 可以从此处获取座位点和结账点等（易扩充）
 * 跑堂类（Waiter） 可以从此处获取点餐点、取菜点、上菜点、收餐点、碗筷点等（易扩充 门内所有事项）
 * 迎宾类（Usher） 可以从此处获取招待点等（易扩充 门外所有事项）
 * 厨师类（Chef） 可以从此处获取厨师点等（易扩充 后厨所有事项）
 * 账房类（Accountant）可以从此处获取账房点（易扩充 账房所有事项）
 * 掌柜类（Boss）可以从此处获取冲突点（易扩充 所有事项
 **********************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnManager : MonoBehaviour
{

    [Tooltip("所有功能性家具的父物体")] public Transform furnitureRoot;



    /**********顾客**********/
    /// <summary>席位点</summary>
    private Transform[] seats;
    /// <summary>结账点</summary>
    private Transform[] check;

    private bool[] b_seats;

    /**********跑堂**********/
    /// <summary>点餐点</summary>
    private List<Transform> order;
    /// <summary>取餐点</summary>
    private Transform[] meal;
    /// <summary>送餐点</summary>
    private List<Transform> treat;
    /// <summary>碗筷点</summary>
    private List<Transform> dishes;
    /// <summary>收餐点</summary>
    private Transform[] clear;


    /**********迎宾**********/
    /// <summary>迎宾点</summary>
    private Transform[] usher;
    /// <summary>驻马点</summary>
    private Transform[] stable;

    /**********厨师**********/
    /// <summary>烹调点</summary>
    private Transform[] cook;

    /**********账房**********/
    /// <summary>账房点</summary>
    private Transform[] book;

    /*********其它**********/
    /// <summary>尚未点餐</summary>
    private List<Order> m_FreshOrders;
    /// <summary>已创建订单</summary>
    private List<Order> m_Orders;
    /// <summary>已完成订单</summary>
    private List<Order> m_FinishedOrders;

    /// <summary>所有可用的侍者</summary>
    private Waiter[] m_WaiterList
        ;
    //private Chief[] m_ChiefList;
    //private Usher[] m_UsherList;
    //private Accountant[] m_AccountantList;

    public static InnManager current
    {
        get
        {
            if (m_current == null)
            {
                m_current = GameObject.FindObjectOfType<InnManager>();
            }
            return m_current;
        }
    }
    private static InnManager m_current;


    /***********内部方法***********/



    private void Start()
    {
        StartCoroutine(WaiterService());
        _ResetValue();
    }

    /// <summary>
    /// waiter提供五种服务 点餐 取餐 上餐 拾餐 收餐
    /// </summary>
    private IEnumerator WaiterService()
    {
        if (m_Orders == null)
        {
            m_Orders = new List<Order>();
        }
        while (true)
        {
            if (m_Orders.Count > 0)
            {
                Waiter w = _GetFreeWaiter();
                if (w != null)
                {
                    switch (m_Orders[0].status)
                    {
                        case 0: StartCoroutine(w._Order(m_Orders[0], "seat", _Order)); break;
                        case 1: StartCoroutine(w._Order(m_Orders[0], "meal", _TakeMeal)); break;
                        case 2: StartCoroutine(w._Order(m_Orders[0], "treat", _Deliver)); break;
                        case 3: StartCoroutine(w._Order(m_Orders[0], "dishes", _ClearTable)); break;
                        case 4: StartCoroutine(w._Order(m_Orders[0], "clear", _PutAway)); break;
                    }
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// 等待点餐 → 等待上餐
    /// </summary>
    public void _Order(Order o)
    {
        if (m_Orders.Remove(o))
        {
            print("删除成功");
        }
        o.status += 1;
        m_Orders.Add(o);
    }

    /// <summary>
    /// 取餐
    /// </summary>
    public void _TakeMeal(Order o)
    {
        m_Orders.Remove(o);
        o.status += 1;
        m_Orders.Add(o);
    }

    /// <summary>
    /// 送餐
    /// </summary>
    public void _Deliver(Order o)
    {
        m_Orders.Remove(o);
        o.status += 1;
        m_Orders.Add(o);
    }

    public void _Clear(Order o)
    {
        m_Orders.Remove(o);
        o.status += 1;
        m_Orders.Add(o);
    }

    public void _ClearTable(Order o)
    {
        m_Orders.Remove(o);
        o.status += 1;
        m_Orders.Add(o);
    }

    public void _PutAway(Order o)
    {
        m_Orders.Remove(o);
    }

    /// <summary>
    /// 每次开始正式营业前调用一次
    /// </summary>
    public void _ResetValue()
    {


        /**********顾客**********/
        GameObject[] temp = GameObject.FindGameObjectsWithTag("seat");
        if (seats == null || seats.Length != temp.Length)
        {
            seats = new Transform[temp.Length];
            for (int i = 0; i < seats.Length; ++i)
            {
                seats[i] = temp[i].transform;
            }
        }

        temp = GameObject.FindGameObjectsWithTag("check");
        if (check == null || check.Length != temp.Length)
        {
            check = new Transform[temp.Length];
            for (int i = 0; i < check.Length; ++i)
            {
                check[i] = temp[i].transform;
            }
        }

        b_seats = new bool[seats.Length];

        /**********跑堂**********/
        if (order == null)
        {
            order = new List<Transform>();
        }

        temp = GameObject.FindGameObjectsWithTag("meal");
        if (meal == null || meal.Length != temp.Length)
        {
            meal = new Transform[temp.Length];
            for (int i = 0; i < meal.Length; ++i)
            {
                meal[i] = temp[i].transform;
            }
        }

        if (treat == null)
        {
            treat = new List<Transform>();
        }

        if (dishes == null)
        {
            dishes = new List<Transform>();
        }

        temp = GameObject.FindGameObjectsWithTag("clear");
        if (clear == null || clear.Length != temp.Length)
        {
            clear = new Transform[temp.Length];
            for (int i = 0; i < clear.Length; ++i)
            {
                clear[i] = temp[i].transform;
            }
        }



        /**********迎宾**********/
        temp = GameObject.FindGameObjectsWithTag("usher");
        if (usher == null || usher.Length != temp.Length)
        {
            usher = new Transform[temp.Length];
            for (int i = 0; i < usher.Length; ++i)
            {
                usher[i] = temp[i].transform;
            }
        }

        temp = GameObject.FindGameObjectsWithTag("stable");
        if (stable == null || stable.Length != temp.Length)
        {
            stable = new Transform[temp.Length];
            for (int i = 0; i < stable.Length; ++i)
            {
                stable[i] = temp[i].transform;
            }
        }


        /**********厨师**********/
        temp = GameObject.FindGameObjectsWithTag("cook");
        if (cook == null || cook.Length != temp.Length)
        {
            cook = new Transform[temp.Length];
            for (int i = 0; i < cook.Length; ++i)
            {
                cook[i] = temp[i].transform;
            }
        }


        /**********账房**********/
        temp = GameObject.FindGameObjectsWithTag("book");
        if (book == null || book.Length != temp.Length)
        {
            book = new Transform[temp.Length];
            for (int i = 0; i < book.Length; ++i)
            {
                book[i] = temp[i].transform;
            }
        }


        /*********其它**********/

        m_WaiterList = new Waiter[1];
    }

    /// <summary>
    /// 获取空闲的侍者
    /// </summary>
    public Waiter _GetFreeWaiter()
    {
        for (int i = 0; i < m_WaiterList.Length; ++i)
        {
            if (m_WaiterList[i].available)
            {
                return m_WaiterList[i];
            }
        }
        return null;
    }


    /**********顾客**********/
    /// <summary>
    /// 获取一个空座位 如果没有 则不进入/排队等待
    /// </summary>
    public bool _GetPos(out Vector3 pos)
    {
        for (int i = 0; i < 10; ++i)//连续十次判断是否有位置 都没有就离开
        {
            if (b_seats == null)
            {
                b_seats = new bool[seats.Length];
            }
            int value = (int)(Random.value * b_seats.Length);
            if (b_seats[value])
            {
                pos = seats[i].position;
                return true;
            }
        }
        pos = Vector3.zero;
        return false;
    }

    /// <summary>
    /// 获取服务
    /// </summary>
    public void _GetService(Order o)
    {
        m_FreshOrders.Add(o);

    }


}


