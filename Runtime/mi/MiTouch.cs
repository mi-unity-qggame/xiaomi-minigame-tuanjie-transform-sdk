using System;
using UnityEngine;
using System.Runtime.InteropServices;
using mi;
using AOT;

#region Plugin
public static class MiTouchPlugin
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    public static extern void MiTouchInitEvent(Action<string> onTouchStart,Action<string> onTouchMove,Action<string> onTouchEnd,Action<string> onTouchCancel);

    [DllImport("__Internal")]
    public static extern void MiTouchRemoveAll();
#else
    public static void MiTouchInitEvent(Action<string> onTouchStart, Action<string> onTouchMove, Action<string> onTouchEnd, Action<string> onTouchCancel) { }
    public static void MiTouchRemoveAll() { }
#endif
}
#endregion

public class MiTouch : MonoBehaviour
{
    private static MiTouch instance = null;

    public static MiTouch Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject(typeof(MiTouch).Name).AddComponent<MiTouch>();
                DontDestroyOnLoad(instance.gameObject);
            }

            return instance;
        }
    }

    private event Action<OnTouchListenerResult> _onTouchStart;
    private event Action<OnTouchListenerResult> _onTouchMove;
    private event Action<OnTouchListenerResult> _onTouchEnd;
    private event Action<OnTouchListenerResult> _onTouchCancel;

    private void Awake()
    {
        // 注册 JS 监听
        MiTouchPlugin.MiTouchInitEvent(OnTouchStartCallback, OnTouchMoveCallback, OnTouchEndCallback, OnTouchCancelCallback);
    }

    #region Callbacks
    [MonoPInvokeCallback(typeof(Action<string>))]
    private static void OnTouchStartCallback(string json)
    {
        Instance._onTouchStart?.Invoke(JsonUtility.FromJson<OnTouchListenerResult>(json));
    }

    [MonoPInvokeCallback(typeof(Action<string>))]
    private static void OnTouchMoveCallback(string json)
    {
        Instance._onTouchMove?.Invoke(JsonUtility.FromJson<OnTouchListenerResult>(json));
    }

    [MonoPInvokeCallback(typeof(Action<string>))]
    private static void OnTouchEndCallback(string json)
    {
        Instance._onTouchEnd?.Invoke(JsonUtility.FromJson<OnTouchListenerResult>(json));
    }

    [MonoPInvokeCallback(typeof(Action<string>))]
    private static void OnTouchCancelCallback(string json)
    {
        Instance._onTouchCancel?.Invoke(JsonUtility.FromJson<OnTouchListenerResult>(json));
    }
#endregion

    /// <summary>
    /// 监听开始触摸事件
    /// </summary>
    /// <param name="onTouchStartResult"></param>
    public void OnTouchStart(Action<OnTouchListenerResult> onTouchStartResult)
    {
       _onTouchStart += onTouchStartResult;
    }

    /// <summary>
    /// 取消监听开始触摸事件
    /// </summary>
    /// <param name="onTouchStartResult"></param>
    public void OffTouchStart(Action<OnTouchListenerResult> onTouchStartResult = null)
    {
        if(_onTouchStart != null)
        {
            _onTouchStart -= onTouchStartResult;
        }
        else
        {
            _onTouchStart =  null;
        }
    }

    /// <summary>
    /// 监听触点移动事件
    /// </summary>
    /// <param name="onTouchMoveResult"></param>
    public void OnTouchMove(Action<OnTouchListenerResult> onTouchMoveResult)
    {
        _onTouchMove += onTouchMoveResult;
    }

    /// <summary>
    /// 取消监听触点移动事件
    /// </summary>
    /// <param name="onTouchMoveResult"></param>
    public void OffTouchMove(Action<OnTouchListenerResult> onTouchMoveResult = null)
    {
        if (_onTouchMove != null)
        {
            _onTouchMove -= onTouchMoveResult;
        }
        else
        {
            _onTouchMove = null;
        }
    }

    /// <summary>
    /// 监听触点失效事件
    /// </summary>
    /// <param name="onTouchCancelResult"></param>
    public void OnTouchCancel(Action<OnTouchListenerResult> onTouchCancelResult)
    {
       _onTouchCancel += onTouchCancelResult;
    }

    /// <summary>
    /// 取消监听触点失效事件
    /// </summary>
    /// <param name="onTouchCancelResult"></param>
    public void OffTouchCancel(Action<OnTouchListenerResult> onTouchCancelResult = null)
    {
        if (_onTouchCancel != null)
        {
            _onTouchCancel -= onTouchCancelResult;
        }
        else
        {
            _onTouchCancel = null;
        }
    }

    /// <summary>
    /// 监听触摸结束事件
    /// </summary>
    /// <param name="onTouchEndResult"></param>
    public void OnTouchEnd(Action<OnTouchListenerResult> onTouchEndResult)
    {
       _onTouchEnd += onTouchEndResult;
    }

    /// <summary>
    /// 取消监听触摸结束事件
    /// </summary>
    /// <param name="onTouchEndResult"></param>
    public void OffTouchEnd(Action<OnTouchListenerResult> onTouchEndResult = null)
    {
        if (_onTouchEnd != null)
        {
            _onTouchEnd -= onTouchEndResult;
        }
        else
        {
            _onTouchEnd = null;
        }
    }

    private void OnDestroy()
    {
        MiTouchPlugin.MiTouchRemoveAll();
        _onTouchStart = null;
        _onTouchMove = null;
        _onTouchEnd = null;
        _onTouchCancel = null;
    }
}