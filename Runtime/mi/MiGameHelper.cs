using UnityEngine;


/// <summary>
/// MiGameHelper 游戏辅助脚本，如果需要请把脚本挂载到游戏场景当中，可以用来精准控制自定义loading销毁时间
/// </summary>
public class MiGameHelper : MonoBehaviour
{
    private void Awake()
    {
        HideCustomLoading();
    }

    /// <summary>
    /// //隐藏自定义loading页面，可以准确控制自定义loading的隐藏时机，强烈建议使用！！！
    /// </summary>
    public void HideCustomLoading()
    {
        MiBridge.Instance.QGLog("auto mi game hide loading");
        MiBridge.Instance.HideLoading();
    }
}
