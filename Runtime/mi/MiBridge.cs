using mi;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

public class MiBridge : MonoBehaviour
{
    private static MiBridge instance = null;
    public static MiBridge Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject(typeof(MiBridge).Name).AddComponent<MiBridge>();
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }

    #region js function

    [DllImport("__Internal")]
    private static extern void QGInit();

    [DllImport("__Internal")]
    private static extern void QGPrint(string info);
	
	[DllImport("__Internal")]
    private static extern void QGPrintInt(int num);

    [DllImport("__Internal")]
    private static extern void QGLogin(int cmd);

    [DllImport("__Internal")]
    private static extern void QGGetUserInfo(int cmd);

    [DllImport("__Internal")]
    private static extern void QGOnUserInfoChanged(int cmd);

    [DllImport("__Internal")]
    private static extern void QGPay(int cmd, string info);

    [DllImport("__Internal")]
    private static extern void QGHasInstalled(int cmd);

    [DllImport("__Internal")]
    private static extern void QGInstall(int cmd, string msg);

    [DllImport("__Internal")]
    private static extern void QGAccessFile(int cmd, string fileName);
    [DllImport("__Internal")]
    private static extern bool QGAccessFileSync(string fileName);
    [DllImport("__Internal")]
    private static extern void QGAppendFile(int cmd, string fileName, string encoding, string textStr, byte[] textData, int length);
    [DllImport("__Internal")]
    private static extern void QGAppendFileSync(string fileName, string encoding, string textStr, byte[] textData, int length);
    [DllImport("__Internal")]
    private static extern bool QGCopyFile(int cmd, string srcPath, string destPath);
    [DllImport("__Internal")]
    private static extern bool QGCopyFileSync(string srcPath, string destPath);
    [DllImport("__Internal")]
    private static extern void QGMkDir(int cmd, string filePath);
    [DllImport("__Internal")]
    private static extern void QGMkDirSync(string filePath, bool recursive);
    [DllImport("__Internal")]
    private static extern void QGDeleteDir(int cmd, string filePath, bool recursive);
    [DllImport("__Internal")]
    private static extern void QGDeleteDirSync(string filePath, bool recursive);
    [DllImport("__Internal")]
    private static extern void QGReadFile(int cmd, string fileName, string encoding);
    [DllImport("__Internal")]
    private static extern string QGReadFileSync(string fileName, string encoding);
    [DllImport("__Internal")]
    private static extern void QGGetFileBuffer(byte[] buffer, string callBackId);
    [DllImport("__Internal")]
    private static extern void QGWriteFile(int cmd, string fileName, string encoding, bool append, string textStr, byte[] textData, int length);
    [DllImport("__Internal")]
    private static extern bool QGWriteFileSync(string fileName, string encoding, bool append, string textStr, byte[] textData, int length);
    [DllImport("__Internal")]
    private static extern void QGStat(int cmd, string path);
    [DllImport("__Internal")]
    private static extern string QGStatSync(string path, bool recursive);
    [DllImport("__Internal")]
    private static extern void QGDeleteFile(int cmd, string fileName);
    [DllImport("__Internal")]
    private static extern void QGDeleteFileSync(string fileName);
    [DllImport("__Internal")]
    private static extern void QGSetItem(string key, string value);
    [DllImport("__Internal")]
    private static extern string QGGetItem(string key);
    [DllImport("__Internal")]
    private static extern void QGDeleteItem(string key);
    [DllImport("__Internal")]
    private static extern bool QGCreateBannerAd(int cmd, int left, int top, int width, int height, string adId);
    [DllImport("__Internal")]
    private static extern bool QGCreateInterstitialAd(int cmd, string adId);
    [DllImport("__Internal")]
    private static extern bool QGCreateCustomAd(int cmd, int left, int top, int width, int height, string adId);
    [DllImport("__Internal")]
    private static extern bool QGCreateRewardedVideoAd(int cmd, string adId);
    [DllImport("__Internal")]
    private static extern void QGLoadAd(int cmd, string adId);
    [DllImport("__Internal")]
    private static extern void QGShowAd(int cmd, string adId);
    [DllImport("__Internal")]
    private static extern void QGHideAd(int cmd, string adId);
    [DllImport("__Internal")]
    private static extern void QGDestroyAd(int cmd, string adId);
    [DllImport("__Internal")]
    private static extern void QGShowRecommendAd(int cmd, string adId, int type);
    [DllImport("__Internal")]
    private static extern void QGCloseRecommendAd(int cmd, string adId, int style);
    [DllImport("__Internal")]
    private static extern void QGHttpPost(int cmd, int timeout, string url, string headers, string body);
    [DllImport("__Internal")]
    private static extern void QGHttpGet(int cmd, int timeout, string url, string headers, string param);
    [DllImport("__Internal")]
    private static extern void QGExitApp(int cmd);
    [DllImport("__Internal")]
    private static extern void QGSetClipboardData(int cmd, string clipData);
    [DllImport("__Internal")]
    private static extern void QGGetClipboardData(int cmd);
    [DllImport("__Internal")]
    private static extern void QGGetSystemInfo(int cmd);
    [DllImport("__Internal")]
    private static extern string QGGetSystemInfoSync();
    [DllImport("__Internal")]
    private static extern string QGGetMenuButtonBoundingClientRect();
    [DllImport("__Internal")]
    private static extern void QGSetPreferredFramesPerSecond(int fps);
    [DllImport("__Internal")]
    private static extern void QGClearAllKV();
    [DllImport("__Internal")]
    private static extern void QGHideLoading();
    [DllImport("__Internal")]
    private static extern void QGUpdateGame();

    #endregion

    #region private members

#pragma warning disable CS0414
    private volatile bool mInited = false;
    private volatile int mRequestId = 0;
