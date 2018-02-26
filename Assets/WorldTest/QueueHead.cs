using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 这个是排队系统的队头 用于减去队伍数量
/// </summary>
public class QueueHead : MonoBehaviour
{
    public int count//获取排队人数
    {
        get
        {
            return tailList.Count;
        }
    }

    private List<NavMeshAgent> tailList;//排队列表
    private bool m_checked;//结束最前一个人的排队状态
    private Vector3 m_pos;//离开队伍时前往的地点
    private List<Transform> trList;//加速变量

    private void Awake()
    {
        tailList = new List<NavMeshAgent>();
        trList = new List<Transform>();
    }

    private void Start()
    {
        StartCoroutine(QueueRun());
    }


    /// <summary>
    /// 从最前离开队伍 不考虑意外
    /// </summary>
    private IEnumerator QueueRun()
    {
        while(tailList.Count > 0)
        {
            yield return new WaitForEndOfFrame();
            if (m_checked)
            {
                tailList[0].SetDestination(m_pos);//移开第一个
                trList.RemoveAt(0);
                tailList.RemoveAt(0);
                m_checked = false;
            }
        }
    }

    /// <summary>
    /// 保持队员一直排队
    /// </summary>
    /// <returns></returns>
    private IEnumerator StandInLine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            for(int i = 1; i < tailList.Count; ++i)
            {
                tailList[i].SetDestination(trList[i -1].position);
            }
            tailList[0].SetDestination(transform.position);
        }
    }

    /*******外部调用*******/

    /// <summary>
    /// 从最后进入队伍 不考虑插队
    /// </summary>
    public void _GetInQueue(NavMeshAgent t)
    {
        tailList.Add(t);
        trList.Add(t.transform);
    }

    /// <summary>
    /// 一名队员离开队伍
    /// </summary>
    /// <param name="pos"></param>
    public void _Leave(Vector3 pos)
    {
        m_pos = pos;
        m_checked = true;
    }

}
