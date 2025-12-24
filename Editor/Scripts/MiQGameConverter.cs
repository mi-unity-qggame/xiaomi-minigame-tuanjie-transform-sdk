#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using System.Security.Cryptography;

namespace Mi
{
    public class MiQGameConverter : EditorWindow
    {
        private const string CURRETN_VERSION = "1.5.9";
        private const string KEY_MI_QGAME_NAME = "key_mi_qgame_name";
        private const string KEY_MI_QGAME_PACKAGE = "key_mi_qgame_package";
        private const string KEY_MI_QGAME_VERSION = "key_mi_qgame_version";
        private const string KEY_MI_QGAME_VERSION_CODE = "key_mi_qgame_version_code";
        private const string KEY_MI_QGAME_BUILD_PATH = "key_mi_qgame_build_path";
        private const string KEY_MI_QGAME_BUILD_MODE = "key_mi_qgame_build_mode";
        private const string KEY_MI_QGAME_ORIENTATION = "key_mi_qgame_orientation";
        private const string KEY_MI_QGAME_SIGN_PATH = "key_mi_qgame_sign_path";
        private const string KEY_MI_QGAME_DATA_PATH = "key_mi_qgame_data_path";
        private const string KEY_MI_QGAME_WASM_PATH = "key_mi_qgame_wasm_path";
        private const string KEY_MI_QGAME_STREAM_PATH = "key_mi_qgame_stream_path";
        private const string KEY_MI_QGAME_STREAM_DATA = "key_mi_qgame_stream_data";
        private const string KEY_MI_QGAME_OPENSSL_PROVIDER = "key_mi_qgame_openssl_provider";
        private const string KEY_MI_QGAME_LOADING_BG = "key_mi_qgame_loading_bg";
        private const string KEY_MI_QGAME_BROTLI_WASM = "key_mi_qgame_brotli_wasm";
        private const string KEY_MI_QGAME_BROTLI_DATA  = "key_mi_qgame_brotli_data";
        private const string KEY_MI_QGAME_WEBGL2 = "key_mi_qgame_webgl2";

        private static string sGameName = "";
        private static string sPackageName = "";
        private static string sVersion = "";
        private static int sVersionCode = 0;
        private static string sLoadingBG = "";
        private static string sBuildPath = "";
        private static string sChooseBuildPath = "";
        private static bool sBuildPathBtn = false;
        private static bool sRelease = false;
        private static bool sPortrait = true;
        private static bool sNodeJS = false;
        private static string sSignPath = "";
        private static bool sSignBtn = false;
        private static bool sIsBuild = false;
        private static string sDataPath = "";
        private static string sWasmPath = "";
        private static bool sStreamRes = false;
        private static string sStreamAssetPath = "";
        private static bool sRemoveOpenSSLProvider = false;
        private Vector2 scrollRoot;
        private static bool sBrotliWasm = false;
        private static bool sBrotliData = false;
        private static bool sBuilding = false;
        private static int sBuildingCount = 0;
        private static bool isAutoHideLoading = true;
        private static bool sIsWebGL2 = false;
        private static string sRelativeSdkPath = string.Empty; // XMSDK

        private static void LoadData()
        {
            if (string.IsNullOrEmpty(sGameName))
            {
                sGameName = PlayerPrefs.GetString(KEY_MI_QGAME_NAME, "");
            }

            if (string.IsNullOrEmpty(sPackageName))
            {
                sPackageName = PlayerPrefs.GetString(KEY_MI_QGAME_PACKAGE, "");
            }

            if (string.IsNullOrEmpty(sVersion))
            {
                sVersion = PlayerPrefs.GetString(KEY_MI_QGAME_VERSION, "");
            }

            if (sVersionCode == 0)
            {
                sVersionCode = PlayerPrefs.GetInt(KEY_MI_QGAME_VERSION_CODE, 0);
            }

            if (string.IsNullOrEmpty(sChooseBuildPath))
            {
                sChooseBuildPath = PlayerPrefs.GetString(KEY_MI_QGAME_BUILD_PATH, "");
            }

            if (string.IsNullOrEmpty(sSignPath))
            {
                sSignPath = PlayerPrefs.GetString(KEY_MI_QGAME_SIGN_PATH, "");
            }

            if (string.IsNullOrEmpty(sDataPath))
            {
                sDataPath = PlayerPrefs.GetString(KEY_MI_QGAME_DATA_PATH, "");
            }

            if (string.IsNullOrEmpty(sWasmPath))
            {
                sWasmPath = PlayerPrefs.GetString(KEY_MI_QGAME_WASM_PATH, "");
            }

            if (string.IsNullOrEmpty(sStreamAssetPath))
            {
                sStreamAssetPath = PlayerPrefs.GetString(KEY_MI_QGAME_STREAM_PATH, "");
            }

            if (string.IsNullOrEmpty(sLoadingBG))
            {
                sLoadingBG = PlayerPrefs.GetString(KEY_MI_QGAME_LOADING_BG, "");
            }

            sStreamRes = PlayerPrefs.GetInt(KEY_MI_QGAME_STREAM_DATA, 0) == 1;
            sRelease = PlayerPrefs.GetInt(KEY_MI_QGAME_BUILD_MODE, 0) == 1; // 0 debug, 1 release
            sPortrait = PlayerPrefs.GetInt(KEY_MI_QGAME_ORIENTATION, 0) == 1; // 0 landscape, 1 portrait
            sRemoveOpenSSLProvider = PlayerPrefs.GetInt(KEY_MI_QGAME_OPENSSL_PROVIDER, 0) == 1; //0reserve 1remove
            sBrotliWasm = PlayerPrefs.GetInt(KEY_MI_QGAME_BROTLI_WASM, 0) == 1; //0 no 1 brotli
            sBrotliData = PlayerPrefs.GetInt(KEY_MI_QGAME_BROTLI_DATA, 0) == 1;
            sIsWebGL2 = PlayerPrefs.GetInt(KEY_MI_QGAME_WEBGL2, 0) == 1;
        }

