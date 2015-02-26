using UnityEngine;
using System.Collections;

public class NTweenTimeManager : MonoBehaviour
{
    private float _frame = 1;
    private float _time;
    private float _startFrame;
    private float _startTime;
    public float _framePlayHead = 0;
    public float _timePlayHead = 0;
    private float _globalTimeScale = 1.0f;

    private bool _paused;

//    static NTweenTimeManager()
//    {
//        _globalTimeScale = 1.0f;
//        _frame = 1;
//        _time = Time.realtimeSinceStartup;
//        _startFrame = 0;
//        _startTime = _time;
//        _framePlayHead = _frame - _startFrame;
//        _timePlayHead = 0;
//        _paused = false;
//    }

    private static NTweenTimeManager instance;

    public static NTweenTimeManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(NTweenTimeManager)) as NTweenTimeManager;
                if (instance == null)
                {
                    GameObject go = new GameObject("NTweenTimeManager");
                    instance = go.AddComponent<NTweenTimeManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }

    public bool paused
    {
        get { return _paused; }
        set { _paused = value;}
    }
    public float globalTimeScale
    {
        get { return _globalTimeScale; }
        set
        {
            if (value != 0)
            {
                _startTime = _time - _timePlayHead / value;
                _startFrame = _frame - _framePlayHead / value;
            }
            _globalTimeScale = value;
        }
    }

    void Update()
    {
        if (!(_globalTimeScale == 0 || _paused))
        {
            UpdateTime(Time.deltaTime);
        }
    }
    public void UpdateTime(float dt)
    {
        _frame += 1;
        _time += dt;
        _framePlayHead = (_frame - _startFrame) * _globalTimeScale;
        _timePlayHead = (_time - _startTime) * _globalTimeScale;
    }
}