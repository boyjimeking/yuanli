using System;
using UnityEngine;
using System.Collections;
public delegate void NTweenCallback(NTween tween,object args);
public delegate float NTweenEaseFunction(float t, float b, float c, float d);
public class NTween : NTweenCore
{
    public static float globalTimeScale
    {
        get { return NTweenTimeManager.Instance.globalTimeScale; }
        set
        {
            NTweenTimeManager.Instance.globalTimeScale = value;
        }
    }
    public static bool globalPause
    {
        get { return NTweenTimeManager.Instance.paused; }
        set { NTweenTimeManager.Instance.paused = value; }
    }
    protected static Hashtable _reservedVars;
    protected static Hashtable plugins;
    static NTween()
    {
        _reservedVars = new Hashtable();
        //if gameObject is specified,tweens are attached to the gameObject,and tweens will be destroyed when gameObject destroyed.
        //NTween always try to use the component gameObject,if no gameObject is specified and no gameObject property is found,a generic gameObject is used.
        //If generic gameObject is used,user should ensure tweens are completed or killed when target get destroyed.
        _reservedVars["gameObject"] = null;
        _reservedVars["ease"] = null;
        _reservedVars["delay"] = 0;
        _reservedVars["loop"] = 0;
        _reservedVars["loopDelay"] = 0;
        _reservedVars["yoyo"] = false;
        _reservedVars["yoyoReverse"] = false;//same as yoyo except ease is reversed
        _reservedVars["useFrames"] = false;
        _reservedVars["skipPlugin"] = false;//used if name conflict with plugin name.
        _reservedVars["overwrite"] = 0;
        _reservedVars["renderNow"] = false;
        _reservedVars["onStart"] = null;
        _reservedVars["onStartParams"] = null;
        _reservedVars["onUpdate"] = null;
        _reservedVars["onUpdateParams"] = null;
        _reservedVars["onComplete"] = null;
        _reservedVars["onCompleteParams"] = null;
        _reservedVars["onLoopComplete"] = null;
        _reservedVars["onLoopCompleteParams"] = null;

        //private
        _reservedVars["__relative__"] = false; //is the vars absolute or relative
        _reservedVars["__fromVars__"] = null;//used by fromTo method
        _reservedVars["__from__"] = false;
        plugins = new Hashtable();
        activatePlugins(new object[]
                                  {
                                    typeof(AlphaPlugin),
                                    typeof(PositionPlugin),
                                    typeof(XPlugin),
                                    typeof(YPlugin),
                                    typeof(ZPlugin),
                                    typeof(ScalePlugin),
                                    typeof(ScaleXPlugin),
                                    typeof(ScaleYPlugin),
                                    typeof(ScaleZPlugin),
                                    typeof(RotationPlugin),
                                    typeof(ColorPlugin),
                                  });
    }
	private float _duration;

    private Hashtable vars; 
	public float startTime;
	public object target;
	public bool gc;
	private bool _useFrames;

    protected NTweenEaseFunction ease;
    private NTweenEaseFunction easeOriginal;
	protected bool _initted;
    private NTweenData _firstPT;

    public event NTweenCallback onStart;
    public object onStartParams;

    public event NTweenCallback onUpdate;
    public object onUpdateParams;

    public event NTweenCallback onComplete;
    public object onCompleteParams;

    public event NTweenCallback onLoopComplete;
    public object onLoopCompleteParams;

    public NTween _prev;
    public NTween _next;

    private float _timeScale;
    private float _currentPlayHead;

    protected NTweenContainer _container;
    private bool _paused;
    private float _pausedTime;
    private int _totalLoop;
    private int _loop;
    private bool _yoyo;
    private bool _yoyoReverse;
    private float _loopDelay;
    private bool _reversed;

    public GameObject gameObject;
    private float delay;
    

