using System;
using static MiBridge;

namespace mi
{
    public struct QGCallBackData
    {
        public int cmd;
        public int ret;
        public string msg;
        public string data;
        public int readLen;
        public byte[] readFileData;
        public QGCallBackType type;
    }

    public enum QGCallBackType
    {
        Login,
        GetUserInfo,
        OnUserInfo,
        Pay,
        HasInstalled,
        Installed,
        AccessFile,
        ReadFileOld,
        ReadFile,
        WriteFile,
        DeleteFile,
        Ad,
        Recommend,
        RecommendClose,
        Http,
        ExitApp,
        SetClip,
        GetClip,
        GetSystemInfo,
        MkDir,
        CopyFile,
        AppendFile,
        FileStat,
    }

    [Serializable]
    public class QGBaseResponse
    {
        public string callbackId;
        public string errMsg;
        public string errCode;
    }

    public struct LoginCallback
    {
        public LoginSuccess success;
        public LoginFailure failure;
    }

    public struct GetUserInfoCallback
    {
        public GetUserInfoSuccess success;
        public GetUserInfoFailure failure;
    }

    public struct OnUserInfoCallback
    {
        public GetUserInfoSuccess success;
    }

    public struct ReadFileCallbackOld
    {
        public ReadFileSuccessOld success;
        public ReadFileFailure failure;
    }

    public struct ReadFileCallback
    {
        public ReadFileSuccess success;
        public ReadFileFailure failure;
    }

    public struct QGAccoutInfo
    {
        public long appAccountId;
        public string session;
    }

    [Serializable]
    public struct QGUserInfo
    {
        public UserInfo userInfo;
    }

    [Serializable]
    public struct UserInfo
    {
        public string nickName;
        public string avatarUrl;
        public int gender;
    }

    public struct QGPayInfo
    {
        public string resultStatus;
        public string memo;
    }

    public struct AdInfo
    {
        public int type;
        public string data;
        public string loadInfo;
        public bool isEnd;
    }

    [Serializable]
    public struct QGLaunchExtraData
    {
        public string engineType;
        public bool useVConsole;
        public bool waitingDebugger;
        public string __SRC__;
    }

    [Serializable]
    public struct QGLaunchReferInfo
    {
        public string package;
        public string appId;
        public string type;
        public QGLaunchExtraData extraData;
    }

    [Serializable]
    public struct QGLaunchQuery
    {
        public string type;
    }

    [Serializable]
    public struct QGLaunchInfo
    {
        public int scene; //启动小游戏的场景值
        public QGLaunchQuery query; //启动小游戏的 query 参数
        public QGLaunchReferInfo referrerInfo;
    }

    /// <summary>
    /// 订单信息
    /// </summary>
    public struct OrderInfo
    {
        public string appId; // 游戏唯一ID
        public long appAccountId; // 与登录接口返回的appAccountId一致
        public string session; // 与登录接口返回的session一致
        public string cpOrderId; // 游戏订单号
        public string cpUserInfo; // cp透传信息 (非空)
        public string displayName; // 支付的时候显示的商品名称
        public int feeValue; // 价格 单位分
        public string sign; // 签名 用于校验 具体生成方式，请查看：https://dev.mi.com/distribute/doc/details?pId=1109
    }

    /// <summary>
    /// 广告加载成功返回参数，注意：只有custom广告会返回！！！
    /// </summary>
    public struct AdLoadInfo
    {
        public string adId; //广告标识，用来上报曝光与点击
        public string title; //广告标题
        public string desc; //广告描述
        public string icon; //推广应用的Icon图标
        public string[] imgUrlList; //	广告图片
        public string logoUrl; // 广告标签图片
        public string clickBtnTxt; // 点击按钮文本描述
        public int creativeType; // 获取广告类型，取值说明：0：无 1：纯文字 2：图片 3：图文混合 4：视频
        public int interactionType; // 获取广告点击之后的交互类型，取值说明： 0：无 1：浏览类 2：下载类 3：浏览器（下载中间页广告） 4：打开应用首页 5：打开应用详情页
    }

    /// <summary>
    /// 广告事件监听
    /// </summary>
    public struct AdEventListener
    {
        public OnAdLoadListener onAdLoad;
        public OnAdCloseListener onAdClose;
        public OnAdErrorListener onAdError;
    }