        [MenuItem("小米快游戏/转换快游戏", false, 1)]
        public static void ShowWindow()
        {
            LoadData();
            Rect rect = new Rect(0, 0, 650, 800);
            MiQGameConverter window = (MiQGameConverter)GetWindowWithRect(typeof(MiQGameConverter), rect, true, "小米快游戏转换工具");
            window.Show();
            // GetVersionInfo();
        }

        // 打包面板UI
        private void OnGUI()
        {
            scrollRoot = EditorGUILayout.BeginScrollView(scrollRoot);
            var labelStyle = new GUIStyle(EditorStyles.boldLabel);
            labelStyle.alignment = TextAnchor.MiddleLeft;
            GUILayout.Label("工具信息", labelStyle);
            EditorGUILayout.BeginVertical("frameBox");
            GUILayout.Label("版本号：" + CURRETN_VERSION);
            GUILayout.BeginHorizontal();
            GUILayout.Label("检查升级：", GUILayout.Width(60));
            versionLog = "请查看工具使用说明";
            GUILayout.Label(versionLog);
            GUILayout.EndHorizontal();
            GUILayout.Label("转换工具适配Unity版本: 2022.3.50f1c1，2021.3.20f1，2021.3.14f1，2020.3.47f1，2019.4.35f1c1");
            GUILayout.Label("工具适配团结引擎版本: Tuanjie 1.x");
            GUILayout.BeginHorizontal();
            GUILayout.Label("小米快游戏介绍");
            var info = GUILayout.Button("链接", GUILayout.Width(100));
            if (info)
            {
                Application.OpenURL("https://dev.mi.com/xiaomihyperos/quickgame-develop");
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("工具使用说明");
            var userInfo = GUILayout.Button("链接", GUILayout.Width(100));
            if (userInfo)
            {
                Application.OpenURL("https://dev.mi.com/xiaomihyperos/documentation/detail?pId=2165");
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.Label("参数设置", labelStyle);
            EditorGUILayout.BeginVertical("frameBox");
            sGameName = EditorGUILayout.TextField("游戏名称", sGameName);
            sPackageName = EditorGUILayout.TextField("包名", sPackageName);
            EditorGUILayout.LabelField("                                                   小米快游测试包名：com.demo.ch.mini");
            sVersion = EditorGUILayout.TextField("版本号", sVersion);
            sVersionCode = EditorGUILayout.IntField("版本Code", sVersionCode);
            sLoadingBG = EditorGUILayout.TextField("自定义Loading图片链接", sLoadingBG);
            EditorGUILayout.LabelField("                                                   用于屏蔽空白页面，需要快游戏框架11110002及以上版本");
            GUILayout.Space(3);
            GUILayout.BeginHorizontal();
            // 路径框
            sChooseBuildPath = EditorGUILayout.TextField("输出目录", sChooseBuildPath);
            sBuildPathBtn = GUILayout.Button("选择", GUILayout.Width(100));
            if (sBuildPathBtn)
            {
                string path = EditorUtility.SaveFolderPanel("选择输出目录", sChooseBuildPath, "");
                if (!string.IsNullOrEmpty(path))
                {
                    sChooseBuildPath = path;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(3);
            GUILayout.Label("游戏屏幕方向", labelStyle);
            GUILayout.BeginHorizontal();
            sPortrait = EditorGUILayout.BeginToggleGroup("横屏", sPortrait);
            EditorGUILayout.EndToggleGroup();
            sPortrait = EditorGUILayout.BeginToggleGroup("竖屏", !sPortrait);
            EditorGUILayout.EndToggleGroup();
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            GUILayout.Label("WebGL版本", labelStyle);
            GUILayout.BeginHorizontal();
            sIsWebGL2 = EditorGUILayout.BeginToggleGroup("WebGL 1.0", sIsWebGL2);
            EditorGUILayout.EndToggleGroup();
            sIsWebGL2 = EditorGUILayout.BeginToggleGroup("WebGL 2.0", !sIsWebGL2);
            EditorGUILayout.EndToggleGroup();
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            GUILayout.Label("NodeJS版本（如果您的NodeJS版本高于17.9.1，请选择高版本）", labelStyle);
            GUILayout.BeginHorizontal();
            sRemoveOpenSSLProvider = EditorGUILayout.BeginToggleGroup("高版本(NodeJS 17.9.1+)", sRemoveOpenSSLProvider);
            EditorGUILayout.EndToggleGroup();
            sRemoveOpenSSLProvider = EditorGUILayout.BeginToggleGroup("低版本", !sRemoveOpenSSLProvider);
            EditorGUILayout.EndToggleGroup();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.Label("包类型", labelStyle);

            GUILayout.BeginHorizontal();
            sRelease = EditorGUILayout.BeginToggleGroup("Debug", sRelease);
            EditorGUILayout.EndToggleGroup();
            sRelease = EditorGUILayout.BeginToggleGroup("Release", !sRelease);
            EditorGUILayout.EndToggleGroup();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            sSignPath = EditorGUILayout.TextField("签名文件根目录", sSignPath);
            sSignBtn = GUILayout.Button("选择", GUILayout.Width(100));
            if (sSignBtn)
            {
                string path = EditorUtility.OpenFolderPanel("选择签名文件目录", sSignPath, "");
                if (!string.IsNullOrEmpty(path))
                {
                    sSignPath = path;
                }
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.LabelField("注意：签名文件生成方式，请查看小米快游戏介绍文档！");
            EditorGUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.Label("资源说明", labelStyle);
            EditorGUILayout.BeginVertical("frameBox");
            GUILayout.Label("如果包体过大，请把打包后的Build/build.data或者Build/build.wasm放在网络上加载", labelStyle);

            sBrotliData = EditorGUILayout.BeginToggleGroup(new GUIContent("data brotli压缩(?)", "使用brotli压缩data文件，网络加载会强制开启"), sBrotliData);
            EditorGUILayout.EndToggleGroup();

            GUILayout.BeginHorizontal();
            sDataPath = EditorGUILayout.TextField("build.data地址(http)", sDataPath);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            sBrotliWasm = EditorGUILayout.BeginToggleGroup(new GUIContent("wasm brotli压缩(?)", "使用brotli压缩wasm文件，网络加载会强制开启"), sBrotliWasm);
            EditorGUILayout.EndToggleGroup();

            GUILayout.BeginHorizontal();
            sWasmPath = EditorGUILayout.TextField("build.wasm地址(http)", sWasmPath);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            sStreamRes = EditorGUILayout.BeginToggleGroup("StreamingAssets", sStreamRes);
            GUILayout.BeginHorizontal();
            sStreamAssetPath = EditorGUILayout.TextField("资源地址(http)", sStreamAssetPath);
            GUILayout.EndHorizontal();
            EditorGUILayout.EndToggleGroup();

            EditorGUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.Label("转换说明", labelStyle);
            EditorGUILayout.BeginVertical("frameBox");
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("(1) 生成快游戏rpk，需要版本nodejs，请确定已安装! ");
            sNodeJS = GUILayout.Button("安装NodeJs", GUILayout.Width(100));
            if (sNodeJS)
            {
                Application.OpenURL("https://nodejs.org/en/download");
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.LabelField("(2) 包名请确定是从小米快游戏中心申请的包名，否则游戏加载不出来！");
            EditorGUILayout.LabelField("(3) 请把Assets/XIAOMI-minigame/MiQGame/image/icon.png，图片替换成自己的游戏图标！");
            GUILayout.EndHorizontal();
            GUILayout.Space(20);

            if (GUILayout.Button("生成快游戏", GUILayout.Height(30)))
            {
                if (Common.TryFindSdkByRelativePath(out string relativeSdkPath))
                {
                    sRelativeSdkPath = relativeSdkPath;
                }

                if (string.IsNullOrEmpty(sRelativeSdkPath))
                {
                    Debug.LogError("未检测到 XMSDK...");
                    return;
                }

                if (sBuilding)
                {
                    sBuildingCount += 1;
                    if (sBuildingCount >= 1)
                    {
                        sBuilding = false;
                        sBuildingCount = 0;
                    }
                    PrintBuildLog("正在生成中，请稍后重试");
                }
                else
                {
                    Camera mainCamera = Camera.main;
                    if (mainCamera != null)
                    {
                        var migame = mainCamera.gameObject.GetComponent<MiGameHelper>();
                        if (migame != null)
                        {
                            isAutoHideLoading = false;
                        }
                    }
                    else
                    {
                        isAutoHideLoading = true;
                    }
                    sBuilding = true;
                    Build();
                    sBuilding = false;
                }
            }

            GUILayout.Space(20);
            EditorGUILayout.EndScrollView();
            GUIUtility.ExitGUI();
        }

        public static void QGameLog(string log)
        {
            UnityEngine.Debug.Log($"[qgame]{log}");
        }

        private static void PrintBuildLog(string log)
        {
            UnityEngine.Debug.Log($"[unity]{log}");
        }

        private static void Build()
        {
            if (sIsBuild)
            {
                PrintBuildLog("正在打包中，请等待打包完成...");
                return;
            }

            #region SaveData

            if (string.IsNullOrEmpty(sGameName))
            {
                PrintBuildLog("游戏名称不能为空，请设置游戏名");
                return;
            }
            PlayerPrefs.SetString(KEY_MI_QGAME_NAME, sGameName);

            if (string.IsNullOrEmpty(sPackageName))
            {
                PrintBuildLog("包名不能为空，请设置包名");
                return;
            }
            PlayerPrefs.SetString(KEY_MI_QGAME_PACKAGE, sPackageName);

            if (string.IsNullOrEmpty(sVersion))
            {
                PrintBuildLog("版本号不能为空，请设置版本号");
                return;
            }
            PlayerPrefs.SetString(KEY_MI_QGAME_VERSION, sVersion);

            if (sVersionCode == 0)
            {
                PrintBuildLog("版本code为0，请设置版本code");
                return;
            }
            PlayerPrefs.SetInt(KEY_MI_QGAME_VERSION_CODE, sVersionCode);
            PlayerPrefs.SetString(KEY_MI_QGAME_LOADING_BG, sLoadingBG);

            if (string.IsNullOrEmpty(sChooseBuildPath))
            {
                PrintBuildLog("输出目录不能为空，请选择正确的输出目录");
                return;
            }

            PlayerPrefs.SetString(KEY_MI_QGAME_BUILD_PATH, sChooseBuildPath);
            PlayerPrefs.SetInt(KEY_MI_QGAME_BUILD_MODE, sRelease ? 1 : 0);
            PlayerPrefs.SetInt(KEY_MI_QGAME_ORIENTATION, sPortrait ? 1 : 0);
            PlayerPrefs.SetInt(KEY_MI_QGAME_OPENSSL_PROVIDER, sRemoveOpenSSLProvider ? 1 : 0);
            PlayerPrefs.SetInt(KEY_MI_QGAME_WEBGL2, sIsWebGL2 ? 1 : 0);

            if (string.IsNullOrEmpty(sSignPath))
            {
                PrintBuildLog("签名路径为空，请选择签名目录");
                return;
            }
            PlayerPrefs.SetString(KEY_MI_QGAME_SIGN_PATH, sSignPath);

            PlayerPrefs.SetString(KEY_MI_QGAME_DATA_PATH, sDataPath);
            PlayerPrefs.SetString(KEY_MI_QGAME_WASM_PATH, sWasmPath);
            PlayerPrefs.SetInt(KEY_MI_QGAME_BROTLI_WASM, sBrotliWasm ? 1 : 0);
            PlayerPrefs.SetInt(KEY_MI_QGAME_BROTLI_DATA, sBrotliData ? 1 : 0);
            PlayerPrefs.SetInt(KEY_MI_QGAME_STREAM_DATA, sStreamRes ? 1 : 0);
            PlayerPrefs.SetString(KEY_MI_QGAME_STREAM_PATH, sStreamAssetPath);

            #endregion

            // 判断是否是webgl平台
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.WebGL)
            {
                if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL))
                {
                    PrintBuildLog("请检查打包平台，非WebGL打包方式，不能打包");
                    return;
                }
            }

            try
            {
                sIsBuild = true;
                var result = BuildWebGL();
                if (!result)
                {
                    UnityEngine.Debug.LogError("webgl打包失败");
                }
                else
                {
                    result = BuildMiQGame(sRelativeSdkPath, sBuildPath, sPackageName, sGameName, sVersion, sVersionCode,
                        sLoadingBG, sPortrait, sIsWebGL2, sRemoveOpenSSLProvider, sRelease, sSignPath, sBrotliData,
                        sBrotliWasm, sDataPath, sWasmPath, sStreamRes, sStreamAssetPath);
                    if (!result)
                    {
                        UnityEngine.Debug.LogError("rpk打包失败");
                    }
                    else
                    {
                        EditorUtility.OpenWithDefaultApp(Path.Combine(sBuildPath, "MiQGame"));
                        GUIUtility.ExitGUI();
                    }
                }
            }
            catch (SystemException e)
            {
                PrintBuildLog($"打包失败，请查看Console日志！");
                QGameLog($"打包失败: {e}");
            }
            finally
            {
                sIsBuild = false;
                sBuilding = false;
            }
        }

        private static bool BuildWebGL()
        {
            PrintBuildLog("WebGL打包...");
            sBuildPath = Path.Combine(sChooseBuildPath, "MiQGameBuild");
            FileInfo fi = new FileInfo(sBuildPath);
            var di = fi.Directory;
            if (!di.Exists)
            {
                di.Create();
            }

            sBuildPath = sBuildPath.Replace("\\", "/");

#if UNITY_2021
            PlayerSettings.colorSpace = ColorSpace.Gamma;
#endif
            PlayerSettings.runInBackground = false;
            PlayerSettings.WebGL.threadsSupport = false;
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.WebGL, false);
            if (!sIsWebGL2)
            {
                PrintBuildLog("webgl 1.0打包");
                GraphicsDeviceType[] targets = { GraphicsDeviceType.OpenGLES2 };
                PlayerSettings.SetGraphicsAPIs(BuildTarget.WebGL, targets);
            }
            else
            {
                PrintBuildLog("webgl 2.0打包");
            }
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
            PlayerSettings.WebGL.template = "APPLICATION:Minimal";
            // EditorSettings.spritePackerMode = SpritePackerMode.AlwaysOnAtlas;
            PlayerSettings.WebGL.linkerTarget = WebGLLinkerTarget.Wasm;

            ClearBuildPath(sBuildPath);
            List<string> scenes = new List<string>();
            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                {
                    scenes.Add(scene.path);
                }
            }

            if (scenes.Count == 0)
            {
                PrintBuildLog("未选择需要打包的场景，请选择打包场景");
                return false;
            }

            var result = BuildPipeline.BuildPlayer(scenes.ToArray(), sBuildPath, BuildTarget.WebGL, BuildOptions.None);
            var summary = result.summary;
            if (summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                PrintBuildLog("WebGL工程打包完成");
                return true;
            }
            else
            {
                PrintBuildLog("WebGL工程打包失败，请查看console日志！");
                return false;
            }
        }

        /// <summary>
        /// 清空输出目录
        /// </summary>
        public static void ClearBuildPath(string path)
        {
            PrintBuildLog("清空输出目录...");
            DirectoryInfo buildDir = new DirectoryInfo(path);
            if (buildDir.Exists)
            {
                DirectoryInfo[] childs = buildDir.GetDirectories();
                foreach (DirectoryInfo child in childs)
                {
                    child.Delete(true);
                }
            }
            else
            {
                Directory.CreateDirectory(path);
            }
            PrintBuildLog("清空输出目录完成");
        }

        /// <summary>
        /// 转换成小米快游戏
        /// </summary>
        public static bool BuildMiQGame(string sdkPath, string buildPath, string packageName, string gameName,
            string version, int versionCode, string loadingBg, bool portrait, bool isWebGL2, bool removeOpenSSLProvider,
            bool release, string signPath, bool brotliData, bool brotliWasm, string dataPath, string wasmPath,
            bool streamRes, string streamAssetPath)
        {
            //资源文件的处理
            PrintBuildLog("小米快游戏转换...");
            UnityEngine.Debug.Log("小米快游戏转换...");
            var result = CopyQGameSdkToBuild(sdkPath, buildPath, signPath);
            if (!result) {
                UnityEngine.Debug.LogError("快游戏转换失败");
                return false;
            }

            // 创建manifest.json
            PrintBuildLog("创建manifest.json");
            result = CreateManifest(buildPath, packageName, gameName, version, versionCode, portrait);
            if (!result)
            {
                UnityEngine.Debug.LogError("快游戏转换失败");
                return false;
            }
            PrintBuildLog("创建manifest.json完成");

            PrintBuildLog("修改package.json文件");
            result = SetPackageParm(buildPath, removeOpenSSLProvider);
            if (!result)
            {
                UnityEngine.Debug.LogError("快游戏转换失败");
                return false;
            }
            PrintBuildLog("修改package.json文件完成");

            PrintBuildLog("修改config.json文件");
            result = SetConfigParm(buildPath, isWebGL2);
            if (!result)
            {
                UnityEngine.Debug.LogError("快游戏转换失败");
                return false;
            }
            PrintBuildLog("修改config.json文件完成");

            // 框架转换
            PrintBuildLog("framework转换");
            result = FrameworkConverter(buildPath, gameName, version, loadingBg, brotliData, brotliWasm, dataPath,
                wasmPath, streamRes, streamAssetPath);
            if (!result)
            {
                UnityEngine.Debug.LogError("快游戏转换失败");
                return false;
            }
            PrintBuildLog("framework转换完成");

            // 复制unity数据
            PrintBuildLog("整理webgl数据");
            result = CopyUnityData(sdkPath, buildPath, brotliData, brotliWasm, dataPath, wasmPath);
            if (!result)
            {
                UnityEngine.Debug.LogError("快游戏转换失败");
                return false;
            }
            PrintBuildLog("整理webgl数据完成");

            // 整理工程结构
            PrintBuildLog("整理工程结构");
            result = ArrangeProject(buildPath);
            if (!result)
            {
                UnityEngine.Debug.LogError("快游戏转换失败");
                return false;
            }
            PrintBuildLog("整理工程结构完成");

            // rpk打包
            PrintBuildLog("rpk打包");
            UnityEngine.Debug.Log("rpk打包...");
            result = PackageRpk(sdkPath, buildPath, release);
            if (!result)
            {
                UnityEngine.Debug.LogError("快游戏转换失败");
                return false;
            }
            UnityEngine.Debug.Log("rpk打包完成");
            PrintBuildLog("rpk打包完成");

            return true;
        }

        private static bool CopyQGameSdkToBuild(string sdkPath, string buildPath, string signPath)
        {
            string sourcePath = Path.Combine(sdkPath, "MiQGame");

            if (!Directory.Exists(sourcePath))
            {
                UnityEngine.Debug.LogError("文件目录不存在MiQGame，请检查资源文件!");
                return false;
            }

            //复制快游戏框架
            CopyDirectory(sourcePath, buildPath);

            if (!Directory.Exists(signPath))
            {
                UnityEngine.Debug.LogError("签名目录不存在，请检查签名目录!");
                return false;
            }

            //复制签名文件
            CopyDirectory(signPath, Path.Combine(buildPath, "MiQGame/sign"), false);

            return true;
        }

        /// <summary>
        /// 递归复制文件夹
        /// </summary>
        /// <param name="sourcePath">源文件夹</param>
        /// <param name="destRoot">目标文件夹</param>
        /// <param name="includeSourceFolder">是否包含源文件夹本身（默认包含）</param>
        private static void CopyDirectory(string sourcePath, string destRoot,  bool includeSourceFolder = true)
        {
            if (!Directory.Exists(sourcePath))
                return;

            string targetDir = includeSourceFolder ? Path.Combine(destRoot, Path.GetFileName(sourcePath)) : destRoot;

            if (!Directory.Exists(targetDir))
                Directory.CreateDirectory(targetDir);

            foreach (var entry in Directory.GetFileSystemEntries(sourcePath))
            {
                string target = Path.Combine(targetDir, Path.GetFileName(entry));

                if (Directory.Exists(entry))
                {
                    CopyDirectory(entry, targetDir);
                }
                else if (!entry.EndsWith(".meta"))
                {
                    File.Copy(entry, target, true);
                }
            }
        }

        private static bool CreateManifest(string buildPath ,string packageName, string gameName, string version, int versionCode, bool portrait)
        {
            var manifest = "{\n";
            manifest += $"  \"package\":\"{packageName}\",\n";
            manifest += $"  \"name\":\"{gameName}\",\n";
            manifest += $"  \"versionName\":\"{version}\",\n";
            manifest += $"  \"versionCode\":{versionCode},\n";
            manifest += $"  \"minPlatformVersion\":1080,\n";
            manifest += $"  \"icon\":\"/image/icon.png\",\n";
            var orientation = portrait ? "portrait" : "landscape";
            manifest += $"  \"orientation\":\"{orientation}\",\n";
            manifest += $"  \"type\":\"game\",\n";
            manifest += "   \"config\":{},\n";
            manifest += "   \"display\":{},\n";
            manifest += "  \"subpackages\": [\n";
            manifest += "  {\n";
            manifest += "       \"name\" : \"subpackage\",\n";
            manifest += "       \"root\" : \"subpackage/\"\n";
            manifest += "  }]\n";
            manifest += "}";
            File.WriteAllText(Path.Combine(buildPath, "MiQGame", "manifest.json"), manifest);
            return true;
        }

        /// <summary>
        /// 修改package.json文件
        /// </summary>
        /// <returns></returns>
        private static bool SetPackageParm(string buildPath, bool removeOpenSSLProvider)
        {
            bool needRemove = false;
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                //判断是否需要移除 OpenSSL Provider
                if (removeOpenSSLProvider)
                {
                    needRemove = true;
                }
            }
            else
            {
                needRemove = true;
            }

            //判断是否需要移除 OpenSSL Provider
            if (needRemove)
            {
                //获取package.json的完整路径
                string packageJsonPath = Path.Combine(buildPath, "MiQGame", "package.json");

                // 读取原始文件内容
                string originalContent = File.ReadAllText(packageJsonPath);
                string openSSLProvider = "SET NODE_OPTIONS=--openssl-legacy-provider && ";
                string replaceStr = string.Empty;
                // 移除 SET NODE_OPTIONS=--openssl-legacy-provider &&
                originalContent = Regex.Replace(originalContent, Regex.Escape(openSSLProvider), replaceStr);

                //保存修改后的内容
                File.WriteAllText(packageJsonPath, originalContent);
            }
            return true;
        }

