using System;
using UnityEngine;
using mi;

public class MiKeyBoard : WebglInputNode
{
    private static MiKeyBoard instance = null;

    public static MiKeyBoard Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject(typeof(MiKeyBoard).Name).AddComponent<MiKeyBoard>();
                DontDestroyOnLoad(instance.gameObject);
            }

            return instance;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _id = GetInstanceID(); // 使用唯一的实例 ID 作为 inputId
    }

    /// <summary>
    /// 显示键盘
    /// </summary>
    public void ShowKeyboard(KeyboardParam param)
    {
        WebGLInputPlugin.WebGLInputBeginEditing(_id, param.defaultValue, param.maxLength, param.multiple, param.confirmHold);
    }

    /// <summary>
    /// 监听键盘输入
    /// </summary>
    /// <param name="onChange"></param>
    /// <returns></returns>
    public string OnKeyboardInput(Action<string> onChange = null)
    {
        Action_OnText += (id, eventType, text) =>
        {
            if (eventType == 1)
            {
                onChange(text);
            }
        };
        return _id.ToString();
    }

    /// <summary>
    /// 监听用户点击键盘 Confirm 按钮
    /// </summary>
    /// <param name="confirmCallback"></param>
    /// <returns></returns>
    public string OnKeyboardConfirm(Action<string> confirmCallback = null)
    {
        Action_OnText += (id, eventType, text) =>
        {
            if (eventType == 2)
            {
                confirmCallback(text);
            }
        };
        return _id.ToString();
    }

    /// <summary>
    /// 监听键盘收起
    /// </summary>
    /// <param name="completedCallback"></param>
    /// <returns></returns>
    public string OnKeyboardComplete(Action<string> completedCallback = null)
    {
        Action_OnText += (id, eventType, text) =>
        {
            if (eventType == 3)
            {
                completedCallback(text);
            }
        };
        return _id.ToString();
    }

    /// <summary>
    /// 取消监听键盘输入
    /// </summary>
    /// <param name="callback"></param>
    public void OffKeyboardInput(Action callback = null)
    {
        Action_OnText -= (id, eventType, text) => callback?.Invoke();
    }

    /// <summary>
    /// 取消监听用户点击键盘 Confirm 按钮
    /// </summary>
    /// <param name="callback"></param>
    public void OffKeyboardConfirm(Action callback = null)
    {
        Action_OnText -= (id, eventType, text) => callback?.Invoke();
    }

    /// <summary>
    /// 取消监听键盘收起
    /// </summary>
    /// <param name="callback"></param>
    public void OffKeyboardComplete(Action callback = null)
    {
        Action_OnText -= (id, eventType, text) => callback?.Invoke();
    }

    /// <summary>
    /// 隐藏键盘
    /// </summary>
    public void HideKeyboard()
    {
        WebGLInputPlugin.WebGLInputEndEditing();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        OffKeyboardInput();
        OffKeyboardConfirm();
        OffKeyboardComplete();
    }
}