using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Customer : MonoBehaviour
{


    [HideInInspector] public Vector3 pos1;//门口
    [HideInInspector] public Vector3 pos2;//对应
    [HideInInspector] public bool fromLeft;//从左面生成的？
    private NavMeshAgent m_agent;
    

    private void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();

        if (Random.value < 0.5f)
        {
            m_agent.SetDestination(pos1);
        }
        else
        {
            m_agent.SetDestination(pos2);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "gate":
                {
                    if (Random.value < 0.25f)
                    {
                        m_agent.SetDestination(pos2);
                    }
                    else
                    {
                        Vector3 pos;
                        if (InnManager.current._GetPos(out pos))//若有空位
                        {
                            m_agent.SetDestination(pos);
                        }
                    }
                }
                break;
            case "seat":
                {
                    Order order = new Order();
                    order.pos = transform.position;
                    order.des = this;
                    order.meal[0] = Meal.getRandomMeal();

                }
                break;
            case "posLeft":
                {
                    if (!fromLeft)
                    {
                        Destroy(this.gameObject);
                    }
                }
                break;
            case "posRight":
                {
                    if (fromLeft)
                    {
                        Destroy(this.gameObject);
                    }
                }
                break;
        }
    }



}