        /// <summary>
        /// 修改config.json文件
        /// </summary>
        /// <returns></returns>
        private static bool SetConfigParm(string buildPath, bool isWebGL2)
        {
            //获取config.json的完整路径
            string jsonPath = Path.Combine(buildPath, "MiQGame", "config.json");

            // 读取原始文件内容
            string originalContent = File.ReadAllText(jsonPath);
            if (isWebGL2)
            {
                string originStr = "\"hasWebGL\": 1,";
                string replaceStr = "\"hasWebGL\": 2,";
                originalContent = Regex.Replace(originalContent, originStr, replaceStr);
            }

            if (!isWebGL2)
            {
                PrintBuildLog("webgl 1.0打包");
            }
            else
            {
                PrintBuildLog("webgl 2.0打包");
            }

            //保存修改后的内容
            File.WriteAllText(jsonPath, originalContent);
            return true;
        }

        private static bool FrameworkConverter(string buildPath, string gameName, string version, string loadingBg,
            bool brotliData, bool brotliWasm, string dataPath, string wasmPath, bool streamRes, string streamAssetPath)
        {
            LoadMiCoreData();
            var result = MiCore.FrameworkAdaptation(buildPath, gameName, version, loadingBg, brotliData, brotliWasm,
                dataPath, wasmPath, streamRes, streamAssetPath);
            return result;
        }