#pragma warning restore CS0414
    private volatile bool mLogOpen = true;

    private Dictionary<int, List<QGCallBackData>> mWaitIdDic = new Dictionary<int, List<QGCallBackData>>();
    private UnityEngine.Object mWaitIdDicLock = new UnityEngine.Object();

    private Dictionary<int, LoginCallback> mLoginDic = new Dictionary<int, LoginCallback>();
    private Dictionary<int, GetUserInfoCallback> mGetUserInfoDic = new Dictionary<int, GetUserInfoCallback>();
    private Dictionary<int, OnUserInfoCallback> mOnUserInfoDic = new Dictionary<int, OnUserInfoCallback>();
    private Dictionary<int, PayListener> mPayDic = new Dictionary<int, PayListener>();
    private Dictionary<int, HasInstalledListener> mHasInstalledDic = new Dictionary<int, HasInstalledListener>();
    private Dictionary<int, CreateShortcutListener> mInstalledDic = new Dictionary<int, CreateShortcutListener>();
    private Dictionary<int, FileExistsListener> mFileExistsDic = new Dictionary<int, FileExistsListener>();
    private Dictionary<int, AppendFileListener> mAppendFileDic = new Dictionary<int, AppendFileListener>();
    private Dictionary<int, CopyFileListener> mCopyFileDic = new Dictionary<int, CopyFileListener>();
    private Dictionary<int, MkDirListener> mMkDirDic = new Dictionary<int, MkDirListener>();
    private Dictionary<int, ReadFileCallbackOld> mReadFileDicOld = new Dictionary<int, ReadFileCallbackOld>();
    private Dictionary<int, ReadFileCallback> mReadFileDic = new Dictionary<int, ReadFileCallback>();
    private Dictionary<int, WriteFileListener> mWriteFileDic = new Dictionary<int, WriteFileListener>();
    private Dictionary<int, StatListener> mStatDic = new Dictionary<int, StatListener>();
    private Dictionary<int, DeleteFileListener> mDeleteFileDic = new Dictionary<int, DeleteFileListener>();
    private Dictionary<int, AdEventListener> mAdDic = new Dictionary<int, AdEventListener>();
    private Dictionary<int, ShowRecommendAdListener> mRecommendAdDic = new Dictionary<int, ShowRecommendAdListener>();
    private Dictionary<int, CloseRecommendAdListener> mCloseRecommendAdDic = new Dictionary<int, CloseRecommendAdListener>();
    private Dictionary<int, HttpListener> mHttpDic = new Dictionary<int, HttpListener>();
    private Dictionary<int, ExitListener> mExitDic = new Dictionary<int, ExitListener>();
    private Dictionary<int, SetClipBoardListener> mSetClipBoardDic = new Dictionary<int, SetClipBoardListener>();
    private Dictionary<int, GetClipBoardListener> mGetClipBoardDic = new Dictionary<int, GetClipBoardListener>();
    private Dictionary<int, GetSystemInfoListener> mGetSystemInfoDic = new Dictionary<int, GetSystemInfoListener>();

    #endregion

    void Update()
    { 
        if (mWaitIdDic.Count > 0) 
        {
            lock (mWaitIdDicLock)
            {
                foreach (var wait in mWaitIdDic)
                {
                    foreach (var data in wait.Value) {
                        var id = wait.Key;
                        var value = data;
                        QGLog("unity update handle msg id={0}, type={1}", id, value.type);
                        try
                        {
                            switch (value.type)
                            {
                                case QGCallBackType.Login:
                                    LoginHandler(id, value);
                                    break;
                                case QGCallBackType.GetUserInfo:
                                    GetUserInfoHandler(id, value);
                                    break;
                                case QGCallBackType.OnUserInfo:
                                    OnUserInfoHandler(id, value);
                                    break;
                                case QGCallBackType.Pay:
                                    PayHandler(id, value);
                                    break;
                                case QGCallBackType.HasInstalled:
                                    HasInstaledHandler(id, value);
                                    break;
                                case QGCallBackType.Installed:
                                    InstalledHandler(id, value);
                                    break;
                                case QGCallBackType.AccessFile:
                                    FileExistsedHandler(id, value);
                                    break;
                                case QGCallBackType.AppendFile:
                                    AppendFiledHandler(id, value);
                                    break;
                                case QGCallBackType.CopyFile:
                                    CopyFileHandler(id, value);
                                    break;
                                case QGCallBackType.MkDir:
                                    MkDirHandler(id, value);
                                    break;
                                case QGCallBackType.ReadFileOld:
                                    ReadFileHandlerOld(id, value);
                                    break;
                                case QGCallBackType.ReadFile:
                                    ReadFileHandler(id, value);
                                    break;
                                case QGCallBackType.WriteFile:
                                    WriteFileHandler(id, value);
                                    break;
                                case QGCallBackType.FileStat:
                                    StatHandler(id, value);
                                    break;
                                case QGCallBackType.DeleteFile:
                                    DeleteFileHandler(id, value);
                                    break;
                                case QGCallBackType.Ad:
                                    AdHandler(id, value);
                                    break;
                                case QGCallBackType.Recommend:
                                    ShowRecommedAdHandler(id, value);
                                    break;
                                case QGCallBackType.RecommendClose:
                                    CloseRecommedAdHandler(id, value);
                                    break;
                                case QGCallBackType.Http:
                                    HttpHandler(id, value);
                                    break;
                                case QGCallBackType.ExitApp:
                                    ExitAppHandler(id, value);
                                    break;
                                case QGCallBackType.GetClip:
                                    GetClipHandler(id, value);
                                    break;
                                case QGCallBackType.SetClip:
                                    SetClipHandler(id, value);
                                    break;
                                case QGCallBackType.GetSystemInfo:
                                    GetSystemInfoHandler(id, value);
                                    break;
                            }
                        }
                        catch (Exception e)
                        {
                            QGLog("handler error {0}", e.ToString());
                        }
                    }

                    wait.Value.Clear();
                    mWaitIdDic.Remove(wait.Key);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 登录信息处理
    /// </summary>
    /// <param name="id"></param>
    /// <param name="qGCallBackData"></param>
    private void LoginHandler(int id, QGCallBackData qGCallBackData)
    {
        QGLog("LoginHandler {0}", qGCallBackData.data);
        if (mLoginDic.ContainsKey(id))
        {
            LoginCallback loginCallback = mLoginDic[id];
            if (qGCallBackData.ret == 0)
            {
                var qgAccout = JsonUtility.FromJson<QGAccoutInfo>(qGCallBackData.data);
                loginCallback.success?.Invoke(qgAccout.appAccountId, qgAccout.session);
            }
            else
            {
                loginCallback.failure?.Invoke(qGCallBackData.ret, qGCallBackData.msg);
            }
            mLoginDic.Remove(id);
        }
    }

    /// <summary>
    /// 获取用户信息处理
    /// </summary>
    /// <param name="id"></param>
    /// <param name="qGCallBackData"></param>
    private void GetUserInfoHandler(int id, QGCallBackData qGCallBackData)
    {
        QGLog("GetUserInfoHandler {0}", qGCallBackData.data);
        if (mGetUserInfoDic.ContainsKey(id))
        {
            GetUserInfoCallback getUserInfoCallback = mGetUserInfoDic[id];
            if (qGCallBackData.ret == 0)
            {
                var qGUserInfo = JsonUtility.FromJson<QGUserInfo>(qGCallBackData.data);
                QGLog("GetUserInfo callback {0}, {1}, {2}", qGUserInfo.userInfo.nickName, qGUserInfo.userInfo.avatarUrl, qGUserInfo.userInfo.gender);
                getUserInfoCallback.success?.Invoke(qGUserInfo.userInfo.nickName, qGUserInfo.userInfo.avatarUrl, qGUserInfo.userInfo.gender);
            }
            else
            {
                getUserInfoCallback.failure?.Invoke(qGCallBackData.ret, qGCallBackData.msg);
            }
            mGetUserInfoDic.Remove(id);
        }
    }

    /// <summary>
    /// 用户信息改变处理
    /// </summary>
    /// <param name="id"></param>
    /// <param name="qGCallBackData"></param>
    private void OnUserInfoHandler(int id, QGCallBackData qGCallBackData)
    {
        QGLog("OnUserInfoHandler {0}", qGCallBackData.data);
        if (mOnUserInfoDic.ContainsKey(id))
        {
            GetUserInfoCallback getUserInfoCallback = mGetUserInfoDic[id];
            if (qGCallBackData.ret == 0)
            {
                var qGUserInfo = JsonUtility.FromJson<QGUserInfo>(qGCallBackData.data);
                getUserInfoCallback.success?.Invoke(qGUserInfo.userInfo.nickName, qGUserInfo.userInfo.avatarUrl, qGUserInfo.userInfo.gender);
            }
            else
            {
                getUserInfoCallback.failure?.Invoke(qGCallBackData.ret, qGCallBackData.msg);
            }
            //id不需要删除，持久化监听
            /*mGetUserInfoDic.Remove(id);*/
        }
    }

    /// <summary>
    /// 支付处理
    /// </summary>
    /// <param name="id"></param>
    /// <param name="qGCallBackData"></param>
    private void PayHandler(int id, QGCallBackData qGCallBackData)
    {
        QGLog("PayHandler {0}", qGCallBackData.data);
        if (mPayDic.ContainsKey(id))
        {
            PayListener listener = mPayDic[id];
            var qGUserInfo = JsonUtility.FromJson<QGPayInfo>(qGCallBackData.data);
            if (qGUserInfo.resultStatus == "9000")
            {
                listener?.Invoke(true, 0, "");
            }
            else
            {
                try
                {
                    listener?.Invoke(false, Int32.Parse(qGUserInfo.resultStatus), qGUserInfo.memo);
                }
                catch (Exception)
                {
                    listener?.Invoke(false, -1, "订单返回信息解析失败");
                }
            }
            mPayDic.Remove(id);
        }
    }

    /// <summary>
    /// 检查是否有桌面图标处理
    /// </summary>
    /// <param name="id"></param>
    /// <param name="qGCallBackData"></param>
    private void HasInstaledHandler(int id, QGCallBackData qGCallBackData)
    {
        QGLog("HasInstaledHandler {0}", qGCallBackData.data);
        if (mHasInstalledDic.ContainsKey(id))
        {
            var listener = mHasInstalledDic[id];
            listener?.Invoke(qGCallBackData.ret == 0);
            mHasInstalledDic.Remove(id);
        }
    }

    /// <summary>
    /// 创建桌面图标处理
    /// </summary>
    /// <param name="id"></param>
    /// <param name="qGCallBackData"></param>
    private void InstalledHandler(int id, QGCallBackData qGCallBackData)
    {
        QGLog("InstalledHandler {0}", qGCallBackData.data);
        if (mInstalledDic.ContainsKey(id))
        {
            var listener = mInstalledDic[id];

            if (qGCallBackData.ret == 0)
            {
                listener?.Invoke(qGCallBackData.ret == 0, 0, "");
            }
            else
            {
                listener?.Invoke(qGCallBackData.ret == 0, qGCallBackData.ret, qGCallBackData.msg);
            }
            mInstalledDic.Remove(id);
        }
    }

    /// <summary>
    /// 文件是否存在
    /// </summary>
    /// <param name="id"></param>
    /// <param name="qGCallBackData"></param>
    private void FileExistsedHandler(int id, QGCallBackData qGCallBackData)
    {
        if (mFileExistsDic.ContainsKey(id))
        {
            var callback = mFileExistsDic[id];
            callback?.Invoke(qGCallBackData.ret == 0);
            mFileExistsDic.Remove(id);
        }
    }

    /// <summary>
    /// 在文件结尾追加内容
    /// </summary>
    private void AppendFiledHandler(int id, QGCallBackData qGCallBackData)
    {
        if (mAppendFileDic.ContainsKey(id))
        {
            var callback = mAppendFileDic[id];
            callback?.Invoke(qGCallBackData.ret == 0, qGCallBackData.msg);
            mAppendFileDic.Remove(id);
        }
    }

    /// <summary>
    /// 复制文件
    /// </summary>
    private void CopyFileHandler(int id, QGCallBackData qGCallBackData)
    {
        if (mCopyFileDic.ContainsKey(id))
        {
            var callback = mCopyFileDic[id];
            callback?.Invoke(qGCallBackData.ret == 0, qGCallBackData.msg);
            mCopyFileDic.Remove(id);
        }
    }

    /// <summary>
    /// 创建目录
    /// </summary>
    private void MkDirHandler(int id, QGCallBackData qGCallBackData)
    {
        if (mMkDirDic.ContainsKey(id))
        {
            var callback = mMkDirDic[id];
            callback?.Invoke(qGCallBackData.ret == 0, qGCallBackData.msg);
            mMkDirDic.Remove(id);
        }
    }

    /// <summary>
    /// 读文件处理 (Old)
    /// </summary>
    /// <param name="id"></param>
    /// <param name="qGCallBackData"></param>
    private void ReadFileHandlerOld(int id, QGCallBackData qGCallBackData)
    {
        QGLog("ReadFileHandlerOld {0}", qGCallBackData.data);
        if (mReadFileDicOld.ContainsKey(id))
        {
            var callback = mReadFileDicOld[id];
            if (qGCallBackData.ret == 0)
            {
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(qGCallBackData.data);
                var len = qGCallBackData.readLen;
                callback.success?.Invoke(byteArray, len);
            }
            else
            {
                callback.failure?.Invoke(qGCallBackData.msg);
            }

            mReadFileDicOld.Remove(id);
        }
    }

    /// <summary>
    /// 读文件处理
    /// </summary>
    /// <param name="id"></param>
    /// <param name="qGCallBackData"></param>
    private void ReadFileHandler(int id, QGCallBackData qGCallBackData)
    {
        QGLog("ReadFileHandler {0}", qGCallBackData.data);
        if (mReadFileDic.ContainsKey(id))
        {
            var callback = mReadFileDic[id];
            if (qGCallBackData.ret == 0)
            {
                QGFileResponse response = new QGFileResponse
                {
                    callbackId = qGCallBackData.cmd.ToString(),
                    encoding = qGCallBackData.msg,
                    byteLength = qGCallBackData.readLen,
                };
                if (response.encoding.Contains("utf8"))
                {
                    response.textStr = qGCallBackData.data;
                }
                else
                {
                    var fileBuffer = new byte[response.byteLength];
                    QGGetFileBuffer(fileBuffer, response.callbackId);
                    response.textData = fileBuffer;
                }
                callback.success?.Invoke(response);
            }
            else 
            {
                callback.failure?.Invoke(qGCallBackData.msg);
            }

            mReadFileDic.Remove(id);
        }
    }

    /// <summary>
    /// 写文件处理
    /// </summary>
    /// <param name="id"></param>
    /// <param name="qGCallBackData"></param>
    private void WriteFileHandler(int id, QGCallBackData qGCallBackData)
    {
        QGLog("WriteFileHandler");
        if (mWriteFileDic.ContainsKey(id))
        {
            var callback = mWriteFileDic[id];
            callback?.Invoke(qGCallBackData.ret == 0, qGCallBackData.msg);
            mWriteFileDic.Remove(id);
        }
    }

    /// <summary>
    /// 获取文件 Stats 对象
    /// </summary>
    /// <param name="id"></param>
    /// <param name="qGCallBackData"></param>
    private void StatHandler(int id, QGCallBackData qGCallBackData)
    {
        QGLog("StatHandler {0}", qGCallBackData.data);
        if (mStatDic.ContainsKey(id))
        {
            var callback = mStatDic[id];
            QGStatResponse response = null;
            if (qGCallBackData.ret == 0)
            {
                response = JsonUtility.FromJson<QGStatResponse>(qGCallBackData.data);
            }

            callback?.Invoke(response, qGCallBackData.msg);

            mStatDic.Remove(id);
        }
    }

    /// <summary>
    /// 删除文件处理
    /// </summary>
    /// <param name="id"></param>
    /// <param name="qGCallBackData"></param>
    private void DeleteFileHandler(int id, QGCallBackData qGCallBackData)
    {
        QGLog("DeleteFileHandler");
        if (mDeleteFileDic.ContainsKey(id))
        {
            var callback = mDeleteFileDic[id];
            callback?.Invoke(qGCallBackData.ret == 0);
            mDeleteFileDic.Remove(id);
        }
    }

    /// <summary>
    /// 广告处理
    /// </summary>
    /// <param name="id"></param>
    /// <param name="qGCallBackData"></param>
    private void AdHandler(int id, QGCallBackData qGCallBackData)
    {
        QGLog("AdHandler");
        if (mAdDic.ContainsKey(id))
        {
            var callback = mAdDic[id];
            var info = JsonUtility.FromJson<AdInfo>(qGCallBackData.data);
            switch (info.type)
            {
                case 0:
                    //load
                    QGLog("AdHandler loaded info: " + info.loadInfo);
                    if (string.IsNullOrEmpty(info.loadInfo))
                    {
                        callback.onAdLoad?.Invoke(new AdLoadInfo());
                    }
                    else
                    {
                        try
                        {
                            var loadInfo = JsonUtility.FromJson<AdLoadInfo>(info.data);
                            callback.onAdLoad?.Invoke(loadInfo);
                        }
                        catch (Exception e)
                        {
                            QGLog("custom广告加载信息解析失败，返回空对象" + e);
                            callback.onAdLoad?.Invoke(new AdLoadInfo());
                        }
                    }
                    break;
                case 1:
                    //close
                    QGLog($"AdHandler {id} closed");
                    callback.onAdClose?.Invoke(info.isEnd);
                    break;
                case 2:
                    //error
                    QGLog($"AdHandler {id} error");
                    callback.onAdError?.Invoke(qGCallBackData.ret, qGCallBackData.msg);
                    break;
            }
        }
        else {
            QGLog($"AdHandler {id} no found!!!");
        }
    }

    private void ShowRecommedAdHandler(int id, QGCallBackData qGCallBackData)
    {
        if (mRecommendAdDic.ContainsKey(id))
        {
            var callback = mRecommendAdDic[id];
            callback?.Invoke(qGCallBackData.ret == 0, qGCallBackData.msg);
            mRecommendAdDic.Remove(id);
        }
    }

    private void CloseRecommedAdHandler(int id, QGCallBackData qGCallBackData)
    {
        if (mCloseRecommendAdDic.ContainsKey(id))
        {
            var callback = mCloseRecommendAdDic[id];
            callback?.Invoke(qGCallBackData.ret == 0, qGCallBackData.msg);
            mCloseRecommendAdDic.Remove(id);
        }
    }

    private void HttpHandler(int id, QGCallBackData qGCallBackData)
    {
        if (mHttpDic.ContainsKey(id))
        {
            var callback = mHttpDic[id];
            callback?.Invoke(qGCallBackData.ret >= 200 && qGCallBackData.ret < 300, qGCallBackData.msg, qGCallBackData.data);
            mHttpDic.Remove(id);
        }
    }

    private void ExitAppHandler(int id, QGCallBackData qGCallBackData)
    {
        QGLog("ExitAppHandler");
        if (mExitDic.ContainsKey(id))
        {
            var callback = mExitDic[id];
            callback?.Invoke(qGCallBackData.ret == 0);
            mExitDic.Remove(id);
        }
    }

    private void SetClipHandler(int id, QGCallBackData qGCallBackData)
    {
        QGLog("SetClipHandler");
        if (mSetClipBoardDic.ContainsKey(id))
        {
            var callback = mSetClipBoardDic[id];
            callback?.Invoke(qGCallBackData.ret == 0);
            mSetClipBoardDic.Remove(id);
        }
    }

    private void GetClipHandler(int id, QGCallBackData qGCallBackData)
    {
        QGLog("GetClipHandler");
        if (mGetClipBoardDic.ContainsKey(id))
        {
            var callback = mGetClipBoardDic[id];
            bool success = qGCallBackData.ret == 0;
            if (success) {
                callback?.Invoke(success, qGCallBackData.data);
            } else {
                callback?.Invoke(success, "");
            }
            mGetClipBoardDic.Remove(id);
        }
    }

    private void GetSystemInfoHandler(int id, QGCallBackData qGCallBackData)
    {
        QGLog("GetSystemInfoHandler {0}", qGCallBackData.data);
        if (mGetSystemInfoDic.ContainsKey(id))
        {
            var callback = mGetSystemInfoDic[id];
            bool success = qGCallBackData.ret == 0;
            if (success)
            {
                try {
                    var systemInfo = JsonUtility.FromJson<mi.SystemInfo>(qGCallBackData.data);
                    callback?.Invoke(success, systemInfo);
                } catch (Exception e) {
                    QGLog("GetSystemInfoHandler error={0}", e.ToString());
                    callback?.Invoke(false, new mi.SystemInfo());
                }
            }
            else
            {
                callback?.Invoke(success, new mi.SystemInfo());
            }
            mGetSystemInfoDic.Remove(id);
        }
    }

    #region public memebers

    /// <summary>
    /// 登录成功
    /// </summary>
    /// <param name="accountId">账号id</param>
    /// <param name="session">session用于账户验证</param>
    public delegate void LoginSuccess(long accountId, string session);
    /// <summary>
    /// 登录失败
    /// </summary>
    /// <param name="errCode">错误码</param>
    /// <param name="errMsg">错误日志</param>
    public delegate void LoginFailure(int errCode, string errMsg);

    /// <summary>
    /// 获取用户信息成功
    /// </summary>
    /// <param name="nickName"></param>
    /// <param name="avatarUrl"></param>
    /// <param name="gender"></param>
    public delegate void GetUserInfoSuccess(string nickName, string avatarUrl, int gender);
    /// <summary>
    /// 获取用户信息失败
    /// </summary>
    /// <param name="errCode">错误码</param>
    /// <param name="errMsg">错误日志</param>
    public delegate void GetUserInfoFailure(int errCode, string errMsg);

    /// <summary>
    /// 支付成功
    /// <param name="success">是否成功</param>
    /// <param name="errCode">错误码</param>
    /// <param name="errMsg">错误日志</param>
    /// </summary>
    public delegate void PayListener(bool success, int errCode, string errMsg);

    /// <summary>
    /// 是否创建过桌面图标
    /// </summary>
    /// <param name="success"></param>
    public delegate void HasInstalledListener(bool success);

    /// <summary>
    /// 创建桌面图标
    /// </summary>
    /// <param name="success">是否成功</param>
    /// <param name="code">失败code</param>
    /// <param name="errMsg">失败原因</param>
    public delegate void CreateShortcutListener(bool success, int code, string errMsg);

    /// <summary>
    /// 文件是否存在
    /// </summary>
    /// <param name="exists"></param>
    public delegate void FileExistsListener(bool exists);

    /// <summary>
    /// 在文件结尾追加内容 监听
    /// </summary>
    public delegate void AppendFileListener(bool success, string err);

    /// <summary>
    /// 复制文件 监听
    /// </summary>
    public delegate void CopyFileListener(bool success, string err);

    /// <summary>
    /// 创建目录 监听
    /// </summary>
    public delegate void MkDirListener(bool success, string err);

    /// <summary>
    /// 读取文件成功 (Old)
    /// </summary>
    /// <param name="length"></param>
    /// <param name="data"></param>
    public delegate void ReadFileSuccessOld(byte []data, int length);
    /// <summary>
    /// 读取文件成功
    /// </summary>
    public delegate void ReadFileSuccess(QGFileResponse response);
    /// <summary>
    /// 读取文件失败
    /// </summary>
    /// <param name="err">错误信息</param>
    public delegate void ReadFileFailure(string err);
    /// <summary>
    /// 写文件监听
    /// </summary>
    /// <param name="err">错误信息</param>
    public delegate void WriteFileListener(bool success, string err);
    /// <summary>
    /// 获取文件 Stats 对象 监听
    /// </summary>
    public delegate void StatListener(QGStatResponse response, string err);
    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="err">是否成功</param>
    public delegate void DeleteFileListener(bool success);
    /// <summary>
    /// 展示推荐位广告成功
    /// </summary>
    /// <param name="success">是否成功</param>
    /// /// <param name="errMsg">错误信息</param>
    public delegate void ShowRecommendAdListener(bool success, string errMsg);
    /// <summary>
    /// 关闭推荐位广告成功
    /// <param name="success">是否成功</param>
    /// <param name="errMsg">错误信息</param>
    /// </summary>
    public delegate void CloseRecommendAdListener(bool success, string errMsg);
    /// <summary>
    /// 广告加载成功返回信息，！！！注意：只有custom广告会返回，其他类型返回空对象！！！
    /// <param name="info">广告信息</param>
    /// </summary>
    public delegate void OnAdLoadListener(AdLoadInfo info);
    /// <summary>
    /// 广告关闭
    /// <param name="isEnd">激励视频使用，判断视频是否是在用户完整观看的情况下被关闭的</param>
    /// </summary>
    public delegate void OnAdCloseListener(bool isEnd);
    /// <summary>
    /// 广告错误监听
    /// </summary>
    /// <param name="code">错误code</param>
    /// <param name="msg">错误日志</param>
    public delegate void OnAdErrorListener(int code, string msg);
    /// <summary>
    /// http监听回调
    /// </summary>
    /// <param name="success">是否成功</param>
    /// <param name="statusMsg">状态信息</param>
    /// <param name="data">请求返回的数据</param>
    public delegate void HttpListener(bool success, string statusMsg, string data);
    /// <summary>
    /// 退出监听
    /// </summary>
    /// <param name="success">是否成功</param>
    public delegate void ExitListener(bool success);
    /// <summary>
    /// 设置监听
    /// </summary>
    /// <param name="success">是否成功</param>
    public delegate void SetClipBoardListener(bool success);
    /// <summary>
    /// 剪切板监听
    /// </summary>
    /// <param name="success">是否成功</param>
    /// /// <param name="success">剪切板内容</param>
    public delegate void GetClipBoardListener(bool success, string data);
    /// <summary>
    /// 获得系统信息监听
    /// </summary>
    /// <param name="success">是否成功</param>
    /// /// <param name="data">系统信息</param>
    public delegate void GetSystemInfoListener(bool success, mi.SystemInfo data);

    /// <summary>
    /// js回调
    /// </summary>
    /// <param name="json">信息</param>
    public void QGCallBack(string json)
    {
        QGLog("unity receive qg msg: {0}", json);
        var qgData = JsonUtility.FromJson<QGCallBackData>(json);
        QGLog("unity receive json parse qg cmd: {0}", qgData.cmd);
        var requestId = qgData.cmd;
        if (mLoginDic.ContainsKey(requestId))
        {
            qgData.type = QGCallBackType.Login;
        }

        if (mGetUserInfoDic.ContainsKey(requestId))
        {
            qgData.type = QGCallBackType.GetUserInfo;
        }

        if (mOnUserInfoDic.ContainsKey(requestId))
        {
            qgData.type = QGCallBackType.OnUserInfo;
        }

        if (mPayDic.ContainsKey(requestId))
        {
            qgData.type = QGCallBackType.Pay;
        }

        if (mHasInstalledDic.ContainsKey(requestId))
        {
            qgData.type = QGCallBackType.HasInstalled;
        }

        if (mInstalledDic.ContainsKey(requestId))
        {
            qgData.type = QGCallBackType.Installed;
        }

        if (mFileExistsDic.ContainsKey(requestId))
        {
            qgData.type = QGCallBackType.AccessFile;
        }

        if (mAppendFileDic.ContainsKey(requestId))
        {
            qgData.type = QGCallBackType.AppendFile;
        }

        if (mCopyFileDic.ContainsKey(requestId))
        {
            qgData.type = QGCallBackType.CopyFile;
        }

        if (mMkDirDic.ContainsKey(requestId))
        {
            qgData.type = QGCallBackType.MkDir;
        }

        if (mReadFileDicOld.ContainsKey(requestId))
        {
            qgData.type = QGCallBackType.ReadFileOld;
        }

        if (mReadFileDic.ContainsKey(requestId))
        {
            qgData.type = QGCallBackType.ReadFile;
        }

        if (mWriteFileDic.ContainsKey(requestId))
        {
            qgData.type = QGCallBackType.WriteFile;
        }

        if (mStatDic.ContainsKey(requestId))
        {
            qgData.type = QGCallBackType.FileStat;
        }

        if (mDeleteFileDic.ContainsKey(requestId))
        {
            qgData.type = QGCallBackType.DeleteFile;
        }

        if (mAdDic.ContainsKey(requestId))
        {
            qgData.type = QGCallBackType.Ad;
        }

        if (mRecommendAdDic.ContainsKey(requestId))
        {
            qgData.type = QGCallBackType.Recommend;
        }

        if (mCloseRecommendAdDic.ContainsKey(requestId))
        {
            qgData.type = QGCallBackType.RecommendClose;
        }

        if (mHttpDic.ContainsKey(requestId))
        {
            qgData.type = QGCallBackType.Http;
        }

        if (mExitDic.ContainsKey(requestId))
        {
            qgData.type = QGCallBackType.ExitApp;
        }

        if (mSetClipBoardDic.ContainsKey(requestId))
        {
            qgData.type = QGCallBackType.SetClip;
        }

        if (mGetClipBoardDic.ContainsKey(requestId))
        {
            qgData.type = QGCallBackType.GetClip;
        }

        if (mGetSystemInfoDic.ContainsKey(requestId))
        {
            qgData.type = QGCallBackType.GetSystemInfo;
        }

        lock (mWaitIdDicLock) 
        {
            if (!mWaitIdDic.ContainsKey(qgData.cmd)) {
                mWaitIdDic.Add(qgData.cmd, new List<QGCallBackData>());
            }
            mWaitIdDic[qgData.cmd].Add(qgData);
        }
    }

    /// <summary>
    /// 初始化，请先于其他接口调用！！！
    /// </summary>
    public void Init()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (!mInited)
        {
            mInited = true;
            QGInit();
        }
#endif
    }

    /// <summary>
    /// 输出打印到快游戏
    /// </summary>
    /// <param name="msg"></param>
    public void QGLog(string msg, params object[] args)
    {
        if (!mLogOpen) {
            return;
        }
#if UNITY_WEBGL && !UNITY_EDITOR
        try
        {
            if (args == null || args.Length == 0)
            {
                QGPrint($"[tid-{Thread.CurrentThread.ManagedThreadId}]{msg}");
            }
            else 
            {
                QGPrint(string.Format($"[tid-{Thread.CurrentThread.ManagedThreadId}]{msg}", args));
            }
        }
        catch (Exception e)
        {
            QGPrint($"日志打印失败，请检查传入的参数，{e}");
        }
#endif
    }

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="success">成功</param>
    /// <param name="failure">失败</param>
    public void Login(LoginSuccess success, LoginFailure failure)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        mRequestId += 1;
		QGPrintInt(mRequestId);

        mLoginDic[mRequestId] = new LoginCallback
        {
            failure = failure,
            success = success
        };

        QGLogin(mRequestId);
#endif
    }

    /// <summary>
    /// 获取用户信息
    /// </summary>
    /// <param name="success"></param>
    /// <param name="failure"></param>
    public void GetUserInfo(GetUserInfoSuccess success, GetUserInfoFailure failure)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        mRequestId += 1;

        mGetUserInfoDic[mRequestId] = new GetUserInfoCallback
        {
            failure = failure,
            success = success
        };

        QGGetUserInfo(mRequestId);
#endif
    }

    /// <summary>
    /// 用户信息变化监听
    /// </summary>
    /// <param name="userInfo"></param>
    public void AddOnUserInfoChangeListener(GetUserInfoSuccess userInfo)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        mRequestId += 1;

        mOnUserInfoDic[mRequestId] = new OnUserInfoCallback
        {
            success = userInfo
        };

        QGOnUserInfoChanged(mRequestId);
#endif
    }

    /// <summary>
    /// 支付
    /// </summary>
    /// <param name="orderInfo">订单信息</param>
    /// <param name="success">成功</param>
    public void Pay(OrderInfo orderInfo, PayListener success)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        mRequestId += 1;
        mPayDic[mRequestId] = success;
        QGPay(mRequestId, JsonUtility.ToJson(orderInfo));
