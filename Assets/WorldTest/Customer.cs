using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Customer :MonoBehaviour
{
    [HideInInspector] public Vector3 pos1;//门口
    [HideInInspector] public Vector3 pos2;//对应
    [HideInInspector] public bool fromLeft;//从左面生成的？
    public delegate object fun1(object o);

    private Vector3 pos;
    private NavMeshAgent m_agent;
    private bool m_think;//思考是否在这个客栈里吃饭
    private bool? m_getIn = null;//false 意味着是从客栈中离开 null意味着没有进入过
    private bool m_seated = false;//是否就坐
    private bool m_checked = false;

    /*******加速*******/
    private Transform tr;

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "posLeft":
                {
                    if (!fromLeft)
                    {
                        Destroy(gameObject);
                    }
                }
                break;
            case "posRight":
                {
                    if (fromLeft)
                    {
                        Destroy(gameObject);
                    }
                }
                break;
            case "posCenter":
                {
                    if (m_think)//停下来思考是否进去吃饭 而不是直接路过门口
                    {
                        StartCoroutine(JustWait(3f));
                    }
                }
                break;
            case "gate"://到达门口
                {
                    print("乘客进门");
                    if (m_getIn == true)//如果是进入方向
                    {
                        //TODO 发出点餐请求 一个侍者跟随
                        InnManager.current._AddWaiterOrder(new Order()
                        {
                            customer = this,
                            available = true,
                            tag = "seat",
                            trf = tr,
                            fun = delegate (object[] o)
                            {
                                NavMeshAgent agent = o[0] as NavMeshAgent;
                                agent.SetDestination(agent.transform.position);//开始点餐之后就停止移动
                                    InnManager.current._AddCookOrder(new Order()//厨师做饭
                                    {
                                    customer = this,
                                    meal = new Meal[] { Meal.GetRandomMeal() },
                                    available = true,
                                    tag = "meal",
                                });
                                return null;
                            }

                        });
                    }
                    else if (m_getIn == false)//如果是离开方向
                    {
                        //TODO 慢走您内
                    }
                }
                break;
            case "seat"://到达任意一处坐席
                {
                    if (m_seated)//如果到达了自己的位置
                    {
                        //TODO 上菜咯
                    }
                    else //如果尚未到达自己的位置（例如踩到了别人的位置）或者食毕准备离开
                    {

                    }
                }
                break;
            case "check"://到达结账点
                {
                    if (m_getIn == true)//如果是刚要进入
                    {

                    }
                    else if (m_getIn == false && !m_checked) //如果是即将离开且尚未结账
                    {
                        //TODO 结账
                        m_checked = true;//防止误入其它柜台继续结账
                    }
                }
                break;
        }
    }

    /// <summary>
    /// 停下来思考是否要走进去
    /// </summary>
    private IEnumerator JustWait(float time)
    {
        yield return new WaitForSeconds(time);//等待一段时间观察
        if (Random.value < 0.25f)
        {
            m_agent.SetDestination(pos2);//直接走过去
        }
        else//打算留下来
        {
            if (InnManager.current._GetPos(out pos))//若有空位
            {
                m_agent.SetDestination(pos);
                m_getIn = true;
                StartCoroutine(isSeated());
                print("看到空位");
            }
            else
            {
                m_agent.SetDestination(pos2);//没有位置了
                print("没看到空位");
            }
        }
    }

    /// <summary>
    /// 是否已经就坐？
    /// </summary>
    /// <returns></returns>
    private IEnumerator isSeated()
    {
        while (Vector3.SqrMagnitude(pos - tr.position) > 0.4f)//若到达位置了 则说明就座了
        {
            yield return new WaitForSeconds(0.1f);
        }
        m_seated = true;
        print("已就坐");
    }

    /*******公有方法*******/
    public void 
}

#region
//[RequireComponent(typeof(NavMeshAgent))]
//public class Customer : MonoBehaviour
//{


//    /****************************/
//    [HideInInspector] public Vector3 pos1;//门口
//    [HideInInspector] public Vector3 pos2;//对应
//    [HideInInspector] public bool fromLeft;//从左面生成的？
//    public delegate object fun1(object o);

//    private Vector3 pos;
//    private NavMeshAgent m_agent;
//    private bool m_think;//思考是否在这个客栈里吃饭
//    private bool? m_getIn = null;//false 意味着是从客栈中离开 null意味着没有进入过
//    private bool m_seated = false;//是否就坐
//    private bool m_checked = false;

//    /*******加速*******/
//    private Transform tr;

//    /*******内部*******/
//    private void Awake()
//    {
//        tr = transform;
//    }

