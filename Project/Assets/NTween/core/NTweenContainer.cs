using UnityEngine;

public class NTweenContainer : MonoBehaviour
{
    public static NTweenContainer genericContainer
    {
        get
        {
            if (_genericContainer == null)
            {
                var go = new GameObject("NTweenCommonContainer");
                DontDestroyOnLoad(go);
                _genericContainer = go.AddComponent<NTweenContainer>();
            }
            return _genericContainer;
        }
    }

    private static NTweenContainer _genericContainer;
    protected NTween _first;
    protected NTween _last;
    public void Add(NTween nt)
    {
        nt._prev = _last;
        if (_last != null)
        {
            _last._next = nt;
        }
        else
        {
            _first = nt;
        }
        _last = nt;
    }
    public void Remove(NTween nt)
    {
        nt.gc = true;
        if (nt._prev != null)
        {
            nt._prev._next = nt._next;
        }
        else if (nt == _first)
        {
            _first = nt._next;
        }
        if (nt._next != null)
        {
            nt._next._prev = nt._prev;
        }
        else if (nt == _last)
        {
            _last = nt._prev;
        }
        nt._next = nt._prev = null;
    }
    public void RemoveByTarget(object target,bool complete=false)
    {
        var pt = _first;
        while(pt != null)
        {
            var next = pt._next;
            if(pt.target == target)
            {
                if (complete)
                {
                    pt.complete();//complete will call remove
                }else
                    Remove(pt);
            }
            pt = next;
        }
    }
    public void RemoveAll(bool complete,bool destroySelf = true)
    {
        var p = _first;
        while (p != null)
        {
            var next = p._next;
            if(complete)
                p.complete();//complete will call remove
            else
                Remove(p);
            p = next;
        }
        if(destroySelf)
            Destroy(this);
    }
    public void Update()
    {
        var tween = _first;
        while (tween != null)
        {
            var playHead = (tween.useFrames) ? NTweenTimeManager.Instance._framePlayHead : NTweenTimeManager.Instance._timePlayHead;
            var next = tween._next;
            if (!tween.paused && playHead >= tween.startTime && !tween.gc)
            {
                if(tween.timeScale == 0)
                {
                    tween.render(tween.currentPlayHead);
                }else
                    tween.render((playHead - tween.startTime) * tween.timeScale);
            }
            tween = next;
        }
    }
}