        private static void LoadMiCoreData()
        {
            MiCore.CURRETN_VERSION = CURRETN_VERSION;
            MiCore.unityVersion = Application.unityVersion;
            MiCore.isAutoHideLoading = isAutoHideLoading;

            #if TUANJIE_1_0_OR_NEWER
                    MiCore.TUANJIE_1_0_OR_NEWER = true;
            #endif

            #if TUANJIE_2022
                    MiCore.TUANJIE_2022 = true;
            #endif

            #if UNITY_2019
                    MiCore.UNITY_2019 = true;
            #elif UNITY_2020
                    MiCore.UNITY_2020 = true;
            #elif UNITY_2021
                    MiCore.UNITY_2021 = true;
            #elif UNITY_2022
                    MiCore.UNITY_2022 = true;
            #endif
        }

        private static bool CopyUnityData(string sdkPath, string buildPath, bool brotliData, bool brotliWasm, string sdataPath, string swasmPath)
        {
            var pathList = buildPath.Split("/".ToCharArray());
            var lastPath = pathList[pathList.Length - 1];

#if UNITY_2019
            var wasmFF = new FileInfo(Path.Combine(buildPath, "Build", $"{lastPath}.wasm.code.unityweb"));
            wasmFF.CopyTo(Path.Combine(buildPath, "Build", "build.wasm"), true);
#else
            var wasmFF = new FileInfo(Path.Combine(buildPath, "Build", $"{lastPath}.wasm"));
#endif
            if (brotliWasm || !string.IsNullOrEmpty(swasmPath))
            {
                //brotli压缩wasm资源   "Build/build.wasm.br"
#if UNITY_2019 || UNITY_2020
                string sourcePath = Path.Combine(sdkPath, "Editor/Tools");
                var workDir = Path.Combine(System.Environment.CurrentDirectory, sourcePath);
                string brotliExePath;
                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    brotliExePath = Path.Combine(System.Environment.CurrentDirectory, sourcePath, "brotli.exe");
                }
                else if (Application.platform == RuntimePlatform.OSXEditor)
                {
                    brotliExePath = Path.Combine(System.Environment.CurrentDirectory, sourcePath, "brotli.sh");
                }
                else
                {
                    Debug.LogError("Unsupported platform for brotli compression.");
                    return false;
                }

                string args = Path.Combine(buildPath, "Build") + " -wasm";
                var outList = Common.RunCmd(brotliExePath, args, workDir, 1000);
#else
                string sourcePath = wasmFF.FullName;
                string outputPath = Path.Combine(buildPath, "Build", "build.wasm.br");
                MiFileCompressor.CompressFileWithBrotli(sourcePath, outputPath);
#endif
            }
            else
            {
                string wasmPath = Path.Combine(buildPath, "Build", "build.wasm");
                if (!File.Exists(wasmPath))
                {
                    wasmFF.CopyTo(wasmPath, true);
                }
            }

