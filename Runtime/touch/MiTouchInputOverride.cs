#if UNITY_WEBGL || UNITY_EDITOR
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

using mi;

using Touch = UnityEngine.Touch;

internal class TouchData
{
    public Touch touch;
    public long timeStamp;
}

/**
 * 由于Unity WebGL发布的多点触控存在问题, 导致在小游戏中多点触控存在粘连的情况
 * 所以需要使用XIAOMI提供的的触控接口重新覆盖Unity的BaseInput关于触控方面的接口
 * 通过设置StandaloneInputModule.inputOverride的方式来实现
*/
[RequireComponent(typeof(StandaloneInputModule))]
public class MiTouchInputOverride : BaseInput
{
    /// <summary>当前激活的触控点</summary>
    private readonly List<TouchData> _touches = new List<TouchData>();
    /// <summary>要重载的 StandaloneInputModule 对象</summary>
    private StandaloneInputModule _standaloneInputModule = null;

    private static float? cachedPixelRatio = null;
    private static float? cachedWindowHeight = null;

    public static float PixelRatio
    {
        get
        {
            if (cachedPixelRatio == null)
            {
                mi.SystemInfo systemInfo = MiBridge.Instance.GetSystemInfoSync();
                cachedPixelRatio = systemInfo.pixelRatio;
            }
            return (float)cachedPixelRatio;
        }
    }

