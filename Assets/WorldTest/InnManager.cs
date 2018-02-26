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
using UnityEngine.AI;

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
    /// <summary>取餐点 厨房</summary>
    private Transform[] meal;
    /// <summary>送餐点 客人</summary>
    private List<Transform> treat;
    /// <summary>碗筷点 客人</summary>
    private List<Transform> dishes;
    /// <summary>收餐点 厨房</summary>
    private Transform[] clear;

    /// <summary>收餐点是否可用</summary>
    private bool[] b_meal;
    /**********迎宾**********/
    /// <summary>迎宾点</summary>
    private Transform[] usher;
    /// <summary>驻马点</summary>
    private Transform[] stable;

    /**********厨师**********/
    /// <summary>烹调点</summary>
    private Transform[] cook;
    /// <summary>制作好的食物</summary>
    private Transform[] food;

    private bool[] b_cook;
    /**********账房**********/
    /// <summary>账房点</summary>
    private Transform[] book;

    /*********其它**********/
    /// <summary>厨师可处理的请求</summary>
    private List<Order> m_CookOrders;
    /// <summary>侍者可处理的请求</summary>
    private List<Order> m_WaiterOrders;
    /// <summary>账房可处理的请求</summary>
    private List<Order> m_CheckOrders;
    /// <summary>迎宾可处理的订单</summary>
    private List<Order> m_UsherOrders;

    /// <summary>所有可用的侍者</summary>
    private Waiter[] m_WaiterList
        ;
    //private Chief[] m_ChiefList;
    //private Usher[] m_UsherList;
    //private Accountant[] m_AccountantList;

    public static InnManager current;


    public static int OrderAvailableCount = 0;

    /***********内部方法***********/
    #region


    private void Start()
    {
        current = GetComponent<InnManager>();//全场只绑定一个InnManager
        _ResetValue();
        StartCoroutine(WaiterService());
        StartCoroutine(CookService());
    }

    /// <summary>
    /// waiter提供五种服务 点餐 取餐 上餐 拾餐 收餐
    /// </summary>
    private IEnumerator WaiterService()
    {
        if (m_WaiterOrders == null)
        {
            m_WaiterOrders = new List<Order>();
        }
        while (true)
        {
            /* 若还有可处理的订单
             * 不可处理的订单通过等待成为可处理的*/
            if (OrderAvailableCount > 0)
            {
                Waiter w = GetFreeWaiter();//获取空闲的侍者
                if (w != null)//如果还有空闲的
                {
                    for (int i = 0; i < m_WaiterOrders.Count; ++i)
                    {
                        if (m_WaiterOrders[i].available)
                        {
                            //StartCoroutine(w._Order(m_WaiterOrders[i]));

                            m_WaiterOrders.RemoveAt(i);
                            OrderAvailableCount -= 1;
                            break;
                        }
                    }
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// 等待点餐 → 等待上餐
    /// </summary>
    private void _Order(Order o)
    {
        //厨师生产菜品
        StartCoroutine(MakeMeal());
    }

    /// <summary>
    /// 取餐 args0为Order args1为Waiter自己
    /// </summary>
    private object _TakeMeal(object[] o)
    {
        Order _o = o[0] as Order;
        Waiter _w = o[1] as Waiter;

        //取餐
        food[_o.index].transform.parent = _w.transform;//Waiter持有该餐食
        food[_o.index].transform.position = _w.transform.position + _w.transform.forward * 0.5f;//餐食在Waiter的正前方

        //送餐（这个与取餐要连续执行 无需通过请求链发送Order）
        _w._Order(new Order()
        {
            customer = _o.customer,
            
        });
        _w._Order(_o.customer.transform, _Deliver);

        return null;
    }

    /// <summary>
    /// 送餐
    /// </summary>
    private object _Deliver(object o)
    {
        Order _o = o as Order;
        //_o.customer._Eat(new Order()
        //{
        //});

        return null;
    }

    /// <summary>
    /// 收拾桌子
    /// </summary>
    /// <param name="o"></param>
    private object _Clear(object o)
    {
        return null;
    }

    /// <summary>
    /// 收拾碗筷
    /// </summary>
    /// <param name="o"></param>
    private object _ClearTable(object o)
    {
        return null;
    }

    /// <summary>
    /// 送洗碗筷
    /// </summary>
    /// <param name="o"></param>
    private object _PutAway(object o)
    {
        return null;
    }

    /******* *******/

    /// <summary>
    /// 做饭
    /// </summary>
    /// <returns></returns>
    private IEnumerator MakeMeal()
    {
        //获取可使用的出餐口 这里的厨师默认空闲
        int index = GetFreeMeal();
        while (index < 0)
        {
            yield return new WaitForEndOfFrame();
            index = GetFreeMeal();
        }
        b_meal[index] = false;

        //cook播放动画 TODO其实并不需要在同一个餐桌 因为一般只有一个后厨
        print("制餐");


        //出菜
        Instantiate(Meal.SuSanXian.model, meal[index].position, Quaternion.identity);
        print("出餐");

        //上单
        m_WaiterOrders.Add(new Order()
        {
            available = true,
            tag = "meal",
            trf = meal[index],
            fun = _TakeMeal
        });
    }

    /// <summary>
    /// 每次开始正式营业前调用一次
    /// </summary>
    private void _ResetValue()
    {


        /**********顾客**********/
        GameObject[] temp = GameObject.FindGameObjectsWithTag("seat");
        if (seats == null)
        {
            seats = new Transform[temp.Length];
        }
        for (int i = 0; i < seats.Length; ++i)
        {
            seats[i] = temp[i].transform;
        }
        print("席位：" + seats.Length);

        temp = GameObject.FindGameObjectsWithTag("check");
        if (check == null)
        {
            check = new Transform[temp.Length];
        }
        for (int i = 0; i < check.Length; ++i)
        {
            check[i] = temp[i].transform;
        }
        print("结账点：" + check.Length);

        b_seats = new bool[seats.Length];
        for (int i = 0; i < b_seats.Length; ++i)
        {
            b_seats[i] = true;
        }


        /**********跑堂**********/
        if (order == null)
        {
            order = new List<Transform>();
        }

        temp = GameObject.FindGameObjectsWithTag("meal");
        if (meal == null)
        {
            meal = new Transform[temp.Length];
        }
        for (int i = 0; i < meal.Length; ++i)
        {
            meal[i] = temp[i].transform;
        }
        print("取餐点：" + meal.Length);

        if (treat == null)
        {
            treat = new List<Transform>();
        }

        if (dishes == null)
        {
            dishes = new List<Transform>();
        }

        temp = GameObject.FindGameObjectsWithTag("clear");
        if (clear == null)
        {
            clear = new Transform[temp.Length];
        }
        for (int i = 0; i < clear.Length; ++i)
        {
            clear[i] = temp[i].transform;
        }
        print("收餐点：" + clear.Length);


        if (b_meal == null)//取餐点是否可用
        {
            b_meal = new bool[meal.Length];
        }
        for (int i = 0; i < b_meal.Length; ++i)
        {
            b_meal[i] = true;
        }
        /**********迎宾**********/
        temp = GameObject.FindGameObjectsWithTag("usher");
        if (usher == null)
        {
            usher = new Transform[temp.Length];
        }
        for (int i = 0; i < usher.Length; ++i)
        {
            usher[i] = temp[i].transform;
        }
        print("迎宾点：" + usher.Length);

        temp = GameObject.FindGameObjectsWithTag("stable");
        if (stable == null)
        {
            stable = new Transform[temp.Length];
        }
        for (int i = 0; i < stable.Length; ++i)
        {
            stable[i] = temp[i].transform;
        }
        print("驻马点：" + stable.Length);

        /**********厨师**********/
        temp = GameObject.FindGameObjectsWithTag("cook");
        if (cook == null)
        {
            cook = new Transform[temp.Length];
        }
        for (int i = 0; i < cook.Length; ++i)
        {
            cook[i] = temp[i].transform;
        }
        print("厨师点：" + cook.Length);

        //food = null;

        b_cook = new bool[cook.Length];
        for (int i = 0; i < b_cook.Length; ++i)
        {
            b_cook[i] = true;
        }
        /**********账房**********/
        temp = GameObject.FindGameObjectsWithTag("book");
        if (book == null)
        {
            book = new Transform[temp.Length];
        }
        for (int i = 0; i < book.Length; ++i)
        {
            book[i] = temp[i].transform;
        }
        print("账房点：" + book.Length);

        /*********其它**********/

        temp = GameObject.FindGameObjectsWithTag("waiter");
        m_WaiterList = new Waiter[temp.Length];
        for (int i = 0; i < temp.Length; ++i)
        {
            m_WaiterList[i] = temp[i].GetComponent<Waiter>();
        }

    }

    /// <summary>
    /// 获取空闲的侍者
    /// </summary>
    private Waiter GetFreeWaiter()
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

    /// <summary>
    /// 获取放食物的地方 但是如果没有 那么就返回-1
    /// </summary>
    /// <returns></returns>
    private int GetFreeMeal()
    {
        //TODO 需要和厨师联系起来
        for (int i = 0; i < b_meal.Length; ++i)
        {
            if (b_meal[i])
            {
                return i;
            }
        }
        return -1;
    }


    #endregion

    /**********顾客**********/
    /// <summary>
    /// 获取一个空座位 如果没有 则不进入/排队等待
    /// </summary>
    public bool _GetPos(out Vector3 pos)
    {
        for (int i = 0; i < 3; ++i)//连续三次判断是否有位置 都没有就离开
        {
            if (b_seats == null)
            {
                b_seats = new bool[seats.Length];
                for (int j = 0; j < b_seats.Length; ++j)
                {
                    b_seats[i] = true;
                }
                print("b_seats为空 已赋值");
            }
            int value = (int)(Random.value * b_seats.Length);
            if (b_seats[value])
            {
                pos = seats[value].position;
                b_seats[value] = false;
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
        m_WaiterOrders.Add(o);
        OrderAvailableCount += 1;
    }

    public void _AddWaiterOrder(Order o)
    {
        m_WaiterOrders.Add(o);
        OrderAvailableCount += 1;
    }

    public void _AddCookOrder(Order o)
    {
        m_CookOrders.Add(o);

    }

    /// <summary>
    /// 去结账
    /// </summary>
    public bool _GoToCheckOut(NavMeshAgent customer)
    {
        int min = int.MaxValue;
        int index = 0;
        for (int i = 0; i < check.Length; ++i)//选取最小值 使得各结账台排队较为均衡
        {
            int value = check[i].GetComponent<QueueHead>().count;
            if (value < min)
            {
                min = value;
                index = i;
            }
        }
        check[index].GetComponent<QueueHead>()._GetInQueue(customer);//去排队 寻路又QueueHead类控制
        return true;
    }


    /*******厨师*******/
    public IEnumerator CookService()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.05f);
            //检查订单是否还有需要处理者
            if (m_CookOrders.Count <= 0)
            {
                continue;//订单列表已空
            }
            //检查出餐口是否有空余位置
            int index = GetFreeMeal();
            if (index < 0)
            {
                Debug.LogWarning("出餐口已满");
                continue;
            }
            //检查是否有可用厨师
            int i;
            for (i = 0; i < cook.Length; ++i)
            {
                /*******正式处理*******/
                if (b_cook[i])
                {
                    print("制餐");
                    //发送菜品 并回调TakeMeal方法
                    {
                        yield return new WaitForSeconds(2f);//模拟制餐动画
                        _Meal(index, m_CookOrders[0]);//模拟回调方法
                    }
                    m_CookOrders.RemoveAt(0);//移除过期订单
                }
            }
            if (i >= cook.Length)//正常应该是相等
            {
                print("没有空闲厨师");
            }
            else
            {
                print("开始制菜");
            }

        }
    }
    /// <summary>
    /// 制餐结束 立刻出餐 立刻通知Waiter取餐
    /// </summary>
    public void _Meal(int index, Order o)
    {
        //出餐
        Instantiate(o.meal[0].model, meal[index].position, Quaternion.identity);
        print("出餐");

        //上单
        m_WaiterOrders.Add(new Order()
        {
            customer = o.customer,
            available = true,
            tag = "meal",
            trf = meal[index],
            fun = _TakeMeal,
            index = index,
        });
    }

    /// <summary>
    /// 返回最短距离的收餐点
    /// </summary>
    public Vector3 _GetRecyclePos(Vector3 Pos)
    {
        int index = 0;
        float minLength = float.MaxValue;
        for(int i = 0; i < meal.Length; ++i)
        {
            float length = Vector3.SqrMagnitude(Pos - meal[i].position);
            if(minLength > length)
            {
                minLength = length;
                index = i;
            }
        }
        return meal[index].position;
    }
}


