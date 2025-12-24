using UnityEngine;
using mi;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 快游戏sdk接入demo
/// </summary>
public class QGSdkDemo : MonoBehaviour
{
    public RawImage rawImage;
    public InputField bannerAdInputField;
    public InputField InterstitialADInputField;
    public InputField RewardADInputField;
    public InputField CustomADInputField;
    public InputField RecommendADInputField;

    void Start()
    {
        StartCoroutine(LoadImage());
    }

    IEnumerator LoadImage()
    {
        UnityWebRequest webRequestTexture = UnityWebRequestTexture.GetTexture("https://pic.kts.g.mi.com/1578527291_1706859021");
        yield return webRequestTexture.SendWebRequest();

#if UNITY_2019
        if (webRequestTexture.isNetworkError || webRequestTexture.isHttpError)
#else
        if (webRequestTexture.result != UnityWebRequest.Result.Success)
#endif
        {
            Debug.LogError("网络请求出错: " + webRequestTexture.error);
        }
        else
        {
            Debug.Log("加载图片成功");
            Texture2D texture = DownloadHandlerTexture.GetContent(webRequestTexture);

            rawImage.texture = texture;
        }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void InitTest()
    {
        Debug.Log("点击初始化");
        MiBridge.Instance.Init();
    }

    /// <summary>
    /// 登录
    /// </summary>
    public void LoginTest()
    {
        Debug.Log("点击登录");
        MiBridge.Instance.Login((accountId, session) =>
        {
            MiBridge.Instance.QGLog("login account id={0}, session={1}", accountId, session);
            MiBridge.Instance.GetUserInfo((nickName, avatarUrl, gender) =>
            {
                MiBridge.Instance.QGLog("get user info nickName={0}, avatar={1}, gender={2}", nickName, avatarUrl, gender);
            }, (code, msg) =>
            {
                MiBridge.Instance.QGLog("get user info error code={0}, msg={1}", code, msg);
            });
        }, (code, msg) =>
        {
            MiBridge.Instance.QGLog("login error code={0}, msg={1}", code, msg);
        });
    }

    /// <summary>
    /// 支付测试
    /// </summary>
    public void PayTest()
    {
        Debug.Log("点击支付测试");
        OrderInfo orderInfo = new OrderInfo
        {
            appId = "1",
            appAccountId = 12,
            session = "1",
            cpOrderId = "1",
            displayName = "1",
            feeValue = 100,
            sign = "1",
        };
        MiBridge.Instance.Pay(orderInfo, (success, code, msg) =>
        {
            MiBridge.Instance.QGLog("pay result success={0}, code={1}, msg={2}", success, code, msg);
        });
    }

    /// <summary>
    /// banner广告创建测试
    /// </summary>
    public void CreateBannerAdTest()
    {
        Debug.Log("点击banner广告创建测试");
        //"f618f7543d0f768b1c58292b0616998c"
        string adUnitId = bannerAdInputField.text;
        MiBridge.Instance.CreateBannerAd(adUnitId, 0, 800, 1080, 300, new AdEventListener
        {
            onAdLoad = (info) => { MiBridge.Instance.QGLog("c# banner ad load"); },

            onAdClose = (isEnd) => { MiBridge.Instance.QGLog("c# banner ad close"); },

            onAdError = (code, msg) => { MiBridge.Instance.QGLog("c# banner ad error {0}", msg); },
        });
    }

    /// <summary>
    /// 展示banner广告测试
    /// </summary>
    public void ShowBannerAdTest()
    {
        Debug.Log("点击展示banner广告测试");
        string adUnitId = bannerAdInputField.text;
        MiBridge.Instance.ShowAd(adUnitId);
    }

    /// <summary>
    /// 隐藏banner广告测试
    /// </summary>
    public void HideBannerAdTest()
    {
        Debug.Log("点击隐藏banner广告测试");
        string adUnitId = bannerAdInputField.text;
        MiBridge.Instance.HideAd(adUnitId);
    }

    /// <summary>
    /// 销毁banner广告测试
    /// </summary>
    public void DestroyBannerAdTest()
    {
        Debug.Log("点击销毁banner广告测试");
        string adUnitId = bannerAdInputField.text;
        MiBridge.Instance.DestroyAd(adUnitId);
    }

    /// <summary>
    /// 创建插屏广告
    /// </summary>
    public void CreateInterstitialAdTest()
    {
        Debug.Log("点击创建插屏广告测试");
        //"6e969534de88080c3a1eda5d7049477a"
        string adUnitId = InterstitialADInputField.text;
        MiBridge.Instance.CreateInterstitialAd(adUnitId, new AdEventListener
        {
            onAdLoad = (info) => { MiBridge.Instance.QGLog("c# insert ad load"); },

            onAdClose = (end) => { MiBridge.Instance.QGLog("c# insert ad close"); },

            onAdError = (code, msg) => { MiBridge.Instance.QGLog("c# insert ad error"); }
        });
    }

    /// <summary>
    /// 展示插屏广告测试
    /// </summary>
    public void ShowInterstitialAdTest()
    {
        Debug.Log("点击展示插屏广告测试");
        string adUnitId = InterstitialADInputField.text;
        MiBridge.Instance.ShowAd(adUnitId);
    }

    /// <summary>
    /// 销毁插屏广告测试
    /// </summary>
    public void DestroyInterstitialAdTest()
    {
        Debug.Log("点击销毁插屏广告测试");
        string adUnitId = InterstitialADInputField.text;
        MiBridge.Instance.DestroyAd(adUnitId);
    }

    /// <summary>
    /// 创建激励视频广告测试
    /// </summary>
    public void CreateRewardedTest()
    {
        Debug.Log("点击创建激励视频广告测试");
        //"8ff5a54ea576d425124c8b179f1bd2ad"
        string adUnitId = RewardADInputField.text;
        MiBridge.Instance.CreateRewardedVideoAd(adUnitId, new AdEventListener
        {
            onAdLoad = (info) => { MiBridge.Instance.QGLog("c# reward ad load"); },

            onAdClose = (end) =>
            {
                string info = "success";
                if (!end)
                {
                    info = "failure";
                }

                MiBridge.Instance.QGLog($"c# reward ad close end={info}");
            },

            onAdError = (code, msg) => { MiBridge.Instance.QGLog("c# reward ad error"); },
        });
    }

    /// <summary>
    /// 加载激励视频广告测试
    /// </summary>
    public void LoadRewardedTest()
    {
        Debug.Log("点击加载激励视频广告测试");
        string adUnitId = RewardADInputField.text;
        MiBridge.Instance.LoadAd(adUnitId);
    }

    /// <summary>
    /// 展示激励视频广告测试
    /// </summary>
    public void ShowRewardAdTest()
    {
        Debug.Log("点击展示激励视频广告测试");
        string adUnitId = RewardADInputField.text;
        MiBridge.Instance.ShowAd(adUnitId);
    }

    /// <summary>
    /// 创建原生广告测试
    /// </summary>
    public void CreateCustomAdTest()
    {
        Debug.Log("点击创建原生广告测试");
        //"e9a171bfe65386da7ffecf5e5294733f"
        string adUnitId = CustomADInputField.text;
        MiBridge.Instance.CreateCustomAd(adUnitId, 0, 700, 1080, 300, new AdEventListener
        {
            onAdLoad = (info) => { MiBridge.Instance.QGLog("c# custom ad load"); },

            onAdClose = (end) => { MiBridge.Instance.QGLog("c# custom ad close"); },

            onAdError = (code, msg) => { MiBridge.Instance.QGLog("c# custom ad error"); }
        });
    }

    /// <summary>
    /// 展示原生广告测试
    /// </summary>
    public void ShowCustomAdTest()
    {
        Debug.Log("点击展示原生广告测试");
        string adUnitId = CustomADInputField.text;
        MiBridge.Instance.ShowAd(adUnitId);
    }

    /// <summary>
    /// 销毁原生广告测试
    /// </summary>
    public void DestroyCustomAdTest()
    {
        Debug.Log("点击销毁原生广告测试");
        string adUnitId = CustomADInputField.text;
        MiBridge.Instance.DestroyAd(adUnitId);
    }

    /// <summary>
    /// 展示互推盒子广告测试
    /// </summary>
    public void ShowRecommendAdTest()
    {
        Debug.Log("点击展示互推盒子广告测试");
        //"da11b7e8c582ee7d1acf16a627ea6b34"
        string adUnitId = RecommendADInputField.text;
        MiBridge.Instance.ShowRecommendAd(adUnitId, 100, (success, msg) =>
        {
            MiBridge.Instance.QGLog("c# recommend ad show:" + success + " msg:" + msg);
        });
    }

    /// <summary>
    /// 关闭互推盒子广告测试
    /// </summary>
    public void CloseRecommendAdTest()
    {
        Debug.Log("点击关闭互推盒子广告测试");
        string adUnitId = RecommendADInputField.text;
        MiBridge.Instance.CloseRecommendAd(adUnitId, 100, (success, msg) =>
        {
            MiBridge.Instance.QGLog("c# recommend ad close:" + success + " msg:" + msg);
        });
    }

    /// <summary>
    /// 写文件测试
    /// </summary>
    public void WriteFileTest()
    {
        Debug.Log("点击写文件测试");
        QGFileParam param = new QGFileParam
        {
            fileName = "log.txt",
            encoding = "utf8",
            textStr = "Hello world!!!",
            append = false
        };
        // param.encoding = "binary";
        // param.textData = Encoding.UTF8.GetBytes("Hello world!!!");
        MiBridge.Instance.WriteFile(param, (success, msg) =>
        {
            MiBridge.Instance.QGLog("write file {0}, {1}", success, msg);
        });
    }

    /// <summary>
    /// 检查文件存在测试
    /// </summary>
    public void AccessFileTest()
    {
        Debug.Log("点击检查文件存在测试");
        MiBridge.Instance.AccessFile("log.txt", (exists) =>
        {
            MiBridge.Instance.QGLog("exists file {0}", exists);
        });
    }

    /// <summary>
    /// 读文件测试
    /// </summary>
    public void ReadFileTest()
    {
        Debug.Log("点击读文件测试");
        QGFileParam param = new QGFileParam
        {
            fileName = "log.txt",
            encoding = "utf8"
        };
        // param.encoding = "binary";
        MiBridge.Instance.ReadFile(param, (response) =>
        {
            MiBridge.Instance.QGLog("MiBridge.Instance.ReadFile success " + JsonUtility.ToJson(response));
            // string test = System.Text.Encoding.UTF8.GetString(response.textData);
            // MiBridge.Instance.QGLog(test);
        }, (msg) =>
        {
            MiBridge.Instance.QGLog("read file failure {0}", msg);
        });
    }

    /// <summary>
    /// 获取文件 Stats 对象测试
    /// </summary>
    public void StatTest()
    {
        Debug.Log("点击获取文件 Stats 对象测试");
        QGDirParam param = new QGDirParam
        {
            filePath = "log.txt"
        };
        
        MiBridge.Instance.Stat(param, (response, err) =>
        {
            MiBridge.Instance.QGLog("Stat: {0} {1}", JsonUtility.ToJson(response), err);
        });
    }

    /// <summary>
    /// 在文件结尾追加内容测试
    /// </summary>
    public void AppendFileTest()
    {
        Debug.Log("点击在文件结尾追加内容测试");
        QGFileParam param = new QGFileParam
        {
            fileName = "log.txt",
            encoding = "utf8",
            textStr = "Hello world!!!",
        };
        // param.encoding = "binary";
        // param.textData = Encoding.UTF8.GetBytes("Hello world!!!");
        MiBridge.Instance.AppendFile(param, (success, err) =>
        {
            MiBridge.Instance.QGLog("appendFile {0} {1}", success, err);
        });
    }

    /// <summary>
    /// 复制文件测试
    /// </summary>
    public void CopyFileTest()
    {
        Debug.Log("点击复制文件测试");
        string srcPath = "log.txt";
        string destPath = "logfuben.txt";
        MiBridge.Instance.CopyFile(srcPath, destPath, (success, err) =>
        {
            MiBridge.Instance.QGLog("copyFile {0} {1}", success, err);
        });
        
        bool fileAccess = MiBridge.Instance.AccessFileSync("logfuben.txt");
        MiBridge.Instance.QGLog("logfuben AccessFileSync:" + fileAccess);
    }

    /// <summary>
    /// 删除文件测试
    /// </summary>
    public void DeleteFileTest()
    {
        Debug.Log("点击删除文件测试");
        MiBridge.Instance.DeleteFile("log.txt", (success) =>
        {
            MiBridge.Instance.QGLog("delete file {0}", success);
        });
    }

    /// <summary>
    /// 创建目录测试
    /// </summary>
    public void MkDirTest()
    {
        Debug.Log("点击创建目录测试");
        QGDirParam param = new QGDirParam
        {
            filePath = "log"
        };

        MiBridge.Instance.MkDir(param, (success, err) =>
        {
            MiBridge.Instance.QGLog("mkdir {0} {1}", success, err);
        });

        bool fileAccess = MiBridge.Instance.AccessFileSync("log");
        MiBridge.Instance.QGLog("AccessFileSync:" + fileAccess);
    }

    /// <summary>
    /// 删除目录测试
    /// </summary>
    public void DeleteDirTest()
    {
        Debug.Log("点击删除目录测试");
        QGDirParam param = new QGDirParam
        {
            filePath = "log",
            recursive = true
        };
        
        MiBridge.Instance.DeleteDir(param,(success, err) =>
        {
            MiBridge.Instance.QGLog("rmdir {0} {1}", success, err);
        });
        
        bool fileAccess = MiBridge.Instance.AccessFileSync("log");
        MiBridge.Instance.QGLog("AccessFileSync:" + fileAccess);
    }

    /// <summary>
    /// 同步写文件测试
    /// </summary>
    public void WriteFileSyncTest()
    {
        Debug.Log("点击同步写文件测试");
        QGFileParam param = new QGFileParam
        {
            fileName = "log.txt",
            encoding = "utf8",  // "binary"
            textStr = "Hello world!!!",
            // textData = Encoding.UTF8.GetBytes("Hello world!!!"),
            append = false
        };
        bool res = MiBridge.Instance.WriteFileSync(param);
        MiBridge.Instance.QGLog("WriteFileSyncTest:" + res);
    }

    /// <summary>
    /// 同步检查文件存在测试
    /// </summary>
    public void AccessFileSyncTest()
    {
        Debug.Log("点击同步检查文件存在测试");
        bool fileAccess = MiBridge.Instance.AccessFileSync("log.txt");
        MiBridge.Instance.QGLog("AccessFileSync:" + fileAccess);
    }

    /// <summary>
    /// 同步读文件测试
    /// </summary>
    public void ReadFileSyncTest()
    {
        Debug.Log("点击同步读文件测试");
        QGFileParam param = new QGFileParam
        {
            fileName = "log.txt",
            encoding = "utf8"  // "binary"
        };
        QGFileInfo fileInfo = MiBridge.Instance.ReadFileSync(param);
        // string str = Encoding.UTF8.GetString(fileInfo.textData);
        MiBridge.Instance.QGLog("ReadFileSync:" + JsonUtility.ToJson(fileInfo));
        // MiBridge.Instance.QGLog("str:" + str);
    }

    /// <summary>
    /// 同步获取文件 Stats 对象测试
    /// </summary>
    public void StatSyncTest()
    {
        Debug.Log("点击同步获取文件 Stats 对象测试");
        QGDirParam param = new QGDirParam
        {
            filePath = "log.txt",
            recursive = false
        };
        QGStatResponse response = MiBridge.Instance.StatSync(param);
        MiBridge.Instance.QGLog("StatSync:" + JsonUtility.ToJson(response));
    }

    /// <summary>
    /// 同步在文件结尾追加内容测试
    /// </summary>
    public void AppendFileSyncTest()
    {
        Debug.Log("点击同步在文件结尾追加内容测试");
        QGFileParam param = new QGFileParam
        {
            fileName = "log.txt",
            encoding = "utf8",
            textStr = "Hello world!!!",
        };
        // param.encoding = "binary";
        // param.textData = Encoding.UTF8.GetBytes("Hello world!!!");
        MiBridge.Instance.AppendFileSync(param);
    }

    /// <summary>
    /// 同步复制文件测试
    /// </summary>
    public void CopyFileSyncTest()
    {
        Debug.Log("点击同步复制文件测试");
        string srcPath = "log.txt";
        string destPath = "logfuben.txt";
        bool res = MiBridge.Instance.CopyFileSync(srcPath, destPath);
        MiBridge.Instance.QGLog("CopyFileSyncTest:" + res);
        
        bool fileAccess = MiBridge.Instance.AccessFileSync("logfuben.txt");
        MiBridge.Instance.QGLog("logfuben AccessFileSync:" + fileAccess);
    }

    /// <summary>
    /// 同步删除文件测试
    /// </summary>
    public void DeleteFileSyncTest()
    {
        Debug.Log("点击同步删除文件测试");
        MiBridge.Instance.DeleteFileSync("log.txt");
    }

    /// <summary>
    /// 同步创建目录测试
    /// </summary>
    public void MkDirSyncTest()
    {
        Debug.Log("点击同步创建目录测试");
        QGDirParam param = new QGDirParam
        {
            filePath = "log",
            recursive = true
        };
        MiBridge.Instance.MkDirSync(param);

        bool fileAccess = MiBridge.Instance.AccessFileSync("log");
        MiBridge.Instance.QGLog("AccessFileSync:" + fileAccess);
    }

    /// <summary>
    /// 同步删除目录测试
    /// </summary>
    public void DeleteDirSyncTest()
    {
        Debug.Log("点击同步删除目录测试");
        QGDirParam param = new QGDirParam
        {
            filePath = "log",
            recursive = true
        };
        MiBridge.Instance.DeleteDirSync(param);

        bool fileAccess = MiBridge.Instance.AccessFileSync("log");
        MiBridge.Instance.QGLog("AccessFileSync:" + fileAccess);
    }

    /// <summary>
    /// 快游戏图标测试
    /// </summary>
    public void ShortcutTest()
    {
        Debug.Log("点击快游戏图标测试");
        MiBridge.Instance.HasShortcut(has =>
        {
            if (has)
            {
                MiBridge.Instance.QGLog("has short cut");
            }
            else
            {
                MiBridge.Instance.QGLog("has not short cut");
                MiBridge.Instance.CreateShortcut("便于打开游戏", (success, code, msg) =>
                {
                    MiBridge.Instance.QGLog("create short cut {0}, {1}, {2}", success, code, msg);
                });
            }
        });
    }

    /// <summary>
    /// Key-Value存储测试
    /// </summary>
    public void SetKeyValueTest()
    {
        Debug.Log("点击Key-Value存储测试");
        MiBridge.Instance.QGLog("set key-value");

        MiBridge.Instance.SetKVInt("int", 1);
        if (MiBridge.Instance.HasKV("int"))
        {
            MiBridge.Instance.QGLog("int key 存在！！！");
        }
        else
        {
            MiBridge.Instance.QGLog("int key 不存在！！！");
        }

        MiBridge.Instance.SetKVFloat("float", 2.0522222f);
        if (MiBridge.Instance.HasKV("float"))
        {
            MiBridge.Instance.QGLog("float key 存在！！！");
        }
        else
        {
            MiBridge.Instance.QGLog("float key 不存在！！！");
        }

        MiBridge.Instance.SetKVDouble("dobule", 3.2656453453d);
        if (MiBridge.Instance.HasKV("dobule"))
        {
            MiBridge.Instance.QGLog("dobule key 存在！！！");
        }
        else
        {
            MiBridge.Instance.QGLog("dobule key 不存在！！！");
        }

        MiBridge.Instance.SetKVString("string", " fsdfsdf ");
        if (MiBridge.Instance.HasKV("string"))
        {
            MiBridge.Instance.QGLog("string key 存在！！！");
        }
        else
        {
            MiBridge.Instance.QGLog("string key 不存在！！！");
        }
    }

    /// <summary>
    /// Key-Value获取测试
    /// </summary>
    public void GetKeyValueTest()
    {
        Debug.Log("点击Key-Value获取测试");
        MiBridge.Instance.QGLog("get key-value");

        var kvInt = MiBridge.Instance.GetKVInt("int");
        MiBridge.Instance.QGLog("get int kv={0}", kvInt);

        var kvInt1 = MiBridge.Instance.GetKVInt("int1");
        MiBridge.Instance.QGLog("get int1 kv={0}", kvInt1);

        var kvFloat = MiBridge.Instance.GetKVFloat("float");
        MiBridge.Instance.QGLog("get float kv={0}", kvFloat);

        var kvDouble = MiBridge.Instance.GetKVDouble("dobule");
        MiBridge.Instance.QGLog("get dobule kv={0}", kvDouble);

        var kvString = MiBridge.Instance.GetKVString("string");
        MiBridge.Instance.QGLog("get string kv={0}", kvString);

        var kvString111 = MiBridge.Instance.GetKVString("string1111");
        MiBridge.Instance.QGLog("get string111 kv={0}", kvString111);
    }

    /// <summary>
    /// 删除KV测试
    /// </summary>
    public void DeleteKVTest()
    {
        Debug.Log("点击删除KV测试");
        MiBridge.Instance.QGLog("delete key-value");

        MiBridge.Instance.DeleteKV("int");
        var kvInt = MiBridge.Instance.GetKVInt("int");
        MiBridge.Instance.QGLog("delete get int kv={0}", kvInt);
    }

    /// <summary>
    /// 删除所有kv测试
    /// </summary>
    public void DeleteAllKV()
    {
        Debug.Log("点击删除所有kv测试");
        MiBridge.Instance.DeleteAllKV();
        if (MiBridge.Instance.HasKV("string"))
        {
            MiBridge.Instance.QGLog("string key 存在！！！");
        }
        else
        {
            MiBridge.Instance.QGLog("string key 不存在！！！");
        }

        if (MiBridge.Instance.HasKV("string1"))
        {
            MiBridge.Instance.QGLog("string1 key 存在！！！");
        }
        else
        {
            MiBridge.Instance.QGLog("string1 key 不存在！！！");
        }
    }

    /// <summary>
    /// HTTP GET请求测试
    /// </summary>
    public void HttpGetTest()
    {
        Debug.Log("点击HTTP GET请求测试");
        MiBridge.Instance.HttpGet("https://jsonplaceholder.typicode.com/posts?", "{ \"Accept\": \"application/json\" }", "{ \"userId\": 1, \"_limit\": 3 }",
            (success, error, data) =>
            {
                MiBridge.Instance.QGLog("http get {0}, {1} {2}", success, error, data);
            });
    }

    /// <summary>
    /// HTTP POST请求测试
    /// </summary>
    public void HttpPostTest()
    {
        Debug.Log("点击HTTP POST请求测试");
        MiBridge.Instance.HttpPost("https://jsonplaceholder.typicode.com/posts", "{ \"Content-Type\": \"application/json\" }", "{ \"title\": \"foo\", \"body\": \"bar\", \"userId\": 1 }",
            (success, error, data) =>
            {
                MiBridge.Instance.QGLog("http post {0}, {1}, {2}", success, error, data);
            }, 3000);
    }

    /// <summary>
    /// 设置剪贴板测试
    /// </summary>
    public void SetClipTest()
    {
        Debug.Log("点击设置剪贴板测试");
        MiBridge.Instance.SetCilpBoardData("Hello world!", (success) =>
        {
            MiBridge.Instance.QGLog("set clipData {0}", success);
        });
    }

    /// <summary>
    /// 获取剪贴板测试
    /// </summary>
    public void GetClipTest()
    {
        Debug.Log("点击获取剪贴板测试");
        MiBridge.Instance.GetCilpBoardData((success, data) =>
        {
            MiBridge.Instance.QGLog("get clipData {0} {1}", success, data);
        });
    }

    /// <summary>
    /// 获取系统信息测试
    /// </summary>
    public void GetSystemInfo()
    {
        Debug.Log("点击获取系统信息测试");
        MiBridge.Instance.GetSystemInfo((success, data) =>
        {
            MiBridge.Instance.QGLog("get system info {0}: brand:{1}, model:{2}, lagunage:{3}", success, data.brand, data.model, data.language);
        });
    }

    /// <summary>
    /// 同步获取系统信息测试
    /// </summary>
    public void GetSystemInfoSync()
    {
        Debug.Log("点击同步获取系统信息测试");
        mi.SystemInfo systemInfo = MiBridge.Instance.GetSystemInfoSync();
        MiBridge.Instance.QGLog("get system info sync: brand:{0}, model:{1}, lagunage:{2}", systemInfo.brand, systemInfo.model, systemInfo.language);
    }

    /// <summary>
    /// 获取菜单按钮（右上角胶囊按钮）的布局位置信息测试
    /// </summary>
    public void GetMenuButtonBoundingClientRect()
    {
        Debug.Log("点击获取菜单按钮（右上角胶囊按钮）的布局位置信息测试");
        MenuButtonBoundingClientRect menuButtonBoundingClientRect = MiBridge.Instance.GetMenuButtonBoundingClientRect();
        MiBridge.Instance.QGLog("get menu button rect: width:{0}, height:{1}", menuButtonBoundingClientRect.width, menuButtonBoundingClientRect.height);
    }

    /// <summary>
    /// 设置屏幕帧率测试
    /// </summary>
    public void SetFPSTest()
    {
        Debug.Log("点击设置屏幕帧率测试");
        MiBridge.Instance.SetScreenFPS(25);
    }

    /// <summary>
    /// 播放音频测试
    /// </summary>
    public void PlayAudioTest()
    {
        Debug.Log("点击播放音频测试");
        StopAudio();
        PlayInnerAudio();
        Invoke("PlayAudio", 5);
    }

    void StopAudio()
    {
        MiBridge.Instance.QGLog("====Stop Audio=====");
        var audioSource = transform.GetComponent<AudioSource>();
        audioSource.Stop();
    }

    void PlayAudio()
    {
        MiBridge.Instance.QGLog("====Play Audio=====");
        var audioSource = transform.GetComponent<AudioSource>();
        audioSource.Play();
    }

    void PlayInnerAudio()
    {
        var player = MiAudioPlayer.PlayAudio(new AudioParam
        {
            url = "https://p2ph5dl3.9917.com/p2ph5_cn_cn/S583509901/HttpRoot/streaming/Assets/Audios/4355804_691d230b68a2ec41b20aa4c414876e9b.mp3",
            startTime = 0,
            loop = false,
            volume = 1,
            autoplay = false,
        });

        player.Stop();
    }

    /// <summary>
    /// 展示软键盘测试
    /// </summary>
    public void ShowKeyBoardTest()
    {
        Debug.Log("点击展示软键盘测试");
        MiKeyBoard.Instance.ShowKeyboard(new KeyboardParam()
        {
            defaultValue = "defaultValue",
            maxLength = 200,
            multiple = false,
            confirmHold = true,
            confirmType = "done"
        });

        MiKeyBoard.Instance.OnKeyboardInput((text) =>
        {
            MiBridge.Instance.QGLog($"[MiKeyBoard] input text {text}");
        });

        MiKeyBoard.Instance.OnKeyboardConfirm((text) =>
        {
            MiBridge.Instance.QGLog($"[MiKeyBoard] confirm text {text}");
        });

        MiKeyBoard.Instance.OnKeyboardComplete((text) =>
        {
            MiBridge.Instance.QGLog($"[MiKeyBoard] OnKeyboardComplete Success {text}");
        });

        Invoke("HideKeyboardTest", 60);
    }

    /// <summary>
    /// 隐藏软键盘测试
    /// </summary>
    public void HideKeyboardTest()
    {
        MiKeyBoard.Instance.HideKeyboard();
    }

    /// <summary>
    /// 退出当前小游戏
    /// </summary>
    public void ExitAppTest()
    {
        Debug.Log("点击退出当前小游戏测试");
        MiBridge.Instance.ExitApp((success) =>
        {
            MiBridge.Instance.QGLog("exit app {0}", success);
        });
    }

    /// <summary>
    /// 监听游戏切入前台事件测试
    /// </summary>
    public void GetOptionsOnShowTest()
    {
        Debug.Log("点击监听游戏切入前台事件测试");

        MiGetOptions.Instance.OnShow((options) =>
        {
            MiBridge.Instance.QGLog($"[MiGetOptions] OnShow: {options}");
        });
    }

    /// <summary>
    /// 监听游戏切入后台事件测试
    /// </summary>
    public void GetOptionsOnHideTest()
    {
        Debug.Log("点击监听游戏切入后台事件测试");

        MiGetOptions.Instance.OnHide(() =>
        {
            MiBridge.Instance.QGLog("[MiGetOptions] OnHide");
        });
    }

    /// <summary>
    /// 取消监听游戏切入前台测试
    /// </summary>
    public void GetOptionsOffShowTest()
    {
        Debug.Log("点击取消监听游戏切入前台测试");

        MiGetOptions.Instance.OffShow();
    }

    /// <summary>
    /// 取消监听游戏切入后台事件测试
    /// </summary>
    public void GetOptionsOffHideTest()
    {
        Debug.Log("点击取消监听游戏切入后台事件测试");

        MiGetOptions.Instance.OffHide();
    }

    /// <summary>
    /// 获取快游戏启动时的参数测试
    /// </summary>
    public void GetLaunchOptionsSyncTest()
    {
        Debug.Log("点击获取快游戏启动时的参数测试");
        QGLaunchInfo launchInfo = MiGetOptions.Instance.GetLaunchOptionsSync();
        MiBridge.Instance.QGLog("GetLaunchOptionsSync:" + JsonUtility.ToJson(launchInfo));
    }

    /// <summary>
    /// 主动检查更新快游戏测试
    /// </summary>
    public void UpdateGameTest()
    {
        Debug.Log("点击主动检查更新快游戏测试");
        MiBridge.Instance.UpdateGame();
    }

    private QGVideo _video;

    /// <summary>
    /// 创建视频测试
    /// </summary>
    public void CreateVideoTest()
    {
        Debug.Log("点击创建视频测试");

        _video = MiVideo.Instance.CreateVideo(new QGCreateVideoParam()
        {
            src =
                "https://media.w3.org/2010/05/sintel/trailer.mp4",
            controls = false,
            showProgress = false,
            showProgressInControlMode = false,
            autoplay = false,
            showCenterPlayBtn = false,
            underGameView = true,
            width = 400,
            height = 250,
        });

        _video.OnProgress(() =>
        {
            MiBridge.Instance.QGLog("video on progress");
            PlayVideoTest();
        });

        _video.OnPlay(() =>
        {
            MiBridge.Instance.QGLog("video on play");
        });

        _video.OnPause(() =>
        {
            MiBridge.Instance.QGLog("video on pause");
        });

        _video.OnEnded(() =>
        {
            MiBridge.Instance.QGLog("video on ended");
        });

        _video.OnTimeUpdate(() =>
        {
            MiBridge.Instance.QGLog("video on time update");
        });

        _video.OnError((string errMsg) =>
        {
            MiBridge.Instance.QGLog("video on error, errMsg: ", errMsg);
        });

        _video.OnWaiting(() =>
        {
            MiBridge.Instance.QGLog("video on waiting");
        });
    }

    /// <summary>
    /// 播放视频测试
    /// </summary>
    public void PlayVideoTest()
    {
        Debug.Log("点击播放视频测试");

        _video.Play();
    }

    /// <summary>
    /// 暂停视频测试
    /// </summary>
    public void PauseVideoTest()
    {
        Debug.Log("点击暂停视频测试");

        _video.Pause();
    }

    /// <summary>
    /// 停止视频测试
    /// </summary>
    public void StopVideoTest()
    {
        Debug.Log("点击停止视频测试");

        _video.Stop();
    }

    /// <summary>
    /// 视频跳转测试
    /// </summary>
    public void SeekVideoTest()
    {
        Debug.Log("点击视频跳转测试");

        _video.Seek(5.0f);
    }

    /// <summary>
    /// 视频全屏测试
    /// </summary>
    public void RequestFullScreenVideoTest()
    {
        Debug.Log("点击视频全屏测试");

        _video.RequestFullScreen(90);

        Invoke("ExitFullScreenVideoTest", 5);
    }

    /// <summary>
    /// 视频退出全屏测试
    /// </summary>
    private void ExitFullScreenVideoTest()
    {
        Debug.Log("点击视频退出全屏测试");

        _video.ExitFullScreen();
    }

    /// <summary>
    /// 销毁视频测试
    /// </summary>
    public void DestroyVideoTest()
    {
        Debug.Log("点击销毁视频测试");

        _video.Destroy();
    }

    #region 触摸

    private System.Action<OnTouchListenerResult> onTouchStartHandler = result =>
    {
        MiBridge.Instance.QGLog($"TouchStart: " + JsonUtility.ToJson(result));
    };

    /// <summary>
    /// 监听触摸开始事件测试
    /// </summary>
    public void OnTouchStartTest()
    {
        Debug.Log("点击监听触摸开始事件测试");

        MiTouch.Instance.OnTouchStart(onTouchStartHandler);
    }

    /// <summary>
    /// 取消监听触摸开始事件测试
    /// </summary>
    public void OffTouchStartTest()
    {
        Debug.Log("点击取消监听触摸开始事件测试");

        MiTouch.Instance.OffTouchStart(onTouchStartHandler);
    }

    private System.Action<OnTouchListenerResult> onTouchMoveHandler = result =>
    {
        MiBridge.Instance.QGLog($"TouchMove: " + JsonUtility.ToJson(result));
    };

    /// <summary>
    /// 监听触摸移动事件测试
    /// </summary>
    public void OnTouchMoveTest()
    {
        Debug.Log("点击监听触摸移动事件测试");
        MiTouch.Instance.OnTouchMove(onTouchMoveHandler);
    }

    /// <summary>
    /// 取消监听触摸移动事件测试
    /// </summary>
    public void OffTouchMoveTest()
    {
        Debug.Log("点击取消监听触摸移动事件测试");
        MiTouch.Instance.OffTouchMove(onTouchMoveHandler);
    }

    private System.Action<OnTouchListenerResult> onTouchEndHandler = result =>
    {
        MiBridge.Instance.QGLog($"TouchEnd: " + JsonUtility.ToJson(result));
    };

    /// <summary>
    /// 监听触摸结束事件测试
    /// </summary>
    public void OnTouchEndTest()
    {
        Debug.Log("点击监听触摸结束事件测试");
        MiTouch.Instance.OnTouchEnd(onTouchEndHandler);
    }

    /// <summary>
    /// 取消监听触摸结束事件测试
    /// </summary>
    public void OffTouchEndTest()
    {
        Debug.Log("点击取消监听触摸结束事件测试");
        MiTouch.Instance.OffTouchEnd(onTouchEndHandler);
    }

    private System.Action<OnTouchListenerResult> onTouchCancelHandler = result =>
    {
        MiBridge.Instance.QGLog($"TouchCancel: " + JsonUtility.ToJson(result));
    };

    /// <summary>
    /// 监听触点失效事件测试
    /// </summary>
    public void OnTouchCancelTest()
    {
        Debug.Log("点击监听触点失效事件测试");
        MiTouch.Instance.OnTouchCancel(onTouchCancelHandler);
    }

    /// <summary>
    /// 取消监听触点失效事件测试
    /// </summary>
    public void OffTouchCancelTest()
    {
        Debug.Log("点击取消监听触点失效事件测试");
        MiTouch.Instance.OffTouchCancel(onTouchCancelHandler);
    }

    #endregion
}