    /// <summary>
    /// 系统信息
    /// </summary>
    [System.Serializable]
    public class SystemInfo
    {
        public String brand; // 设备品牌
        public String model; // 设备型号
        public String system; // 操作系统名称，如"Android 8.1.0"
        public String platformVersionName; // 运行平台版本名称
        public int platformVersionCode; // 运行平台标准版本号，如"1040"
        public String language; // 系统语言
        public float screenWidth; // 系统语言
        public float screenHeight; // 屏幕宽
        public float pixelRatio; // 设备像素比
        public float windowHeight; // 可使用窗口高度
        public float windowWidth; // 可使用窗口宽度
        public float statusBarHeight; // 状态栏/异形缺口高度
        public SafeArea safeArea; // 在竖屏正方向下的安全区域
    }

    [System.Serializable]
    public class SafeArea
    {
        public float bottom; // 安全区域右下角纵坐标
        public float height; // 安全区域的高度，单位逻辑像素
        public float left; // 安全区域左上角横坐标
        public float right; // 安全区域右下角横坐标
        public float top; // 安全区域左上角纵坐标
        public float width; // 安全区域的宽度，单位逻辑像素
    }

    [Serializable]
    public class MenuButtonBoundingClientRect
    {
        public float width; // 宽度，单位：px
        public float height; // 高度，单位：px
        public float top; // 上边界坐标，单位：px
        public float right; // 右边界坐标，单位：px
        public float bottom; // 下边界坐标，单位：px
        public float left; // 左边界坐标，单位：px
    }

    public class KeyboardParam
    {
        public string defaultValue; //键盘输入框显示的默认值
        public int maxLength; //键盘中文本的最大长度
        public bool multiple; //是否为多行输入
        public bool confirmHold; //当点击完成时键盘是否收起
        public string confirmType; //键盘右下角confirm按钮类型，只影响按钮的文本内容
    }

    [Serializable]
    public class QGFileResponse : QGBaseResponse
    {
        public string textStr;  // 读取的文本
        public byte[] textData; // 读取的二进制数据
        public string encoding;
        public int byteLength;
    }

    public class QGFileParam
    {
        public string fileName; //需要读取的本地文件，filePath = "internal://files/unity/" + fileName
        public string encoding = "utf8"; //编码格式, utf8，binary。默认 utf8
        public bool append = false;//是否是追加模式，非追加模式覆盖源文件，没有文件会生成新的文件，默认为 false，覆盖旧文件

        public string textStr; // 写入的文本
        public byte[] textData; // 写入的二进制数据
    }

    public class QGFileInfo
    {
        public string textStr;
        public byte[] textData;
    }

    public class QGDirParam
    {
        public string filePath; //创建的目录路径 (本地路径), dirPath = "internal://files/unity/" + filePath
        public bool recursive = false; //是否在递归创建该目录的上级目录后再创建该目录。
    }

    [Serializable]
    public class QGStatResponse
    {
        public bool isDirectory; //是否是目录
        public long size; //文件大小
        public string birthtime; //文件创建时间
        public string lastModifiedTime; //文件最后修改时间
    }

    [Serializable]
    public class QGCreateVideoParam
    {
        /// <summary>视频的左上角横坐标</summary>
        public int x = 0;
        /// <summary>视频的左上角纵坐标</summary>
        public int y = 0;
        /// <summary>视频的宽度</summary>
        public int width = 300;
        /// <summary>视频的高度</summary>
        public int height = 100;
        /// <summary>视频的资源地址</summary>
        public string src;
        /// <summary>视频的封面</summary>
        public string poster;
        /// <summary>视频的初始播放位置，单位为 s 秒</summary>
        public int initialTime = 0;
        /// <summary>视频的播放速率，有效值有 0.5、0.8、1.0、1.25、1.5</summary>
        public float playbackRate = 1.0f;
        /// <summary>视频是否为直播</summary>
        public bool live;
        /// <summary>视频的缩放模式</summary>
        public string objectFit = "contain";
        /// <summary>视频是否显示控件</summary>
        public bool controls = true;
        /// <summary>是否显示视频底部进度条</summary>
        public bool showProgress = true;
        /// <summary>是否显示控制栏的进度条</summary>
        public bool showProgressInControlMode = true;
        /// <summary>视频背景颜色</summary>
        public string backgroundColor = "#000000";
        /// <summary>视频是否自动播放</summary>
        public bool autoplay;
        /// <summary>视频是否是否循环播放</summary>
        public bool loop;
        /// <summary>视频是否禁音播放</summary>
        public bool muted;
        /// <summary>视频是否遵循系统静音开关设置（仅iOS）</summary>
        public bool obeyMuteSwitch;
        /// <summary>是否启用手势控制播放进度</summary>
        public bool enableProgressGesture = true;
        /// <summary>是否开启双击播放的手势</summary>
        public bool enablePlayGesture;
        /// <summary>是否显示视频中央的播放按钮</summary>
        public bool showCenterPlayBtn = true;
        /// <summary>视频是否显示在游戏画布之下</summary>
        public bool underGameView;
    }
}