#endif
    }

    /// <summary>
    /// 是否创建过游戏桌面图标
    /// </summary>
    public void HasShortcut(HasInstalledListener listener)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        mRequestId += 1;
        mHasInstalledDic[mRequestId] = listener;
        QGHasInstalled(mRequestId);
#endif
    }

    /// <summary>
    /// 创建桌面图标
    /// </summary>
    /// <param name="msg">权限弹窗上的说明文字，用于向用户解释为什么要创建桌面图标</param>
    /// <param name="listener">监听</param>
    public void CreateShortcut(string msg, CreateShortcutListener listener)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        mRequestId += 1;
        mInstalledDic[mRequestId] = listener;
        QGInstall(mRequestId, msg);
#endif
    }

    [Obsolete("Use ReadFile(QGFileParam param, ReadFileSuccess success, ReadFileFailure failure) instead.", false)]
    public void ReadFile(string fileName, ReadFileSuccessOld success, ReadFileFailure failure)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        mRequestId += 1;
        mReadFileDicOld[mRequestId] = new ReadFileCallbackOld
        {
            success = success,
            failure = failure
        };
        QGReadFile(mRequestId, fileName, "utf8");
#endif
    }

    /// <summary>
    /// 读取文件
    /// </summary>
    public void ReadFile(QGFileParam param, ReadFileSuccess success, ReadFileFailure failure)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        mRequestId += 1;
        mReadFileDic[mRequestId] = new ReadFileCallback
        {
            success = success,
            failure = failure
        };
        QGReadFile(mRequestId, param.fileName, param.encoding);