            if (string.IsNullOrEmpty(swasmPath))
            {
                if (!brotliWasm)
                {
                    wasmFF = new FileInfo(Path.Combine(buildPath, "Build", "build.wasm"));
                    wasmFF.CopyTo(Path.Combine(buildPath, "MiQGame", "unity", "build.wasm"), true);
                }
                else
                {
                    //复制wasm资源     "unity/build.wasm.br"
                    var wasmBr = new FileInfo(Path.Combine(buildPath, "Build", "build.wasm.br"));
                    wasmBr.CopyTo(Path.Combine(buildPath, "MiQGame", "unity", "build.wasm.br"), true);
                }
            }

#if UNITY_2019
            var dataFF = new FileInfo(Path.Combine(buildPath, "Build", $"{lastPath}.data.unityweb"));
            dataFF.CopyTo(Path.Combine(buildPath, "Build", "build.data"), true);
#else
            var dataFF = new FileInfo(Path.Combine(buildPath, "Build", $"{lastPath}.data"));
#endif

            if (brotliData || !string.IsNullOrEmpty(sdataPath))
            {
                //brotli压缩data资源   "Build/build.data.br"
#if UNITY_2019 || UNITY_2020
                string sourcePath = Path.Combine(sdkPath, "Editor/Tools");
                var workDir = Path.Combine(System.Environment.CurrentDirectory, sourcePath);
                string brotliExePath;
                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    brotliExePath = Path.Combine(System.Environment.CurrentDirectory, sourcePath, "brotli.exe");
                }
                else if (Application.platform == RuntimePlatform.OSXEditor)
                {
                    brotliExePath = Path.Combine(System.Environment.CurrentDirectory, sourcePath, "brotli.sh");
                }
                else
                {
                    Debug.LogError("Unsupported platform for brotli compression.");
                    return false;
                }

