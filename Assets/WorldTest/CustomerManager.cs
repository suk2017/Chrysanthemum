using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{

    [Tooltip("模型")] public GameObject[] model;
    [Tooltip("模型范围")] public Vector2[] range;
    public Transform posLeft;
    public Transform posCenter;
    public Transform posRight;

    private Vector3 m_posLeft;
    private Vector3 m_posCenter;
    private Vector3 m_posRight;

    void Start()
    {
        /*******初始化*******/
        m_posLeft = posLeft.position;
        m_posCenter = posCenter.position;
        m_posRight = posRight.position;

        /*******检查是否一致*******/
        if (range.Length < model.Length)//若二者不一致 则取默认值
        {
            Vector2[] temp = new Vector2[model.Length];
            for (int i = range.Length; i < temp.Length; ++i)
            {
                temp[i] = new Vector2(10, 10);
            }
            range = temp;
        }


        /*******检查是否合理*******/
        for (int i = 0; i < range.Length; ++i)
        {
            if (range[i].x < 3)
            {
                range[i].x = 3;//出现频率最小值为3
            }
        }

        /*******启用协程*******/
        for (int i = 0; i < model.Length; ++i)
        {
            StartCoroutine(GenerateCustomer(i));
        }
    }

    private IEnumerator GenerateCustomer(int index)
    {
        while (true)
        {
            yield return new WaitForSeconds(range[index].x + Random.value * (range[index].y - range[index].x));
            Vector3 _pos;
            Vector3 _pos2;
            Vector3 _random = new Vector3(Random.value * 2 - 1, 0, Random.value * 2 - 1);
            bool fromLeft;
            if (Random.value > 0.5f)
            {
                _pos = m_posLeft + _random;
                _pos2 = m_posRight + _random;
                fromLeft = true;
            }
            else
            {
                _pos = m_posRight + _random;
                _pos2 = m_posLeft + _random;
                fromLeft = false;
            }
            Customer c = Instantiate(model[index], _pos, Quaternion.identity).GetComponent<Customer>();
            c.gameObject.SetActive(true);
            c.fromLeft = fromLeft;
            c.pos1 = m_posCenter + _random;
            c.pos2 = _pos2;
        }
    }

}
