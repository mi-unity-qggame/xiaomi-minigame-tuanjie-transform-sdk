using System;
using UnityEngine;
using AOT;
using System.Runtime.InteropServices; // for DllImport
using mi;

public class MiGetOptions : MonoBehaviour
{
    #region Plugin

    [DllImport("__Internal")]
    public static extern void MiGetOptionsInitEvent(int optionsId, Action<int, string> onShow, Action<int> onHide);
    [DllImport("__Internal")]
    public static extern void MiGetOptionsOnShow();
    [DllImport("__Internal")]
    public static extern void MiGetOptionsOnHide();
    [DllImport("__Internal")]
    public static extern void MiGetOptionsOffShow();
    [DllImport("__Internal")]
    public static extern void MiGetOptionsOffHide();
    [DllImport("__Internal")]
    private static extern string QGGetLaunchOptionsSync();

    protected static int _id;
    protected static Action<int, string> Action_OnShow;
    protected static Action<int> Action_OnHide;

    static bool hasInitEvent = false;

    private static MiGetOptions instance = null;

    public static MiGetOptions Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject(typeof(MiGetOptions).Name).AddComponent<MiGetOptions>();
                DontDestroyOnLoad(instance.gameObject);
            }

            return instance;
        }
    }

    protected virtual void Awake()
    {
        _id = GetInstanceID(); // 使用唯一的实例 ID 作为 inputId

        if (!hasInitEvent)
        {
            hasInitEvent = true;
            MiGetOptionsInitEvent(_id, onShow, onHide);
        }

        Action_OnShow += OnOptions;
        Action_OnHide += OnHideOptions;
    }

    [MonoPInvokeCallback(typeof(Action<int, string>))]
    protected static void onShow(int id, string value)
    {
        // Debug.Log("OnShow " + value);
        Action_OnShow?.Invoke(id, value);
    }

    [MonoPInvokeCallback(typeof(Action<int>))]
    protected static void onHide(int id)
    {
        // Debug.Log("OnHide ");
        Action_OnHide?.Invoke(id);
    }

    protected void OnOptions(int id, string options)
    {
    }

    protected void OnHideOptions(int id)
    {
    }

    private void OnDestroy()
    {
        Action_OnShow -= OnOptions;
        Action_OnHide -= OnHideOptions;
        OffShow();
        OffHide();
    }

    #endregion

    /// <summary>
    /// 监听游戏切入前台事件
    /// </summary>
    /// <param name="onShow"></param>
    /// <returns></returns>
    public string OnShow(Action<string> onShow = null)
    {
        MiGetOptionsOnShow();

        Action_OnShow += (id, options) =>
        {
            onShow(options);
        };
        return _id.ToString();
    }

    /// <summary>
    /// 监听游戏切入后台事件
    /// </summary>
    /// <param name="onHide"></param>
    /// <returns></returns>
    public string OnHide(Action onHide = null)
    {
        MiGetOptionsOnHide();

        Action_OnHide += (id) =>
        { 
            onHide();
        };
        return _id.ToString();
    }

    /// <summary>
    /// 取消监听游戏切入前台
    /// </summary>
    /// <param name="callback"></param>
    public void OffShow(Action callback = null)
    {
        if (callback == null)
        {
            // 如果不传参数，则移除所有监听函数
            MiGetOptionsOffShow();
            Action_OnShow = null;
        }
        else
        {
            // 如果传入了参数，则移除指定的监听函数
            Action_OnShow -= (id, options) => callback?.Invoke();
        }
    }

    /// <summary>
    /// 取消监听游戏切入后台事件
    /// </summary>
    /// <param name="callback"></param>
    public void OffHide(Action callback = null)
    {
        if (callback == null)
        {
            // 如果不传参数，则移除所有监听函数
            MiGetOptionsOffHide();
            Action_OnHide = null;
        }
        else
        {
            // 如果传入了参数，则移除指定的监听函数
            Action_OnHide -= (id) => callback?.Invoke();
        }
    }

    /// <summary>
    /// 获取快游戏启动时的参数
    /// </summary>
    public QGLaunchInfo GetLaunchOptionsSync()
    {
        string msg = QGGetLaunchOptionsSync();
        try
        {
            QGLaunchInfo launchInfo = JsonUtility.FromJson<QGLaunchInfo>(msg);
            MiBridge.Instance.QGLog("QGLaunchInfo: " + JsonUtility.ToJson(launchInfo));
            return launchInfo;
        }
        catch (Exception e)
        {
            MiBridge.Instance.QGLog("GetLaunchOptionsSync error = {0}", e.ToString());
            return new QGLaunchInfo();
        }
    }
}