                string args = Path.Combine(buildPath, "Build") + " -data";
                var outList = Common.RunCmd(brotliExePath, args, workDir, 1000);
#else
                string sourcePath = dataFF.FullName;
                string outputPath = Path.Combine(buildPath, "Build", "build.data.br");
                MiFileCompressor.CompressFileWithBrotli(sourcePath, outputPath);
#endif
            }
            else
            {
                string dataPath = Path.Combine(buildPath, "Build", "build.data");
                if (!File.Exists(dataPath))
                {
                    dataFF.CopyTo(dataPath, true);
                }
            }

            if (string.IsNullOrEmpty(sdataPath))
            {
                if (!brotliData)
                {
                    dataFF = new FileInfo(Path.Combine(buildPath, "Build", "build.data"));
                    dataFF.CopyTo(Path.Combine(buildPath, "MiQGame", "unity", "build.data"), true);
                }
                else
                {
                    //复制data资源     "unity/build.data.br"
                    var dataBr = new FileInfo(Path.Combine(buildPath, "Build", "build.data.br"));
                    dataBr.CopyTo(Path.Combine(buildPath, "MiQGame", "unity", "build.data.br"), true);
                }
            }

            return true;
        }

        private static bool ArrangeProject(string buildPath)
        {
            Directory.Move(Path.Combine(buildPath, "MiQGame", "unity"), Path.Combine(buildPath, "MiQGame", "subpackage", "unity"));

            File.Move(Path.Combine(buildPath, "MiQGame", "main.js"),
                Path.Combine(buildPath, "MiQGame", "subpackage", "main.js"));

            File.Move(Path.Combine(buildPath, "MiQGame", "build.framework.js"),
                Path.Combine(buildPath, "MiQGame", "subpackage", "build.framework.js"));

            File.Move(Path.Combine(buildPath, "MiQGame", "qgame-adapter.js"),
                Path.Combine(buildPath, "MiQGame", "subpackage", "qgame-adapter.js"));

            File.Move(Path.Combine(buildPath, "MiQGame", "game.js"),
                Path.Combine(buildPath, "MiQGame", "main.js"));

            File.Move(Path.Combine(buildPath, "MiQGame", "config.json"),
                Path.Combine(buildPath, "MiQGame", "subpackage", "config.json"));

            return true;
        }

        private static bool PackageRpk(string sdkPath, string buildPath, bool release)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                string sourcePath = Path.Combine(sdkPath, "Editor/Tools");

                var cmdInstall = Path.Combine(System.Environment.CurrentDirectory, sourcePath, "npm_install.bat");
                cmdInstall = Common.FormatPath(cmdInstall);

                var cmdRelease = Path.Combine(System.Environment.CurrentDirectory, sourcePath, "npm_release.bat");
                cmdRelease = Common.FormatPath(cmdRelease);

                var cmdBuild = Path.Combine(System.Environment.CurrentDirectory, sourcePath, "npm_build.bat");
                cmdBuild = Common.FormatPath(cmdBuild);

                var workDir = Path.Combine(buildPath, "MiQGame");
                workDir = Common.FormatPath(workDir);

                var cmdRpk = release ? cmdRelease : cmdBuild;

                var outList = Common.RunCmd(cmdInstall, "", workDir, 60);
                foreach (var log in outList)
                {
                    if (!String.IsNullOrEmpty(log))
                    {
                        UnityEngine.Debug.Log(log);
                    }
                }

                outList = Common.RunCmd(cmdRpk, "", workDir, 30);
                foreach (var log in outList)
                {
                    if (!string.IsNullOrEmpty(log))
                    {
                        if (!log.Contains("Warning:"))
                        {
                            UnityEngine.Debug.Log(log);
                        }
                    }
                }
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                UnityEngine.Debug.Log("OSX暂不支持自动打包RPK，请进入转换完成目录手动执行npm打包命令 " +
                    "\n 1. npm install \n 2. Debug RPK： npm run build \n 3. Release RPK： npm run release");
            }

            return true;
        }