#endif
    }

    /// <summary>
    /// 读取文件的同步版本
    /// </summary>
    public QGFileInfo ReadFileSync(QGFileParam param)
    {
        string msg = QGReadFileSync(param.fileName, param.encoding);
        QGFileResponse res = JsonUtility.FromJson<QGFileResponse>(msg);
        if (res == null)
        {
            QGLog("QGFileResponse: null");
            return null;
        }

        QGLog("QGFileResponse: " + JsonUtility.ToJson(res));
        QGFileInfo fileInfo = new QGFileInfo();
        fileInfo.textStr = res.textStr;
        if (res.encoding != "utf8")
        {
            var fileBuffer = new byte[res.byteLength];
            QGGetFileBuffer(fileBuffer, res.callbackId);
            fileInfo.textData = fileBuffer;
        }

        return fileInfo;
    }

    [Obsolete("Use WriteFile(QGFileParam param, WriteFileListener listener) instead.", false)]
    public void WriteFile(string fileName, byte[] data, bool append, WriteFileListener listener)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        mRequestId += 1;
        mWriteFileDic[mRequestId] = listener;
        string str = System.Text.Encoding.UTF8.GetString(data);
        QGWriteFile(mRequestId, fileName, "utf8", append, str, null, 0);
#endif
    }

    /// <summary>
    /// 写文件
    /// </summary>
    public void WriteFile(QGFileParam param, WriteFileListener listener)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        mRequestId += 1;
        mWriteFileDic[mRequestId] = listener;
        QGWriteFile(mRequestId, param.fileName, param.encoding, param.append, param.textStr, param.textData, param.textData?.Length ?? 0);
