#if TUANJIE_1_4_OR_NEWER
using System;
using UnityEditor;
using UnityEditor.Build.Profile;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Rendering;
using System.Linq;

namespace Mi
{
    internal class XiaomiMiniGameSettingsEditor : MiniGameSettingsEditor
    {
        private readonly Dictionary<string, string> _editingInputData = new Dictionary<string, string>();
        private readonly Dictionary<string, bool> _editingBooleanData = new Dictionary<string, bool>();
        private readonly Dictionary<string, int> _editingEnumData = new Dictionary<string, int>();

        private bool _isFoldInfoOptions = true;
        private bool _isFoldLaunchOptions = true;

        private static int _textareaHeight = 50;
        private static float _labelWidth = 140;
        private static float _toggleWidth = 16;
        private static float _snapPadding = 10;
        private static float _fieldWidth => EditorGUIUtility.currentViewWidth - (_labelWidth + _snapPadding * 2 + 35);

        public override void OnMiniGameSettingsIMGUI(SerializedObject serializedObject, SerializedProperty miniGameProperty)
        {
            ReadSettingsProperties(miniGameProperty);
            serializedObject.UpdateIfRequiredOrScript();

            DrawXiaoMiSettingsGUI(serializedObject, miniGameProperty);

            SaveSettingsModifiedProperties(miniGameProperty);
            serializedObject.ApplyModifiedProperties();
        }

        #region Style

        private void DrawXiaoMiSettingsGUI(SerializedObject serializedObject, SerializedProperty miniGameProperty)
        {
            _isFoldInfoOptions = EditorGUILayout.Foldout(_isFoldInfoOptions, "基本信息");
            if (_isFoldInfoOptions)
            {
                EditorGUILayout.BeginVertical("frameBox", GUILayout.ExpandWidth(true));

                CreateInput("gameName", "游戏名称");
                CreateInput("packageName", "包名", "小米快游测试包名：com.demo.ch.mini");
                CreateInput("version", "版本号");
                CreateInput("versionCode", "版本Code");
                CreateInput("loadingBG", "自定义Loading图片链接");
                CreateEnumPopup("landscape", "游戏方向", new[] { "竖屏", "横屏" }, new[] { 0, 1 });
                CreateEnumPopup("isWebGL2", "WebGL版本", new[] { "WebGL 1.0", "WebGL 2.0" }, new[] { 0, 1 });
                CreateEnumPopup("removeOpenSSLProvider", "NodeJS版本 (?)", new[] { "高版本(NodeJS 17.9.1+)", "低版本" }, new[] { 0, 1 }, "如果您的NodeJS版本高于17.9.1，请选择高版本");
                CreateEnumPopup("release", "包类型", new[] { "Debug", "Release" }, new[] { 0, 1 });
                CreateInput("signPath", "签名文件根目录 (?)", "签名文件生成方式，请查看小米快游戏介绍文档！");

                EditorGUILayout.EndVertical();
            }

            _isFoldLaunchOptions = EditorGUILayout.Foldout(_isFoldLaunchOptions, new UnityEngine.GUIContent("资源加载配置", "如果包体过大，请把打包后的Build/build.data或者Build/build.wasm放在网络上加载"));
            if (_isFoldLaunchOptions)
            {
                EditorGUILayout.BeginVertical("frameBox", GUILayout.ExpandWidth(true));

                CreateBoolean("brotliData", "data brotli压缩(?)", "使用brotli压缩data文件，网络加载会强制开启");
                CreateInput("dataPath", "build.data地址(http)", "如果包体过大，请把打包后的Build/build.data放在网络上加载");
                CreateBoolean("brotliWasm", "wasm brotli压缩(?)", "使用brotli压缩wasm文件，网络加载会强制开启");
                CreateInput("wasmPath", "build.wasm地址(http)", "如果包体过大，请把打包后的Build/build.wasm放在网络上加载");
                CreateBoolean("streamRes", "StreamingAssets");
                CreateInput("streamAssetPath", "资源地址(http)");

                EditorGUILayout.EndVertical();
            }
        }

        /// <summary>
        /// 创建配置项标签
        /// </summary>
        private void CreateFieldLabel(string label, string tooltip = null)
        {
            EditorGUILayout.LabelField(string.Empty, GUILayout.Width(_snapPadding));
            if (tooltip == null)
            {
                GUILayout.Label(label, GUILayout.Width(_labelWidth));
            }
            else
            {
                GUILayout.Label(new GUIContent(label, tooltip), GUILayout.Width(_labelWidth));
            }
        }