    public NTween(object target, float duration, Hashtable vars,NTweenContainer container)
    {
        if(target == null)
        {
            throw new Exception("ntween target is null");
        }
        
		this.vars = CleanArgs(vars);
		this._duration = duration;
		this.target = target;
        this._container = container;
        this.gameObject = container.gameObject;
		if (null == this.vars["ease"]) {
			ease = Linear.EaseOut;
		} else {
			ease = (NTweenEaseFunction)this.vars["ease"];
		}
        easeOriginal = ease;
        this._timeScale = 1.0f;
		this._useFrames = SafeGetVar<bool>(vars,"useFrames");
		delay = SafeGetVar<float>(vars,"delay");
        if (vars.ContainsKey("loop"))
        {
            if (vars["loop"] is bool)
            {
                _totalLoop = this._loop = (bool)vars["loop"] ? -1 : 0;
            }
            else
            {
                _totalLoop = this._loop = SafeGetVar<int>(vars, "loop");
            }
        }
        this._loopDelay = SafeGetVar<float>(vars, "loopDelay");
        this._yoyo = SafeGetVar<bool>(vars, "yoyo");
        this._yoyoReverse = SafeGetVar<bool>(vars, "yoyoReverse");
        if ((this._yoyo || this._yoyoReverse) && _totalLoop == 0)
        {
            _totalLoop = _loop = -1;
        }
        this.startTime = nowTime + delay;
        this.onUpdate = SafeGetVar<NTweenCallback>(vars, "onUpdate");
        this.onUpdateParams = SafeGetVar<object>(vars, "onUpdateParams");

        this.onComplete = SafeGetVar<NTweenCallback>(vars, "onComplete");
        this.onCompleteParams = SafeGetVar<object>(vars, "onCompleteParams");

        this.onLoopComplete = SafeGetVar<NTweenCallback>(vars, "onLoopComplete");
        this.onLoopCompleteParams = SafeGetVar<object>(vars, "onLoopCompleteParams");

		if (SafeGetVar<int>(vars,"overwrite") == 1)
		{
            container.RemoveByTarget(target);
		}
			
		if (SafeGetVar<bool>(vars,"renderNow") || (duration == 0 && delay == 0)) {
			render(0);
		}
	}
    public bool paused
    {
        get { return _paused; }
        set
        {
            if(_paused != value)
            {
                _paused = value;
                if(!_paused)
                {
                    if(timeScale != 0)
                    {
                        startTime = nowTime - currentPlayHead/timeScale;
                    }

                }
            }
        }
    }
    public float timeScale
    {
        get { return _timeScale; }
        set
        {
            if(value != 0)
            {
                this.startTime = nowTime - currentPlayHead / value;
            }
            _timeScale = value;
        }
    }
    public float duration
    {
        get { return _duration; }
    }
    public bool useFrames
    {
        get { return _useFrames; }
    }
    public float currentPlayHead
    {
        get { return _currentPlayHead; }
    }
    private float nowTime
    {
        get
        {
            return _useFrames ? NTweenTimeManager.Instance._framePlayHead : NTweenTimeManager.Instance._timePlayHead;
        }
    }
    public override string ToString()
    {
        return "NTween[target:" + target + ",vars:" + vars + "]\n";
    }

	private void init()
	{
        if (vars["__fromVars__"] != null)
        {
            var fromVars = (Hashtable) vars["__fromVars__"];
            fromVars["overwrite"] = 0;
            fromVars["renderNow"] = true;
            to(target, 0, fromVars);
        }

	    this._firstPT = null;
        parseTween(target);
		if (SafeGetVar<bool>(vars,"__from__")) 
        {
            var pt = this._firstPT;
			while (pt != null) 
            {
				pt.start = Addition(pt.start,pt.change);
				pt.change = Negation(pt.change);
				pt = pt._next;
			}
		}
		_initted = true;
	}
	protected void parseTween(object target)
	{
        bool isRelative = SafeGetVar<bool>(vars,"__relative__");
	    bool isSkipPlugin = SafeGetVar<bool>(vars, "skipPlugin");
        foreach (string p in this.vars.Keys)
        {
            if (!(_reservedVars.ContainsKey(p)))
            {
                NTweenPlugin plugin;
                if (!isSkipPlugin && plugins.ContainsKey(p) && (plugin = (NTweenPlugin)Activator.CreateInstance((Type)plugins[p])).onInit(target, vars[p], isRelative, this))
                {
                    this._firstPT = new NTweenData(plugin,
                                                    "setRatio",
                                                    0f,
                                                    1f,
                                                    plugin.propName,
                                                    true,
                                                    this._firstPT);
                }
                else
                {
                    var s = GetPropValue(target, p);
                    if (null == s)
                    {
                        throw new Exception("Unknown property:" + p + " of " + target);
                    }else
                        _firstPT = new NTweenData(target, p, s,(isRelative ? vars[p] : Subtraction(vars[p], s)), p, false, _firstPT);
                }
            }
        }
	}