//    private void Start()
//    {
//        m_agent = GetComponent<NavMeshAgent>();

//        if (Random.value < 0.5f)
//        {
//            m_agent.SetDestination(pos1);//停下来观察
//            m_think = true;
//        }
//        else
//        {
//            m_agent.SetDestination(pos2);//直接走过去
//            m_think = false;
//        }
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        switch (other.tag)
//        {
//            case "posLeft":
//                {
//                    if (!fromLeft)
//                    {
//                        Destroy(gameObject);
//                    }
//                }
//                break;
//            case "posRight":
//                {
//                    if (fromLeft)
//                    {
//                        Destroy(gameObject);
//                    }
//                }
//                break;
//            case "posCenter":
//                {
//                    if (m_think)//停下来思考是否进去吃饭 而不是直接路过门口
//                    {
//                        StartCoroutine(JustWait(3f));
//                    }
//                }
//                break;
//            case "gate"://到达门口
//                {
//                    print("乘客进门");
//                    if (m_getIn == true)//如果是进入方向
//                    {
//                        //TODO 发出点餐请求 一个侍者跟随
//                        InnManager.current._AddWaiterOrder(new Order()
//                        {
//                            customer = this,
//                            available = true,
//                            tag = "seat",
//                            trf = tr,
//                            fun = delegate (object[] o)
//                            {
//                                NavMeshAgent agent = o[0] as NavMeshAgent;
//                                agent.SetDestination(agent.transform.position);//开始点餐之后就停止移动
//                                InnManager.current._AddCookOrder(new Order()//厨师做饭
//                                {
//                                    customer = this,
//                                    meal = new Meal[] { Meal.GetRandomMeal() },
//                                    available = true,
//                                    tag = "meal",
//                                });
//                                return null;
//                            }

//                        });
//                    }
//                    else if (m_getIn == false)//如果是离开方向
//                    {
//                        //TODO 慢走您内
//                    }
//                }
//                break;
//            case "seat"://到达任意一处坐席
//                {
//                    if (m_seated)//如果到达了自己的位置
//                    {
//                        //TODO 上菜咯
//                    }
//                    else //如果尚未到达自己的位置（例如踩到了别人的位置）或者食毕准备离开
//                    {

//                    }
//                }
//                break;
//            case "check"://到达结账点
//                {
//                    if (m_getIn == true)//如果是刚要进入
//                    {

//                    }
//                    else if (m_getIn == false && !m_checked) //如果是即将离开且尚未结账
//                    {
//                        //TODO 结账
//                        m_checked = true;//防止误入其它柜台继续结账
//                    }
//                }
//                break;
//        }
//    }

//    /// <summary>
//    /// 停下来思考是否要走进去
//    /// </summary>
//    private IEnumerator JustWait(float time)
//    {
//        yield return new WaitForSeconds(time);//等待一段时间观察
//        if (Random.value < 0.25f)
//        {
//            m_agent.SetDestination(pos2);//直接走过去
//        }
//        else//打算留下来
//        {
//            if (InnManager.current._GetPos(out pos))//若有空位
//            {
//                m_agent.SetDestination(pos);
//                m_getIn = true;
//                StartCoroutine(isSeated());
//                print("看到空位");
//            }
//            else
//            {
//                m_agent.SetDestination(pos2);//没有位置了
//                print("没看到空位");
//            }
//        }
//    }

//    /// <summary>
//    /// 是否已经就坐？
//    /// </summary>
//    /// <returns></returns>
//    private IEnumerator isSeated()
//    {
//        while (Vector3.SqrMagnitude(pos - tr.position) > 0.4f)//若到达位置了 则说明就座了
//        {
//            yield return new WaitForSeconds(0.1f);
//        }
//        m_seated = true;
//        print("已就坐");
//    }



//    /// <summary>
//    /// 食用
//    /// </summary>
//    private IEnumerator Eat(Order o, fun1 fun)
//    {
//        yield return new WaitForSeconds(3f);

//        //TODO 通知侍者拾餐
//        InnManager.current._AddWaiterOrder(new Order()
//        {
//            customer = o.customer,
//            meal = o.meal,
//            available = true,
//            tag = "dishes",
//            trf = tr,
//            fun = delegate (object[] _o)
//            {
//                return null;
//            }
//        });

//        m_seated = false;//食毕离席

//        //TODO 前往结账

//    }

//    /*******使用*******/

//    public void _Eat(Order o, fun1 _fun)
//    {
//        //播放动画
//        StartCoroutine(Eat(o, _fun));//等待一段时间之后通知收餐 并去结账
//    }

//}
#endregion