#endif
    }

    /// <summary>
    /// 写文件的同步版本
    /// </summary>
    public bool WriteFileSync(QGFileParam param)
    {
        return QGWriteFileSync(param.fileName, param.encoding, param.append, param.textStr, param.textData, param.textData?.Length ?? 0);
    }

    /// <summary>
    /// 获取文件 Stats 对象
    /// </summary>
    public void Stat(QGDirParam param, StatListener listener)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        mRequestId += 1;
        mStatDic[mRequestId] = listener;
        QGStat(mRequestId, param.filePath);
#endif
    }

    /// <summary>
    /// 获取文件 Stats 对象 的同步版本
    /// </summary>
    public QGStatResponse StatSync(QGDirParam param)
    {
        string msg = QGStatSync(param.filePath, param.recursive);
        QGStatResponse res = JsonUtility.FromJson<QGStatResponse>(msg);
        if (res == null)
        {
            QGLog("QGStatResponse: null");
            return null;
        }

        QGLog("QGStatResponse: " + JsonUtility.ToJson(res));

        return res;
    }

    [Obsolete("Use AccessFile instead.", false)]
    public void FileExists(string fileName, FileExistsListener listener)
    {
        AccessFile(fileName, listener);
    }

    /// <summary>
    /// 判断文件是否存在
    /// </summary>
    public void AccessFile(string fileName, FileExistsListener listener)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        mRequestId += 1;
        mFileExistsDic[mRequestId] = listener;
        QGAccessFile(mRequestId, fileName);