        /// <summary>
        /// 创建单行输入控件
        /// </summary>
        private void CreateInput(string name, string label, string tooltip = null)
        {
            if (!_editingInputData.TryGetValue(name, out var value))
                _editingInputData[name] = value = "";

            GUILayout.BeginHorizontal();
            CreateFieldLabel(label, tooltip);
            _editingInputData[name] = GUILayout.TextField(value, GUILayout.MaxWidth(_fieldWidth));
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 创建带“选择目录”按钮的单行输入控件
        /// </summary>
        private void CreateDirectoryInput(string name, string label, string tooltip = null)
        {
            if (!_editingInputData.TryGetValue(name, out var value))
                _editingInputData[name] = value = "";

            GUILayout.BeginHorizontal();
            CreateFieldLabel(label, tooltip);
            _editingInputData[name] = GUILayout.TextField(value, GUILayout.MaxWidth(_fieldWidth));
            if (GUILayout.Button("选择", GUILayout.Width(60)))
            {
                string selectedFolder = EditorUtility.OpenFolderPanel("选择目录", value, "");
                if (!string.IsNullOrEmpty(selectedFolder))
                {
                    _editingInputData[name] = selectedFolder;
                }
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 创建多行输入控件
        /// </summary>
        private void CreateTextarea(string name, string label, string tooltip = null)
        {
            if (!_editingInputData.TryGetValue(name, out var value))
                _editingInputData[name] = value = "";

            GUILayout.BeginHorizontal();
            CreateFieldLabel(label, tooltip);
            EditorGUI.BeginDisabledGroup(false);
            _editingInputData[name] = EditorGUILayout.TextArea(value,
                GUILayout.MaxWidth(_fieldWidth), GUILayout.MinHeight(_textareaHeight));
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 创建布尔型复选框控件
        /// </summary>
        private void CreateBoolean(string name, string label, string tooltip = null, bool disabled = false, string disabledLabel = null, string disabledTooltip = null, Action createAction = null)
        {
            if (!_editingBooleanData.TryGetValue(name, out var value))
                _editingBooleanData[name] = value = false;

            GUILayout.BeginHorizontal();
            CreateFieldLabel(label, tooltip);
            EditorGUI.BeginDisabledGroup(disabled);
            _editingBooleanData[name] = EditorGUILayout.Toggle(value, GUILayout.Width(_toggleWidth));
            if (disabled && disabledLabel != null)
            {
                EditorGUILayout.LabelField(string.Empty, GUILayout.Width(_snapPadding));
                if (disabledTooltip == null)
                {
                    GUILayout.Label(disabledLabel);
                }
                else
                {
                    GUILayout.Label(new UnityEngine.GUIContent(disabledLabel, disabledTooltip));
                }
                EditorGUILayout.LabelField(string.Empty, GUILayout.Width(_snapPadding));
            }

            if (createAction != null)
                createAction();

            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 创建枚举下拉选框控件
        /// </summary>
        private void CreateEnumPopup(string name, string label, string[] options, int[] values, string tooltip = null)
        {
            if (!_editingEnumData.TryGetValue(name, out var value))
                _editingEnumData[name] = value = 0;

            GUILayout.BeginHorizontal();
            CreateFieldLabel(label, tooltip);
            _editingEnumData[name] = EditorGUILayout.IntPopup(value, options, values, GUILayout.MaxWidth(_fieldWidth));
            GUILayout.EndHorizontal();
        }

        #endregion

        #region Data

        /// <summary>
        /// 从配置文件读取所有属性
        /// </summary>
        /// <param name="miniGameProperty"></param>
        private void ReadSettingsProperties(SerializedProperty miniGameProperty)
        {
            _editingInputData["gameName"] = ReadProperty<string>(miniGameProperty, "gameName");
            _editingInputData["packageName"] = ReadProperty<string>(miniGameProperty, "packageName");
            _editingInputData["version"] = ReadProperty<string>(miniGameProperty, "version");
            _editingInputData["versionCode"] = ReadProperty<int>(miniGameProperty, "versionCode").ToString();
            _editingInputData["loadingBG"] = ReadProperty<string>(miniGameProperty, "loadingBG");
            _editingInputData["signPath"] = ReadProperty<string>(miniGameProperty, "signPath");
            _editingInputData["dataPath"] = ReadProperty<string>(miniGameProperty, "dataPath");
            _editingInputData["wasmPath"] = ReadProperty<string>(miniGameProperty, "wasmPath");
            _editingInputData["streamAssetPath"] = ReadProperty<string>(miniGameProperty, "streamAssetPath");

            _editingEnumData["landscape"] = ReadProperty<int>(miniGameProperty, "landscape");
            _editingEnumData["isWebGL2"] = ReadProperty<int>(miniGameProperty, "isWebGL2");
            _editingEnumData["removeOpenSSLProvider"] = ReadProperty<int>(miniGameProperty, "removeOpenSSLProvider");
            _editingEnumData["release"] = ReadProperty<int>(miniGameProperty, "release");

            _editingBooleanData["brotliWasm"] = ReadProperty<bool>(miniGameProperty, "brotliWasm");
            _editingBooleanData["brotliData"] = ReadProperty<bool>(miniGameProperty, "brotliData");
            _editingBooleanData["streamRes"] = ReadProperty<bool>(miniGameProperty, "streamRes");
        }

        /// <summary>
        /// 保存所有属性到配置文件
        /// </summary>
        /// <param name="miniGameProperty"></param>
        private void SaveSettingsModifiedProperties(SerializedProperty miniGameProperty)
        {
            SaveProperty(miniGameProperty, "gameName", _editingInputData["gameName"]);
            SaveProperty(miniGameProperty, "packageName", _editingInputData["packageName"]);
            SaveProperty(miniGameProperty, "version", _editingInputData["version"]);
            SaveProperty(miniGameProperty, "versionCode", int.Parse(_editingInputData["versionCode"]));
            SaveProperty(miniGameProperty, "loadingBG", _editingInputData["loadingBG"]);
            SaveProperty(miniGameProperty, "signPath", _editingInputData["signPath"]);
            SaveProperty(miniGameProperty, "dataPath", _editingInputData["dataPath"]);
            SaveProperty(miniGameProperty, "wasmPath", _editingInputData["wasmPath"]);
            SaveProperty(miniGameProperty, "streamAssetPath", _editingInputData["streamAssetPath"]);

            SaveProperty(miniGameProperty, "landscape", _editingEnumData["landscape"]);
            SaveProperty(miniGameProperty, "isWebGL2", _editingEnumData["isWebGL2"]);
            SaveProperty(miniGameProperty, "removeOpenSSLProvider", _editingEnumData["removeOpenSSLProvider"]);
            SaveProperty(miniGameProperty, "release", _editingEnumData["release"]);

            SaveProperty(miniGameProperty, "brotliWasm", _editingBooleanData["brotliWasm"]);
            SaveProperty(miniGameProperty, "brotliData", _editingBooleanData["brotliData"]);
            SaveProperty(miniGameProperty, "streamRes", _editingBooleanData["streamRes"]);
        }

        /// <summary>
        /// 从配置文件读入属性字段
        /// </summary>
        private static T ReadProperty<T>(SerializedProperty miniGameProperty, string propertyName)
        {
            var property = miniGameProperty.FindPropertyRelative(propertyName);
            if (property == null)
                return default;

            var rt = typeof(T);

            if (property.isArray && property.arrayElementType == "string")
            {
                var arr = new string[property.arraySize];
                for (var i = 0; i < property.arraySize; ++i)
                {
                    arr[i] = property.GetArrayElementAtIndex(i).stringValue;
                }

                return (T)(object)string.Join("\n", arr);
            }

            if (rt == typeof(bool))
            {
                return (T)(object)property.boolValue;
            }

            if (rt == typeof(int))
            {
                return (T)(object)property.intValue;
            }

            if (rt == typeof(float))
            {
                return (T)(object)property.floatValue;
            }

            if (rt == typeof(string))
            {
                return (T)(object)property.stringValue;
            }

            throw new Exception($"Unsupported property type with ReadProperty<{typeof(T).FullName}>.");
        }

        /// <summary>
        /// 保存属性字段值到配置文件
        /// </summary>
        private static void SaveProperty<T>(SerializedProperty miniGameProperty, string propertyName, T value)
        {
            var property = miniGameProperty.FindPropertyRelative(propertyName);
            var rt = typeof(T);

            if (property.isArray && property.arrayElementType == "string")
            {
                var raw = (string)(object)value;
                var arr = raw.Split('\n');
                arr = arr.Where(x => x.Length > 0).ToArray(); // 移除其中的空字符串
                property.arraySize = arr.Length;
                for (var i = 0; i < property.arraySize; ++i)
                {
                    var elm = property.GetArrayElementAtIndex(i);
                    elm.stringValue = arr[i];
                    elm.stringValue = elm.stringValue.Trim();
                }
            }
            else if (rt == typeof(bool))
            {
                property.boolValue = (bool)(object)value;
            }
            else if (rt == typeof(int))
            {
                property.intValue = (int)(object)value;
            }
            else if (rt == typeof(float))
            {
                property.floatValue = (float)(object)value;
            }
            else if (rt == typeof(string))
            {
                property.stringValue = (string)(object)value;
            }
            else
            {
                throw new Exception($"Unsupported property type with SaveProperty<{typeof(T).FullName}>.");
            }
        }

        #endregion
    }
}
#endif