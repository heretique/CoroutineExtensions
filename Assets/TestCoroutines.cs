using UnityEngine;
using System.Collections;
using Atlas;

public class TestCoroutines : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(TestCoroutine());
	}
	
    IEnumerator TestCoroutine()
    {
        yield return new ParallelYield(Coroutine1(), Coroutine4());
        Debug.Log("Finished TestCoroutine!!!");
    }

    IEnumerator Coroutine1()
    {
        Debug.Log("Coroutine 1 Step 1");
        yield return null;
        Debug.Log("Coroutine 1 Step 2");
        yield return new WaitForSecondsYield(3);
        Debug.Log("Coroutine 1 Step 3");
    }

    IEnumerator Coroutine2()
    {
        Debug.Log("Coroutine 2 Step 1");
        yield return new WaitForSecondsYield(5);
        Debug.Log("Coroutine 2 Step 2");
        yield return null;
        Debug.Log("Coroutine 2 Step 3");
    }

    IEnumerator Coroutine3()
    {
        Debug.Log("Coroutine 3 Step 1");
        yield return null;
        Debug.Log("Coroutine 3 Step 2");
        yield return null;
        Debug.Log("Coroutine 3 Step 3");
    }

    IEnumerator Coroutine4()
    {
        Debug.Log("Coroutine 4 Step 1");
        yield return Coroutine3();
        Debug.Log("Coroutine 4 Step 2");
        yield return new SerialYield(Coroutine1(), Coroutine5());
        Debug.Log("Coroutine 4 Step 3");
    }

    IEnumerator Coroutine5()
    {
        Debug.Log("Coroutine 5 Step 1");
        yield return Coroutine1();
        Debug.Log("Coroutine 5 Step 2");
        yield return Coroutine2();
        Debug.Log("Coroutine 5 Step 3");
    }
}