#endif
    }

    /// <summary>
    /// 判断文件是否存在 的同步版本
    /// </summary>
    public bool AccessFileSync(string fileName)
    {
        return QGAccessFileSync(fileName);
    }

    /// <summary>
    /// 在文件结尾追加内容
    /// </summary>
    public void AppendFile(QGFileParam param, AppendFileListener listener)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        mRequestId += 1;
        mAppendFileDic[mRequestId] = listener;
        QGAppendFile(mRequestId, param.fileName, param.encoding, param.textStr, param.textData, param.textData == null ? 0 : param.textData.Length);
#endif
    }

    /// <summary>
    /// 在文件结尾追加内容 的同步版本
    /// </summary>
    public void AppendFileSync(QGFileParam param)
    {
        QGAppendFileSync(param.fileName, param.encoding, param.textStr, param.textData, param.textData == null ? 0 : param.textData.Length);
    }

    /// <summary>
    /// 复制文件
    /// </summary>
    public void CopyFile(string srcPath, string destPath, CopyFileListener listener)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        mRequestId += 1;
        mCopyFileDic[mRequestId] = listener;
        QGCopyFile(mRequestId, srcPath, destPath);
#endif
    }

    /// <summary>
    /// 复制文件 的同步版本
    /// </summary>
    public bool CopyFileSync(string srcPath, string destPath)
    {
        return QGCopyFileSync(srcPath, destPath);
    }

    /// <summary>
    /// 创建目录
    /// </summary>
    public void MkDir(QGDirParam param, MkDirListener listener)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        mRequestId += 1;
        mMkDirDic[mRequestId] = listener;
        QGMkDir(mRequestId, param.filePath);
#endif
    }

    /// <summary>
    /// 创建目录 的同步版本
    /// </summary>
    /// <param name="param"></param>
    public void MkDirSync(QGDirParam param)
    {
        QGMkDirSync(param.filePath, param.recursive);
    }

    /// <summary>
    /// 删除目录
    /// </summary>
    public void DeleteDir(QGDirParam param, MkDirListener listener)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        mRequestId += 1;
        mMkDirDic[mRequestId] = listener;
        QGDeleteDir(mRequestId, param.filePath, param.recursive);
