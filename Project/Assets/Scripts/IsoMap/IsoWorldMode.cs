using System;
using TouchScript;
using TouchScript.Gestures;
using TouchScript.Gestures.Simple;
using UnityEngine;
using System.Collections;
using Object = UnityEngine.Object;

public abstract class IsoWorldMode {
    private bool isRegisteredEvents = false;
    private tk2dCamera mainCamera;
    private Vector3 lastPanDelta;
    private bool allowZoom;

    protected IsoWorldMode()
    {
        mainCamera = Camera.main.GetComponent<tk2dCamera>();
    }
    public void EnterMode()
    {
        RegisterTouch();
        OnEnter();
    }

    protected virtual void OnEnter()
    {   
        //重置相机
        mainCamera.ZoomFactor = 0.4f;
        mainCamera.transform.localPosition = new Vector3(0,40,-10);
    }

    public void ExitMode()
    {
        UnregisterTouch();
        OnExit();
    }

    protected virtual void OnExit()
    {
    }

    public virtual void Update(float dt)
    {
        //  按下状态不做处理
        var n = TouchManager.Instance.NumberOfTouches;
        if (n == 0)
        {
            if (lastPanDelta.sqrMagnitude > 1)
            {
                lastPanDelta *= 0.8f;
                mainCamera.transform.position -= lastPanDelta;
            }
        }
        if (n < 2)
        {
            if (mainCamera.ZoomFactor > Constants.ADJUST_CAMERA_ZOOM)
            {
                mainCamera.ZoomFactor = Mathf.Lerp(mainCamera.ZoomFactor, Constants.ADJUST_CAMERA_ZOOM, dt * Constants.ADJUST_CAMERA_SPEED);
            }
        }
    }

    protected void RegisterTouch()
    {
        if (!isRegisteredEvents)
        {
            isRegisteredEvents = true;

            var scaleGesture = GameObject.FindObjectOfType<SimpleScaleGesture>();
            scaleGesture.Scaled += OnScaleGesture;
            scaleGesture.ScaleStarted += OnScaleGestureStarted;

            var panGesture = GameObject.FindObjectOfType<SimplePanGesture>();
            panGesture.Panned += OnPanGesture;

            var tapGesture = GameObject.FindObjectOfType<TapGesture>();
            tapGesture.Tapped += OnTapGesture;

            var pressGesture = GameObject.FindObjectOfType<PressGesture>();
            pressGesture.Pressed += OnPressGesture;

            var releaseGesture = GameObject.FindObjectOfType<ReleaseGesture>();
            releaseGesture.Released += OnReleaseGesture;

            var longPressGesture = GameObject.FindObjectOfType<LongPressGesture>();
            longPressGesture.LongPressed += OnLongPressGesture;
        }
    }

    protected void UnregisterTouch()
    {
        if (isRegisteredEvents)
        {
            isRegisteredEvents = false;

            var scaleGesture = Object.FindObjectOfType<SimpleScaleGesture>();
            scaleGesture.Scaled -= OnScaleGesture;
            scaleGesture.ScaleStarted -= OnScaleGestureStarted;


            var panGesture = Object.FindObjectOfType<SimplePanGesture>();
            panGesture.Panned -= OnPanGesture;

            var tapGesture = Object.FindObjectOfType<TapGesture>();
            tapGesture.Tapped -= OnTapGesture;

            var pressGesture = Object.FindObjectOfType<PressGesture>();
            pressGesture.Pressed -= OnPressGesture;

            var releaseGesture = Object.FindObjectOfType<ReleaseGesture>();
            releaseGesture.Released -= OnReleaseGesture;

            var longPressGesture = Object.FindObjectOfType<LongPressGesture>();
            longPressGesture.LongPressed -= OnLongPressGesture;
        }
    }

    virtual protected void OnPress(Vector2 screenPosition)
    {
        lastPanDelta = Vector3.zero;
    }

    virtual protected void OnLongPress(Vector2 screenPosition) { }

    virtual protected void OnTap(Vector2 screenPosition)
    {
        
    }

    virtual protected void OnPan(Vector2 screenPosition, Vector3 deltaPosition)
    {
        lastPanDelta = deltaPosition;
        mainCamera.transform.position -= deltaPosition;
    }
    /// <summary>
    ///return true to allowZoom.
    /// </summary>
    virtual protected bool OnZoomStart() { return true;}
    virtual protected void OnRelease() { }
    private void OnScaleGestureStarted(object sender, EventArgs e)
    {
        allowZoom = OnZoomStart();
    }

    private void OnReleaseGesture(object sender, EventArgs e)
    {
        OnRelease();
    }

    private void OnLongPressGesture(object sender, EventArgs e)
    {
        OnLongPress(((LongPressGesture) sender).ScreenPosition);
    }

    private void OnPressGesture(object sender,EventArgs e)
    {
        OnPress(((PressGesture) sender).ScreenPosition);
    }

    private void OnTapGesture(object sender,EventArgs e)
    {
        OnTap(((Gesture) sender).ScreenPosition);
    }

    private void OnPanGesture(object sender, EventArgs e)
    {
        OnPan(((SimplePanGesture) sender).ScreenPosition, ((SimplePanGesture) sender).WorldDeltaPosition);
    }

    private void OnScaleGesture(object sender, EventArgs e)
    {
    	if(allowZoom)
        	mainCamera.ZoomFactor = Mathf.Clamp(mainCamera.ZoomFactor * ((SimpleScaleGesture) sender).LocalDeltaScale, Constants.MIN_CAMERA_ZOOM, Constants.MAX_CAMERA_ZOOM);
//        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize * (2 - (sender as SimpleScaleGesture).LocalDeltaScale), 2, 24);
    }
}