#region ===============检查工具版本=================
        private static string pkgPath  = "";
        private static string url      = "http://tennews.cn/unity/";
        private static string infoName = "version.txt";
        private static string versionLog = "";
        private static void GetVersionInfo()
        {
            //本地package包文件夹
            pkgPath = Path.Combine(Application.dataPath, "../") + "/Mi_Tools/";
            if (!Directory.Exists(pkgPath))
            {
                Directory.CreateDirectory(pkgPath);
            }
            EditorCoroutineUtility.StartCoroutineOwnerless(GetRequest(url + infoName));
        }

        static IEnumerator GetRequest(string uri)
        {
            versionLog = "开始获取版本信息...";
            UnityEngine.Debug.Log(uri);
            UnityWebRequest webRequest = UnityWebRequest.Get(uri);
            webRequest.timeout = 10;
            yield return webRequest.SendWebRequest();
#if UNITY_2019
            if (webRequest.isNetworkError ||webRequest.isHttpError)
#else
            if (webRequest.result != UnityWebRequest.Result.Success)
#endif
            {
                versionLog = "获取版本信息失败，请确保服务器配置了版本信息";
                yield break;
            }

            var lastVersion = webRequest.downloadHandler.text;
            versionLog = $"最新版本，{lastVersion}";

            // 解析版本号
            Version latestVersionParsed = ParseVersion(lastVersion);
            Version currentVersionParsed = ParseVersion(CURRETN_VERSION);
            if (currentVersionParsed.CompareTo(latestVersionParsed) >= 0)
            {
                versionLog = "当前已是最新版本";
                yield break;
            }

            string newPkg = $"MiQGameConverter_{webRequest.downloadHandler.text}.unitypackage";
            versionLog = "发现新版本,开始下载..." + url + newPkg;

            UnityWebRequest pkgReq = UnityWebRequest.Get(url + newPkg);
            yield return pkgReq.SendWebRequest();

#if UNITY_2019
            if (pkgReq.isNetworkError ||pkgReq.isHttpError)
#else
            if (pkgReq.result != UnityWebRequest.Result.Success)
#endif
            {
                versionLog = $"下载失败，{pkgReq.error}";
                yield break;
            }

            string savePath = pkgPath + newPkg;
            byte[] resultData = pkgReq.downloadHandler.data;
            FileInfo fileInfo = new FileInfo(savePath);
            FileStream fileStream = fileInfo.Create();
            fileStream.Write(resultData, 0, resultData.Length);
            fileStream.Flush();
            fileStream.Close();
            fileStream.Dispose();

            versionLog = "最新版本下载完成，请手动导入！";
            EditorUtility.OpenWithDefaultApp(pkgPath);
        }

        private static Version ParseVersion(string versionString)
        {
            // 版本号格式为 "主版本.次版本.修订号" 或 "主版本.次版本.修订号后缀"
            var match = Regex.Match(versionString, @"(\d+\.\d+\.\d+)");
            if (match.Success)
            {
                // 提取数字部分（如 "1.4.0"）
                string numericPart = match.Groups[1].Value;
                return new Version(numericPart);
            }
            else
            {
                throw new FormatException("版本号格式不正确");
            }
        }
#endregion
    }
}
#endif