#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

namespace Mi
{
    public class SignWindow : EditorWindow
    {
        private const string KEY_MI_SIGN_OUTPUT_DIR = "mi_sign_output_dir";
        private const string KEY_MI_SIGN_COUNTRY = "mi_sign_country";
        private const string KEY_MI_SIGN_STATE = "mi_sign_state";
        private const string KEY_MI_SIGN_LOCALITY = "mi_sign_locality";
        private const string KEY_MI_SIGN_ORG = "mi_sign_org";
        private const string KEY_MI_SIGN_COMMON_NAME = "mi_sign_common_name";
        private const string KEY_MI_SIGN_EMAIL = "mi_sign_email";

        private static string outputDir = "";
        private static string country = "CN";
        private static string state = "default";
        private static string locality = "default";
        private static string org = "default";
        private static string commonName = "RPK";
        private static string email = "default";
        private Vector2 scrollRoot;
        private static string sRelativeSdkPath = string.Empty; // XMSDK

        private static void LoadData()
        {
            if (string.IsNullOrEmpty(outputDir))
            {
                outputDir = EditorPrefs.GetString(KEY_MI_SIGN_OUTPUT_DIR, outputDir);
            }

            country = EditorPrefs.GetString(KEY_MI_SIGN_COUNTRY, country);
            state = EditorPrefs.GetString(KEY_MI_SIGN_STATE, state);
            locality = EditorPrefs.GetString(KEY_MI_SIGN_LOCALITY, locality);
            org = EditorPrefs.GetString(KEY_MI_SIGN_ORG, org);
            commonName = EditorPrefs.GetString(KEY_MI_SIGN_COMMON_NAME, commonName);
            email = EditorPrefs.GetString(KEY_MI_SIGN_EMAIL, email);
        }

        [MenuItem("小米快游戏/生成证书", false, 2)]
        private static void ShowWindow()
        {
            LoadData();
            SignWindow window = (SignWindow)EditorWindow.GetWindow(typeof(SignWindow), true, "创建新的签名");
            window.minSize = new Vector2(580, 380);
            window.Show();
        }

        private void OnGUI()
        {
            scrollRoot = EditorGUILayout.BeginScrollView(scrollRoot);
            EditorGUILayout.BeginVertical("box");

            // 输出路径选择
            EditorGUILayout.LabelField("证书生成路径", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            outputDir = EditorGUILayout.TextField("路径", outputDir);
            if (GUILayout.Button("浏览...", GUILayout.Width(100)))
            {
                string path = EditorUtility.SaveFolderPanel("选择证书保存目录", outputDir, "Sign");
                if (!string.IsNullOrEmpty(path))
                {
                    outputDir = path;
                }
            }

            EditorGUILayout.EndHorizontal();

            // 参数配置
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("证书参数配置", EditorStyles.boldLabel);

            country = EditorGUILayout.TextField(new GUIContent("国家名称(?)", "请输入2个字母，如：CN"), country,
                GUILayout.Height(25));
            EditorGUILayout.Space(10);

            state = EditorGUILayout.TextField(new GUIContent("省份名称(?)", "请输入全称，如：GuangDong"), state,
                GUILayout.Height(25));
            EditorGUILayout.Space(10);

            locality = EditorGUILayout.TextField(new GUIContent("城市名称(?)", "城市，如：Shen zhen"), locality,
                GUILayout.Height(25));
            EditorGUILayout.Space(10);

            org = EditorGUILayout.TextField(new GUIContent("组织单位名称(?)", "如：组织单位名称：Operations"), org,
                GUILayout.Height(25));
            EditorGUILayout.Space(10);

            commonName = EditorGUILayout.TextField(new GUIContent("通用名称(?)", "如，服务器域名或你的名字：default"), commonName,
                GUILayout.Height(25));
            EditorGUILayout.Space(10);

            email = EditorGUILayout.TextField(new GUIContent("邮箱地址(?)", "如：default@default.com"), email,
                GUILayout.Height(25));

            EditorGUILayout.Space(20);
            if (GUILayout.Button("生成证书", GUILayout.Height(30)))
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

                SaveData();
                var result =GenerateCertificate();
                if (!result)
                {
                    Debug.LogError("生成证书失败");
                }
                else
                {
                    EditorUtility.OpenWithDefaultApp(outputDir);
                    GUIUtility.ExitGUI();
                }
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        private void SaveData()
        {
            if (!string.IsNullOrEmpty(outputDir))
            {
                EditorPrefs.SetString(KEY_MI_SIGN_OUTPUT_DIR, outputDir);
            }

            EditorPrefs.SetString(KEY_MI_SIGN_COUNTRY, country);
            EditorPrefs.SetString(KEY_MI_SIGN_STATE, state);
            EditorPrefs.SetString(KEY_MI_SIGN_LOCALITY, locality);
            EditorPrefs.SetString(KEY_MI_SIGN_ORG, org);
            EditorPrefs.SetString(KEY_MI_SIGN_COMMON_NAME, commonName);
            EditorPrefs.SetString(KEY_MI_SIGN_EMAIL, email);
        }

        private bool GenerateCertificate()
        {
            // 参数验证
            if (string.IsNullOrEmpty(outputDir))
            {
                EditorUtility.DisplayDialog("错误", "请选择输出目录！", "确定");
                return false;
            }

            Debug.Log("开始生成证书...");
            try
            {
                DirectoryInfo buildDir = new DirectoryInfo(outputDir);
                if (buildDir.Exists)
                {
                    DirectoryInfo[] directories = buildDir.GetDirectories();
                    foreach (DirectoryInfo dir in directories)
                    {
                        dir.Delete(true);
                    }
                }

                string sourcePath = Path.Combine(sRelativeSdkPath, "Editor/Tools");
                var workDir = Path.Combine(System.Environment.CurrentDirectory, sourcePath);
                string signExePath;
                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    signExePath = Path.Combine(System.Environment.CurrentDirectory, sourcePath, "sign.exe");
                }
                else if (Application.platform == RuntimePlatform.OSXEditor)
                {
                    signExePath = Path.Combine(System.Environment.CurrentDirectory, sourcePath, "sign.sh");
                }
                else
                {
                    Debug.LogError("不支持的平台");
                    return false;
                }

                // 构建参数
                string parameterArgs = string.Join(" ", new[]
                {
                    $"-c {country}",
                    $"-st {state}",
                    $"-l {locality}",
                    $"-o {org}",
                    $"-cn {commonName}",
                    $" -e {email}"
                });

                // 构建完整命令行
                string args = outputDir + " " + parameterArgs;
                var outList = Common.RunCmd(signExePath, args, workDir, 1000);

                Debug.Log("生成证书完成！");
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"生成证书失败：{ex.Message}");
                return false;
            }
        }
    }
}
#endif