using AOT;
using System;
using UnityEngine;
using System.Runtime.InteropServices; // for DllImport

static class WebGLInputPlugin
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    public static extern void WebGLInputInitEvent(Action<int, string> OnChange, Action<int, string> OnConfirm, Action<int, string> OnComplete);

    [DllImport("__Internal")]
    public static extern void WebGLInputBeginEditing(int inputId, string text, int maxLength, bool multiple, bool confirmHold);

    [DllImport("__Internal")]
    public static extern void WebGLInputEndEditing();
#else
    public static void WebGLInputInitEvent(Action<int, string> OnChange, Action<int, string> OnConfirm, Action<int, string> OnComplete) { }
    public static void WebGLInputBeginEditing(int inputId, string text, int maxLength, bool multiple, bool confirmHold) { }
    public static void WebGLInputEndEditing() { }

#endif
}

public class WebglInputNode : MonoBehaviour
{
    protected static int _id;
    protected static Action<int,int,string> Action_OnText;

    static bool hasInitEvent = false;
    protected virtual void Awake()
    {
        _id = _id + 1;

        if (!hasInitEvent) {
            hasInitEvent = true;
            WebGLInputPlugin.WebGLInputInitEvent(OnValueChange, OnConfirm, OnComplete);
        }

        Action_OnText += OnText;
    }

    [MonoPInvokeCallback(typeof(Action<int,string>))]
    protected static void OnValueChange(int id ,string value)
    {
        //Debug.Log("OnValueChange " + value);
        Action_OnText?.Invoke(id, 1, value);
    }

    [MonoPInvokeCallback(typeof(Action<int, string>))]
    protected static void OnConfirm(int id, string value)
    {
        Action_OnText?.Invoke(id, 2, value);
    }

    [MonoPInvokeCallback(typeof(Action<int, string>))]
    protected static void OnComplete(int id, string value)
    {
        //Debug.Log("OnComplete");
        Action_OnText?.Invoke(id, 3, value);
    }

    protected virtual void OnText(int id, int eventType, string text) { 
    
    }

    protected virtual void OnDestroy()
    {
        Action_OnText -= OnText;
    }
}
