using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Atlas
{

    public static class YieldUtils
    {
        public static IEnumerator GetEnumerator(object candidate)
        {
            if (candidate != null && typeof(IEnumerator).IsAssignableFrom(candidate.GetType()))
                return (IEnumerator)candidate;

            return null;
        }

        public static IEnumerator GetLeafEnumerator(object candidate, IEnumerator deadLeaf = null)
        {
            IEnumerator enumerator = GetEnumerator(candidate);
            IEnumerator leaf = null;
            while (enumerator != null && enumerator != deadLeaf)
            {
                leaf = enumerator;
                enumerator = GetEnumerator(enumerator.Current);
            }

            return leaf;
        }
    }

    public class ParallelYield : IEnumerator
    {
        List<IEnumerator> _coroutines;

        public ParallelYield(params IEnumerator[] coroutines)
        {
            _coroutines = new List<IEnumerator>(coroutines);
        }

        public object Current { get { return null; } }

        public bool MoveNext()
        {
            for (var i = _coroutines.Count - 1; i >= 0; --i)
            {
                // find leaf enumerator to move next on
                IEnumerator leaf = YieldUtils.GetLeafEnumerator(_coroutines[i]);
                if (leaf != null && leaf != _coroutines[i])
                {
                    while (leaf != null && !leaf.MoveNext())
                    {
                        leaf = YieldUtils.GetLeafEnumerator(_coroutines[i], leaf);
                    }
                    if (leaf == null && !_coroutines[i].MoveNext())
                    {
                        _coroutines.RemoveAt(i);
                    }
                }
                else if (!_coroutines[i].MoveNext())
                {
                    _coroutines.RemoveAt(i);
                }
            }

            if (_coroutines.Count == 0)
                return false;

            return true;
        }

        public void Reset() { }
    }

    public class SerialYield : IEnumerator
    {
        Queue<IEnumerator> _coroutines;
        IEnumerator _current;

        public SerialYield(params IEnumerator[] coroutines)
        {
            _coroutines = new Queue<IEnumerator>(coroutines);
            _current = _coroutines.Dequeue();
        }

        public object Current { get { return null; } }

        public bool MoveNext()
        {
            IEnumerator leaf = YieldUtils.GetLeafEnumerator(_current);
            if (leaf != null && leaf != _current)
            {
                while (leaf != null && !leaf.MoveNext())
                {
                    leaf = YieldUtils.GetLeafEnumerator(_current, leaf);
                }
                if (leaf == null && !_current.MoveNext())
                {
                    if (_coroutines.Count == 0)
                        return false;

                    _current = _coroutines.Dequeue();
                }
            }
            else if (!_current.MoveNext())
            {
                if (_coroutines.Count == 0)
                    return false;

                _current = _coroutines.Dequeue();
            }

            return true;
        }

        public void Reset() { }
    }

    public class WaitForSecondsYield : IEnumerator
    {
        float seconds;
        float startTime;

        public object Current { get { return null; } }

        public WaitForSecondsYield(float seconds)
        {
            this.seconds = seconds;
            startTime = Time.realtimeSinceStartup;
        }

        public bool MoveNext() { return Time.realtimeSinceStartup - startTime <= seconds; }

        public void Reset() { }
    }

    public class WWWYeild : IEnumerator
    {
        WWW _www;

        public WWWYeild(WWW www) { _www = www; }

        public object Current { get { return null; } }

        public bool MoveNext() { return !_www.isDone; }

        public void Reset() { }
    }

    class WaitWhile : IEnumerator
    {
        Func<bool> _predicate;

        public WaitWhile(Func<bool> predicate) { _predicate = predicate; }

        public object Current { get { return null; } }

        public bool MoveNext() { return _predicate(); }

        public void Reset() { }
    }

    class WaitUntil : IEnumerator
    {
        Func<bool> _predicate;

        public WaitUntil(Func<bool> predicate) { _predicate = predicate; }

        public object Current { get { return null; } }

        public bool MoveNext() { return !_predicate(); }

        public void Reset() { }
    }
}