#endif
    }

    /// <summary>
    /// 删除目录 的同步版本
    /// </summary>
    /// <param name="param"></param>
    public void DeleteDirSync(QGDirParam param)
    {
        QGDeleteDirSync(param.filePath, param.recursive);
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="listener"></param>
    public void DeleteFile(string fileName, DeleteFileListener listener)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        mRequestId += 1;
        mDeleteFileDic[mRequestId] = listener;
        QGDeleteFile(mRequestId, fileName);
#endif
    }

    /// <summary>
    /// 删除文件 的同步版本
    /// </summary>
    public void DeleteFileSync(string fileName)
    {
        QGDeleteFileSync(fileName);
    }

    /// <summary>
    /// 判断是否有key的存储
    /// </summary>
    /// <param name="key">key</param>
    /// <returns></returns>
    public bool HasKV(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            return false;
        }
        return !string.IsNullOrEmpty(QGGetItem(key));
    }

    /// <summary>
    /// key-value int类型本地存储 
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="value">value</param>
    public void SetKVInt(string key, int value) 
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (string.IsNullOrEmpty(key))
        {
            return;
        }

        QGSetItem(key, value.ToString());
#endif
    }

    /// <summary>
    /// 获取 key-value int值
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="defaultValue">默认值，默认0, 注意：如果没有键值也会返回默认值！！！</param>
    public int GetKVInt(string key, int defaultValue = 0)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        string item = QGGetItem(key);
        if (string.IsNullOrEmpty(item))
        {
            return defaultValue;
        }

        try
        {
            return int.Parse(item);
        }
        catch (Exception) 
        {
            return defaultValue;
        }
#endif
        return defaultValue;
    }

    /// <summary>
    /// key-value float类型本地存储 
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="value">value</param>
    public void SetKVFloat(string key, float value)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (string.IsNullOrEmpty(key))
        {
            return;
        }

        QGSetItem(key, value.ToString());
#endif
    }

    /// <summary>
    /// 获取 key-value float值
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="defaultValue">默认值，默认0， 注意：如果没有键值也会返回默认值！！！</param>
    public float GetKVFloat(string key, float defaultValue = 0)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (string.IsNullOrEmpty(key))
        {
            return defaultValue;
        }

        string item = QGGetItem(key);
        try
        {
            return float.Parse(item);
        }
        catch (Exception)
        {
            return defaultValue;
        }
#endif
        return defaultValue;
    }

    /// <summary>
    /// key-value double类型本地存储 
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="value">value</param>
    public void SetKVDouble(string key, double value)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (string.IsNullOrEmpty(key))
        {
            return;
        }
        QGSetItem(key, value.ToString());
#endif
    }

    /// <summary>
    /// 获取 key-value double
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="defaultValue">默认值，默认0，注意：如果没有键值也会返回默认值！！！</param>
    public double GetKVDouble(string key, double defaultValue = 0)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (string.IsNullOrEmpty(key))
        {
            return defaultValue;
        }

        string item = QGGetItem(key);
        try
        {
            return double.Parse(item);
        }
        catch (Exception)
        {
            return defaultValue;
        }
#endif
        return defaultValue;
    }

    /// <summary>
    /// key-value string类型本地存储 
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="value">value</param>
    public void SetKVString(string key, string value)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (string.IsNullOrEmpty(key))
        {
            return;
        }
        QGSetItem(key, value);
#endif
    }

    /// <summary>
    /// 获取 key-value string值
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="defaultValue">默认值，默认， 注意：如果没有键值也会返回默认值！！！</param>
    public string GetKVString(string key, string defaultValue = "")
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (string.IsNullOrEmpty(key))
        {
            return defaultValue;
        }
        return string.IsNullOrEmpty(QGGetItem(key))? defaultValue : QGGetItem(key);
#endif
        return defaultValue;
    }

    /// <summary>
    /// 删除key-value 存储
    /// </summary>
    /// <param name="key">key</param>
    public void DeleteKV(string key)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (string.IsNullOrEmpty(key))
        {
            return;
        }
        QGDeleteItem(key);
#endif
    }

    /// <summary>
    /// 删除所有kv存储
    /// </summary>
    public void DeleteAllKV()
    {
        QGLog($"ClearAllKV");
#if UNITY_WEBGL && !UNITY_EDITOR
        QGClearAllKV();
#endif
    }

    /// <summary>
    /// 创建banner广告
    /// </summary>
    /// <param name="adId">广告id</param>
    /// <param name="left">横幅广告组件的左上角横坐标</param>
    /// <param name="top">横幅广告组件的左上角纵坐标</param>
    /// <param name="width">横幅广告组件的宽度</param>
    /// <param name="height">横幅广告组件的高度</param>
    /// <param name="listener">监听器</param>
    /// <returns></returns>
    public bool CreateBannerAd(string adId, int left, int top, int width, int height, AdEventListener listener) 
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        mRequestId += 1;
        mAdDic[mRequestId] = listener;
        return QGCreateBannerAd(mRequestId, left, top, width, height, adId);
#endif
        return false;
    }

    /// <summary>
    /// 创建插屏广告
    /// </summary>
    /// <param name="adId">广告id</param>
    /// <param name="listener">监听器</param>
    /// <returns></returns>
    public bool CreateInterstitialAd(string adId, AdEventListener listener)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        mRequestId += 1;
        mAdDic[mRequestId] = listener;
        return QGCreateInterstitialAd(mRequestId, adId);
#endif
        return false;
    }

    [Obsolete("Use CreateCustomAd instead.", false)]
    public bool CreateNativeAd(string adId, int left, int top, int width, int height, AdEventListener listener)
    {
        return CreateCustomAd(adId, left, top, width, height, listener);
    }

    /// <summary>
    /// 创建原生广告
    /// </summary>
    /// <param name="adId">广告id</param>
    /// <param name="left">模板广告组件的左上角横坐标</param>
    /// <param name="top">模板广告组件的左上角纵坐标</param>
    /// <param name="width">模板广告组件的宽度</param>
    /// <param name="height">模板广告组件的高度</param>
    /// <param name="listener">监听器</param>
    /// <returns></returns>
    public bool CreateCustomAd(string adId, int left, int top, int width, int height, AdEventListener listener)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        mRequestId += 1;
        mAdDic[mRequestId] = listener;
        return QGCreateCustomAd(mRequestId, left, top, width, height, adId);
#endif
        return false;
    }

    /// <summary>
    /// 创建激励视频广告
    /// </summary>
    /// <param name="adId">广告id</param>
    /// <param name="listener">监听器</param>
    /// <returns></returns>
    public bool CreateRewardedVideoAd(string adId, AdEventListener listener)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        mRequestId += 1;
        mAdDic[mRequestId] = listener;
        return QGCreateRewardedVideoAd(mRequestId, adId);
#endif
        return false;
    }

    /// <summary>
    /// 展示互推盒子广告
    /// 互推盒子广告介绍：https://dev.mi.com/distribute/doc/details?pId=1442
    /// </summary>
    /// <param name="adId">广告id</param>
    /// <param name="type">
    /// 广告类型: 
    /// 1. type=100，    互推盒子-九宫格 
    /// 2. type=120，    互推盒子-横幅 
    /// 3. type=130/140，互推盒子-抽屉，    白色背景为130，黑色背景为140
    /// 4. type=150，    互推盒子-悬浮球
    /// </param>
    /// <param name="listener">监听器</param>
    /// <returns></returns>
    public void ShowRecommendAd(string adId, int type, ShowRecommendAdListener listener)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        mRequestId += 1;
        mRecommendAdDic[mRequestId] = listener;
        QGShowRecommendAd(mRequestId, adId, type);
