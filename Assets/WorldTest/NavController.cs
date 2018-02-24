using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavController : MonoBehaviour
{

    public Transform[] target;
    public Transform comeIn;
    public Transform leaveOut;
    public Transform standHere;
    public Transform payTheBill;

    private NavMeshAgent m_agent;

    void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            m_agent.SetDestination(target[0].position);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            m_agent.SetDestination(target[1].position);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            m_agent.SetDestination(target[2].position);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            m_agent.SetDestination(target[3].position);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            m_agent.SetDestination(target[4].position);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            m_agent.SetDestination(target[5].position);
        }

        else if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            m_agent.SetDestination(comeIn.position);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            m_agent.SetDestination(standHere.position);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            m_agent.SetDestination(leaveOut.position);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            m_agent.SetDestination(payTheBill.position);
        }
    }
}
