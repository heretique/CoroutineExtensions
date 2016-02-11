#Coroutine Extensions

This is an attempt to extended the basic behavior of coroutines in Unity3D by adding support for more complex yielding and eventually add support for return values, exception handling and soft stopping coroutines. If you find any issues with these utilities or you have any improvement idea please use the [Issues](https://github.com/heretique/CoroutineExtensions/issues) section.

##Background

Inspired by Jean Simonet's [blog post](http://gamasutra.com/blogs/JeanSimonet/20160128/264083/Logic_Over_Time.php)  on [Gamasutra](http://gamasutra.com/) I set out to implement two new yield instructions **ParallelYield** and **SerialYield** that would allow for new ways of using the coroutines.

What I found out is that coroutines in Unity are a bit of a mess in the way they're implemented right now and how they left room for improvement for new yield instructions. Right now there are three types of possible yield instructions, the native ones derived from **YieldInstruction** (WaitForSeconds, WaitForEndOfFrame, WaitForFixedUpdate), custom ones derived from **CustomYieldInstruction** and the yeild instructions that inherit **IEnumerator**, the ones that give the most flexibility out of them all. And let's not forget **WWW** yields using WWW that is in a category of it's own it seems.
Why Unity guys didn't just used one general approach based on IEnumerator alone I don't know. But as it is, it's hard to combine them all in some cases.

That's why I decided that these utility classes should use only IEnumerators and to use them exclusively without using any of the existing Unity yield instructions. (to get rig of a lot of headaches).

##Usage

*!!! Note !!!: You cannot use ParallelYield and SerialYield yield instructions combined with any of Unity's default yield instructions. For simplicity they assume all yields are IEnumerator and they cannot step the default ones. To remove this limitation I have re-implemented most of the natives ones to inherit IEnumerator.*

Bellow is an excerpt taken from the test Monobehaviour:

    public class TestCoroutines : MonoBehaviour
    {

        void Start()
        {
            StartCoroutine(TestCoroutine());
        }

        IEnumerator TestCoroutine()
        {
            yield return new ParallelYield(Coroutine1(), Coroutine4());
            ...
        }

        IEnumerator Coroutine1()
        {
            ...
            yield return null;
            ...
            yield return new WaitForSecondsYield(3);
            ...
        }

        IEnumerator Coroutine2()
        {
            ...
            yield return new WaitForSecondsYield(5);
            ...
            yield return null;
            ...
        }

        IEnumerator Coroutine3()
        {
            ...
            yield return null;
            ...
            yield return null;
            ...
        }

        IEnumerator Coroutine4()
        {
            ...
            yield return Coroutine3();
            ...
            yield return new SerialYield(Coroutine1(), Coroutine5());
            ...
        }

        IEnumerator Coroutine5()
        {
            ...
            yield return Coroutine1();
            ...
            yield return Coroutine2();
            ...
        }
    }

##Limitations

- As stated it is preferably to avoid using ParallelYield and SerialYield combined with Unity yield implementations.
- I don't have yet and implementation for **WaitForEndOfFrame** and **WaitForFixedUpdate**. **Any help more than appreciated**
- The current implementation doesn't have exception handling nor return values support