	public void render(float playHead) {
		if (!_initted) {
			init();
            if (onStart != null)
                onStart(this,onStartParams);
		}else if(_currentPlayHead == playHead)//if we are render same time,just call onUpdate
		{
            if (onUpdate != null)
            {
                onUpdate(this,onUpdateParams);
            }
		    return;
		}
	    _currentPlayHead = playHead;
        if (_reversed)
            playHead = this.duration - playHead;
	    float ratio;
		if (playHead >= this.duration) {
			ratio = 1;
		} else if (playHead <= 0) {
			ratio = 0;
		} else {
			ratio = ease(playHead, 0, 1, this.duration);			
		}
        var pt = this._firstPT;
		while (pt != null)
		{
            SetPropValue(pt.target,pt.property,Addition(pt.start, Multiply(pt.change, ratio)));
			pt = pt._next;
		}
		if (onUpdate != null)
	    {
	        onUpdate(this,onUpdateParams);
		}
		if (playHead >= this.duration || (_reversed && playHead <= 0)) {
            if(_totalLoop != 0)
                if (onLoopComplete != null)
                {
                    onLoopComplete(this,onLoopCompleteParams);
                }
            if(_loop != 0)
            {
                if(_loop > 0)
                    --_loop;
                if (_reversed)
                {
                    startTime = nowTime + playHead;
                }
                else
                {
                    startTime = nowTime + (playHead - this.duration);
                }
                startTime += _loopDelay / timeScale;
                _currentPlayHead = 0;
                if (_yoyoReverse || _yoyo)
                {
                    _reversed = !_reversed;
                }
                if(_yoyoReverse)
                {
                    ease = _reversed ? easeReversed : easeOriginal;
                }
            }
            else
            {
                complete(true);
            }
		}
	}
	public void kill(params string[] varListToKill)
	{
        if (varListToKill.Length == 0)
        {
            _container.Remove(this);
        }
        else
        {
            var pt = _firstPT;
            while(pt != null)
            {
                var next = pt._next;
                foreach (var s in varListToKill)
                {
                    if (s == pt.name)
                    {
                        if (pt._prev != null)
                        {
                            pt._prev._next = pt._next;
                        }
                        else if (pt == _firstPT)
                        {
                            _firstPT = pt._next;
                        }
                        if (pt._next != null)
                        {
                            pt._next._prev = pt._prev;
                        }
                        pt._next = pt._prev = null;
                    }
                }
                pt = next;
            }
            if(_firstPT == null)
            {
                _container.Remove(this);
            }
        }
	}
    public void complete(bool skipRender = false)
    {
        if (!skipRender)
        {
            _loop = 0;//no more loops
            render(this.duration);
            return;
        }
        kill();
        if (onComplete != null)
        {
            onComplete(this,onCompleteParams);
        }
    }
    public void reverse()
    {
        var prevReversed = _reversed;
        if (prevReversed)
        {
            this.ease = easeOriginal;
        }
        else
        {
            ease = easeReversed;
        }
        back();
	}
    public void back()
    {
        _reversed = !_reversed;

        if (timeScale != 0)
            this.startTime = nowTime - (duration - currentPlayHead / timeScale);
    }
    private float easeReversed(float t, float b, float c, float d) 
    {
        return 1 - easeOriginal(d - t, b, c, d);
	}
    public void restart(bool skipDelay = true) {
		if (skipDelay) {
            this.startTime = nowTime;
		} else {
            if(timeScale != 0)
				this.startTime = nowTime + (this.delay / this.timeScale);
		}
        _reversed = false;
		_loop = _totalLoop;
        render(0);
        paused = false;
    }
    #region Static Functions
    private static NTween NewNTween(object target,float duration,Hashtable vars)
    {
        GameObject go = SafeGetVar<GameObject>(vars, "gameObject") ?? GetPropValue(target, "gameObject") as GameObject;
        if(go == null)
        {
            go = NTweenContainer.genericContainer.gameObject;
        }
        var ntContainer = go.GetComponent<NTweenContainer>() ?? go.AddComponent<NTweenContainer>();
        var nt = new NTween(target, duration, vars,ntContainer);
        if(!nt.gc)
            ntContainer.Add(nt);
        return nt;
    }
	public static NTween to(object target, float duration,Hashtable vars)
	{
		return NewNTween(target, duration, vars);
	}
    public static NTween to(object target, float duration, params object[] var_list)
    {
        return to(target, duration, Hash(var_list));
    }
    public static NTween to(object target, float duration, NTweenParam param)
    {
        return to(target, duration, param.getParam());
    }

