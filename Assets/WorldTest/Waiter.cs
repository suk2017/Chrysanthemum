using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waiter : MonoBehaviour {

    public bool available = true;

    private string triggerTag;
    public delegate void fun1(Order o);

    public IEnumerator _Order(Order o,string targetTag, fun1 fun)
    {
        while (true)
        {
            if(tag == targetTag)
            {
                fun(o);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        tag = other.tag;
    }
}