#endif
    }

    /// <summary>
    /// 关闭互推盒子广告
    /// 互动盒子广告介绍：https://dev.mi.com/distribute/doc/details?pId=1442
    /// </summary>
    /// <param name="adId">广告id</param>
    /// <param name="type">
    /// 广告类型: 
    /// 1. type=100，    互推盒子-九宫格 
    /// 2. type=120，    互推盒子-横幅 
    /// 3. type=130/140，互推盒子-抽屉，    白色背景为130，黑色背景为140
    /// 4. type=150，    互推盒子-悬浮球
    /// </param>
    /// <param name="listener">监听器</param>
    public void CloseRecommendAd(string adId, int type, CloseRecommendAdListener listener)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        mRequestId += 1;
        mCloseRecommendAdDic[mRequestId] = listener;
        QGCloseRecommendAd(mRequestId, adId, type);
#endif
    }

    /// <summary>
    /// 加载广告
    /// </summary>
    /// <param name="adId"></param>
    public void LoadAd(string adId)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        mRequestId += 1;
        QGLoadAd(mRequestId, adId);
#endif
    }

    /// <summary>
    /// 展示广告
    /// </summary>
    /// <param name="adId"></param>
    public void ShowAd(string adId)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        mRequestId += 1;
        QGShowAd(mRequestId, adId);
#endif
    }
    
    /// <summary>
    /// 隐藏广告
    /// </summary>
    /// <param name="adId"></param>
    public void HideAd(string adId)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        mRequestId += 1;
        QGHideAd(mRequestId, adId);
#endif
    }

    /// <summary>
    /// 销毁广告，销毁后，如需重新显示，请先调用创建广告
    /// </summary>
    /// <param name="adId"></param>
    public void DestroyAd(string adId)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        mRequestId += 1;
        QGDestroyAd(mRequestId, adId);
#endif
    }

    /// <summary>
    /// http post
    /// </summary>
    /// <param name="url">url</param>
    /// <param name="headers">headers</param>
    /// <param name="data">post data</param>
    /// <param name="listener">监听器</param>
    /// <param name="timeout">超时时间/毫秒（默认15*1000毫秒）</param>
    public void HttpPost(string url, string headers, string data, HttpListener listener, int timeout = 15000)
    {
        QGLog($"post {headers} " + $"{data}");
#if UNITY_WEBGL && !UNITY_EDITOR
        QGPrint($"[tid-{Thread.CurrentThread.ManagedThreadId}]http-post: {url}{headers}{listener}");        
        mRequestId += 1;
        mHttpDic[mRequestId] = listener;
        QGHttpPost(mRequestId, timeout, url, headers, data);
#endif
    }

    /// <summary>
    /// http get
    /// </summary>
    /// <param name="url">url</param>
    /// <param name="headers">headers</param>
    /// <param name="param">url params</param>
    /// <param name="listener">监听器</param>
    /// <param name="timeout">超时时间/毫秒（默认15*1000毫秒）</param>
    public void HttpGet(string url, string headers, string param, HttpListener listener, int timeout = 15000)
    {
        QGLog($"get {headers} " + $"{param}");
#if UNITY_WEBGL && !UNITY_EDITOR
        QGPrint($"[tid-{Thread.CurrentThread.ManagedThreadId}]http-get: {url}{headers}{param}");
        mRequestId += 1;
        mHttpDic[mRequestId] = listener;
        QGHttpGet(mRequestId, timeout, url, headers, param);
#endif
    }

    /// <summary>
    /// 退出app
    /// </summary>
    /// <param name="listener"></param>
    public void ExitApp(ExitListener listener)
    {
        QGLog($"ExitApp");
#if UNITY_WEBGL && !UNITY_EDITOR
        mRequestId += 1;
        mExitDic[mRequestId] = listener;
        QGExitApp(mRequestId);
#endif
    }

    /// <summary>
    /// 设置剪切板内容
    /// </summary>
    /// <param name="clipData"></param>
    /// <param name="listener"></param>
    public void SetCilpBoardData(string clipData, SetClipBoardListener listener)
    {
        QGLog($"SetCilpBoardData");
#if UNITY_WEBGL && !UNITY_EDITOR
        mRequestId += 1;
        mSetClipBoardDic[mRequestId] = listener;
        QGSetClipboardData(mRequestId, clipData);
#endif
    }

    /// <summary>
    /// 获取剪切板内容
    /// </summary>
    /// <param name="listener"></param>
    public void GetCilpBoardData(GetClipBoardListener listener)
    {
        QGLog($"GetCilpBoardData");
#if UNITY_WEBGL && !UNITY_EDITOR
        mRequestId += 1;
        mGetClipBoardDic[mRequestId] = listener;
        QGGetClipboardData(mRequestId);
#endif
    }

    /// <summary>
    /// 日志开关
    /// </summary>
    /// <param name="open">开关</param>
    public void SwitchLog(bool open)
    {
        QGLog($"SwitchLog");
        mLogOpen = open;
    }

    /// <summary>
    /// 获取系统信息
    /// </summary>
    /// <param name="listener">系统监听</param>
    public void GetSystemInfo(GetSystemInfoListener listener)
    {
        QGLog($"GetSystemInfo");
#if UNITY_WEBGL && !UNITY_EDITOR
        mRequestId += 1;
        mGetSystemInfoDic[mRequestId] = listener;
        QGGetSystemInfo(mRequestId);
#endif
    }

    /// <summary>
    /// 同步获取系统信息
    /// </summary>
    public mi.SystemInfo GetSystemInfoSync()
    {
        string msg = QGGetSystemInfoSync();
        try
        {
            var systemInfo = JsonUtility.FromJson<mi.SystemInfo>(msg);
            QGLog("QGGetSystemInfoSync: " + JsonUtility.ToJson(systemInfo));
            return systemInfo;
        }
        catch (Exception e)
        {
            QGLog("QGGetSystemInfoSync error = {0}", e.ToString());
            return new mi.SystemInfo();
        }
    }

    /// <summary>
    /// 获取菜单按钮（右上角胶囊按钮）的布局位置信息。坐标信息以屏幕左上角为原点。
    /// </summary>
    public MenuButtonBoundingClientRect GetMenuButtonBoundingClientRect()
    {
        string msg = QGGetMenuButtonBoundingClientRect();
        try
        {
            var menuButtonBoundingClientRect = JsonUtility.FromJson<MenuButtonBoundingClientRect>(msg);
            QGLog("QGGetMenuButtonBoundingClientRect: " + JsonUtility.ToJson(menuButtonBoundingClientRect));
            return menuButtonBoundingClientRect;
        }
        catch (Exception e)
        {
            QGLog("QGGetMenuButtonBoundingClientRect error = {0}", e.ToString());
            return new MenuButtonBoundingClientRect();
        }
    }

    /// <summary>
    /// 设置屏幕帧率 
    /// </summary>
    /// <param name="fps">帧率值，范围：0-60</param>
    public void SetScreenFPS(int fps)
    {
        QGLog($"SetPreferredFramesPerSecond");
#if UNITY_WEBGL && !UNITY_EDITOR
        QGSetPreferredFramesPerSecond(fps);
#endif
    }

    /// <summary>
    /// 隐藏自定义loading页面 (不要手动调用！！！请把MiGameHelper脚本挂载到main camera上，会自动调用)
    /// </summary>
    public void HideLoading()
    {
        QGLog($"[from unity]HideLoading");
#if UNITY_WEBGL && !UNITY_EDITOR
        QGHideLoading();
#endif
    }

    /// <summary>
    /// 主动检查更新快游戏
    /// </summary>
    public void UpdateGame()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        QGUpdateGame();
#endif
    }

    #endregion
}