	public static NTween from(object target,float duration,Hashtable vars) {
		vars["__from__"] = true;
		if (!vars.ContainsKey("renderNow")) {
			vars["renderNow"] = true;
		}
        return NewNTween(target, duration, vars);
	}
    public static NTween from(object target,float duration,params object[] var_list)
    {
        return @from(target, duration, Hash(var_list));
    }
    public static NTween from(object target, float duration, NTweenParam param)
    {
        return @from(target, duration, param.getParam());
    }
		
    public static NTween fromTo(object target, float duration, Hashtable fromVars, Hashtable toVars) 
    {
        toVars["__fromVars__"] = fromVars;
        if (fromVars["renderNow"] != null && (bool)fromVars["renderNow"])
        {
			toVars["renderNow"] = true;
		}
        return NewNTween(target, duration, toVars);
	}
    public static NTween by(object target,float duration,params object[] var_list)
    {
        return @by(target, duration, Hash(var_list));
    }
    public static NTween by(object target, float duration, NTweenParam param)
    {
        return @by(target, duration, param.getParam());
    }
    public static NTween by(object target,float duration,Hashtable vars)
    {
        vars["__relative__"] = true;
        return NewNTween(target, duration, vars);
    }
    public static NTween set(object target,params object[] var_list)
    {
        return set(target, Hash(var_list));
    }
    public static NTween set(object target, NTweenParam param)
    {
        return set(target, param.getParam());
    }
    public static NTween set(object target, Hashtable vars)
    {
        vars["renderNow"] = true;
        return to(target, 0, vars);
    }

    public static NTween delayedCall(float delay, NTweenCallback callback, object args = null, bool useFrames = false)
    {
        return NewNTween(callback.Target, 0, Hash("delay", delay, "onComplete", callback, "onCompleteParams", args, "useFrames", useFrames, "overwrite", 0));
	}
    /// <summary>
    /// kill all tweens on a target
    /// </summary>
    /// <param name="target"></param>
    /// <param name="complete">complete the tween</param>
    /// <param name="gameObject">target belong to which gameObject,if target is a Component which has gameObject property,this parameter is not required.</param>
    public static void killTweensOf(object target,bool complete=false,GameObject gameObject=null)
    {
        GameObject go = gameObject ?? null;
        if (go == null)
        {
            go = GetPropValue(target, "gameObject") as GameObject;
        }
        if (go == null)
            go = NTweenContainer.genericContainer.gameObject;
        var nTweenContainer = go.GetComponent<NTweenContainer>();
        if (nTweenContainer)
        {
            nTweenContainer.RemoveByTarget(target,complete);
        }
    }
    /// <summary>
    /// kill all tweens on a host gameObject
    /// </summary>
    /// <param name="gameObject">tweens host</param>
    /// <param name="complete"></param>
    public static void killTweensByHost(GameObject gameObject,bool complete=false)
    {
        var nTweenContainer = gameObject.GetComponent<NTweenContainer>();
        if (nTweenContainer)
        {
            nTweenContainer.RemoveAll(complete);
        }
    }
    public static bool activatePlugins(object[] plugins)
    {
        int i = plugins.Length;
        while (i-- > 0)
        {
            NTweenPlugin instance = (NTweenPlugin)Activator.CreateInstance((Type)plugins[i]);
            NTween.plugins[instance.propName] = plugins[i];
        }
        return true;
    }
    #endregion
}