    public static float WindowHeight
    {
        get
        {
            if (cachedWindowHeight == null)
            {
                mi.SystemInfo systemInfo = MiBridge.Instance.GetSystemInfoSync();
                cachedWindowHeight = systemInfo.windowHeight;
            }
            return (float)cachedWindowHeight;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _standaloneInputModule = GetComponent<StandaloneInputModule>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        Debug.Log("Enable MiTouchInputOverride");
        InitMiTouchEvents();
        if (_standaloneInputModule)
        {
            _standaloneInputModule.inputOverride = this;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UnregisterMiTouchEvents();
        if (_standaloneInputModule)
        {
            _standaloneInputModule.inputOverride = null;
        }
    }

    private void InitMiTouchEvents()
    {
        RegisterMiTouchEvents();
    }

    private void RegisterMiTouchEvents()
    {
        MiTouch.Instance.OnTouchStart(OnMiTouchStart);
        MiTouch.Instance.OnTouchMove(OnMiTouchMove);
        MiTouch.Instance.OnTouchEnd(OnMiTouchEnd);
        MiTouch.Instance.OnTouchCancel(OnMiTouchCancel);
    }

    private void UnregisterMiTouchEvents()
    {
        MiTouch.Instance.OffTouchStart(OnMiTouchStart);
        MiTouch.Instance.OffTouchMove(OnMiTouchMove);
        MiTouch.Instance.OffTouchEnd(OnMiTouchEnd);
        MiTouch.Instance.OffTouchCancel(OnMiTouchCancel);
    }

    private void OnMiTouchStart(OnTouchListenerResult touchEvent)
    {
        foreach (var miTouch in touchEvent.changedTouches)
        {
            Vector2 pos = new Vector2(miTouch.clientX, miTouch.clientY);
            pos = FixTouchPos(pos);

            var data = FindOrCreateTouchData(miTouch.identifier);
            data.touch.phase = TouchPhase.Began;
            data.touch.position = pos;
            data.touch.rawPosition = data.touch.position;
            data.timeStamp = touchEvent.timeStamp;

            // Debug.Log($"OnMiTouchStart:{miTouch.identifier}, {data.touch.phase}");
        }
    }

    private void OnMiTouchMove(OnTouchListenerResult touchEvent)
    {
        foreach (var miTouch in touchEvent.changedTouches)
        {
            Vector2 pos = new Vector2(miTouch.clientX, miTouch.clientY);
            pos = FixTouchPos(pos);

            var data = FindOrCreateTouchData(miTouch.identifier);
            UpdateTouchData(data, pos, touchEvent.timeStamp, TouchPhase.Moved);
        }
    }

    private void OnMiTouchEnd(OnTouchListenerResult touchEvent)
    {
        foreach (var miTouch in touchEvent.changedTouches)
        {
            Vector2 pos = new Vector2(miTouch.clientX, miTouch.clientY);
            pos = FixTouchPos(pos);

            TouchData data = FindTouchData(miTouch.identifier);
            if (data == null)
            {
                Debug.LogError($"OnMiTouchEnd, error identifier:{miTouch.identifier}");
                return;
            }

            if (data.touch.phase == TouchPhase.Canceled || data.touch.phase == TouchPhase.Ended)
            {
                Debug.LogWarning($"OnMiTouchEnd, error phase:{miTouch.identifier}, phase:{data.touch.phase}");
            }

            // Debug.Log($"OnMiTouchEnd:{miTouch.identifier}");
            UpdateTouchData(data, pos, touchEvent.timeStamp, TouchPhase.Ended);
        }
    }

    private void OnMiTouchCancel(OnTouchListenerResult touchEvent)
    {
        foreach (var miTouch in touchEvent.changedTouches)
        {
            Vector2 pos = new Vector2(miTouch.clientX, miTouch.clientY);
            pos = FixTouchPos(pos);

            TouchData data = FindTouchData(miTouch.identifier);
            if (data == null)
            {
                Debug.LogError($"OnMiTouchCancel, error identifier:{miTouch.identifier}");
                return;
            }

            if (data.touch.phase == TouchPhase.Canceled || data.touch.phase == TouchPhase.Ended)
            {
                Debug.LogWarning($"OnMiTouchCancel, error phase:{miTouch.identifier}, phase:{data.touch.phase}");
            }

            // Debug.Log($"OnMiTouchCancel:{miTouch.identifier}");
            UpdateTouchData(data, pos, touchEvent.timeStamp, TouchPhase.Canceled);
        }
    }

    private void LateUpdate()
    {
        foreach (var t in _touches)
        {
            if (t.touch.phase == TouchPhase.Began)
            {
                t.touch.phase = TouchPhase.Stationary;
            }
        }

        RemoveEndedTouches();
    }

    private void RemoveEndedTouches()
    {
        if (_touches.Count > 0)
        {
            _touches.RemoveAll(touchData =>
            {
                var touch = touchData.touch;
                return touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled;
            });
        }
    }

    private TouchData FindTouchData(int identifier)
    {
        foreach (var touchData in _touches)
        {
            var touch = touchData.touch;
            if (touch.fingerId == identifier)
            {
                return touchData;
            }
        }

        return null;
    }

    private TouchData FindOrCreateTouchData(int identifier)
    {
        var touchData = FindTouchData(identifier);
        if (touchData != null)
        {
            return touchData;
        }

        var data = new TouchData();
        data.touch.pressure = 1.0f;
        data.touch.maximumPossiblePressure = 1.0f;
        data.touch.type = TouchType.Direct;
        data.touch.tapCount = 1;
        data.touch.fingerId = identifier;
        data.touch.radius = 0;
        data.touch.radiusVariance = 0;
        data.touch.altitudeAngle = 0;
        data.touch.azimuthAngle = 0;
        data.touch.deltaTime = 0;
        _touches.Add(data);
        return data;
    }

    private static void UpdateTouchData(TouchData data, Vector2 pos, long timeStamp, TouchPhase phase)
    {
        data.touch.phase = phase;
        data.touch.deltaPosition = pos - data.touch.position;
        data.touch.position = pos;
        data.touch.deltaTime = (timeStamp - data.timeStamp) / 1000000.0f;
    }

    private static Vector2 FixTouchPos(Vector2 pos)
    {
        pos.x = Mathf.RoundToInt(pos.x * PixelRatio);
        pos.y = Mathf.RoundToInt(WindowHeight * PixelRatio - pos.y * PixelRatio);
        return pos;
    }

    public static void RefreshCache()
    {
        cachedPixelRatio = null;
        cachedWindowHeight = null;
    }

#if !UNITY_EDITOR
    public override bool touchSupported
    {
        get
        {
            return true;
        }
    }
    public override bool mousePresent
    {
        get
        {
            return false;
        }
    }
    public override int touchCount
    {
        get { return _touches.Count; }
    }

    public override Touch GetTouch(int index)
    {
        // Debug.LogError($"GetTouch touchCount:{touchCount}, index:{index}, touch:{_touches[index].touch.fingerId}, {_touches[index].touch.phase}");
        return _touches[index].touch;
    }
#endif
}
#endif