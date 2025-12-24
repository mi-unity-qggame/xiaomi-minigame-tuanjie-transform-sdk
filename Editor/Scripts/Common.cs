#if UNITY_EDITOR
using UnityEngine;
using System;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace Mi
{
    public static class Common
    {
        /// <summary>
        /// 文件路径规范化
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>规范化路径</returns>
        public static string FormatPath(string path)
        {
            path = path.Replace("/", "\\");
            if (Application.platform == RuntimePlatform.OSXEditor)
                path = path.Replace("\\", "/");

            return path;
        }

        /// <summary>
        /// 运行cmd命令
        /// </summary>
        /// <param name="cmd">命令</param>
        /// <param name="args">参数</param>
        /// <param name="workingDir">运行目录</param>
        /// <param name="timeout">超时时长</param>
        /// <returns>string[] res[0]命令的stdout输出, res[1]命令的stderr输出</returns>
        public static string[] RunCmd(string cmd, string args, string workingDir = "", int timeout = 60)
        {
            string[] res = new string[2];
            using (Process myProcess = CreateCmdProcess(cmd, args, workingDir))
            {
                // 读取输出流的回调
                var outputBuilder = new System.Text.StringBuilder();
                var errorBuilder = new System.Text.StringBuilder();

                myProcess.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data != null) outputBuilder.AppendLine(e.Data);
                };
                myProcess.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data != null) errorBuilder.AppendLine(e.Data);
                };

                myProcess.Start();
                myProcess.BeginOutputReadLine();
                myProcess.BeginErrorReadLine();

                bool exited = myProcess.WaitForExit(timeout * 1000); // 等待指定毫秒数

                if (!exited)
                {
                    UnityEngine.Debug.LogWarning("进程超时，强制关闭...");
                    myProcess.CloseMainWindow();
                    myProcess.Close();
                }

                res[0] = outputBuilder.ToString();
                res[1] = errorBuilder.ToString();

                return res;
            }
        }

        public static Process CreateCmdProcess(string cmd, string args, string workingDir = "")
        {
            var pStartInfo = new ProcessStartInfo(cmd)
            {
                Arguments = args,
                CreateNoWindow = false,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                StandardErrorEncoding = System.Text.Encoding.UTF8,
                StandardOutputEncoding = System.Text.Encoding.UTF8,
                WindowStyle = ProcessWindowStyle.Normal,
            };

            if (!string.IsNullOrEmpty(workingDir))
                pStartInfo.WorkingDirectory = workingDir;

            return Process.Start(pStartInfo);
        }

        /// <summary>
        /// 获取Package SDK
        /// </summary>
        public static bool TryFindSdkByRelativePath(out string foundSdkPath)
        {
            foundSdkPath = string.Empty;
            try
            {
                string scriptGuid = AssetDatabase.FindAssets($"t:MonoScript {nameof(MiQGameConverter)}")[0];
                string currentScriptPath = AssetDatabase.GUIDToAssetPath(scriptGuid);
                currentScriptPath = Path.GetFullPath(currentScriptPath).Replace("\\", "/");

                // XMSDK 根目录
                string xmsdkRootDir = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(currentScriptPath)));
                foundSdkPath = xmsdkRootDir;

                Debug.Log($"XMSDK：{foundSdkPath}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"获取XMSDK失败：{e.Message}");
                return false;
            }
        }
    }

    public static class MiFileCompressor
    {
        /// <summary>
        /// 使用Brotli同步压缩文件
        /// </summary>
        /// <param name="sourceFilePath">要进行压缩的文件路径</param>
        /// <param name="outputFilePath">压缩后的输出文件路径</param>
        /// <param name="compressionLevel">压缩级别（默认最佳压缩）</param>
        /// <param name="overwrite">是否覆盖已存在的目标文件</param>
        /// <param name="bufferSize">缓冲区大小（默认8MB）</param>
        public static void CompressFileWithBrotli(string sourceFilePath, string outputFilePath, System.IO.Compression.CompressionLevel compressionLevel = System.IO.Compression.CompressionLevel.Optimal, bool overwrite = true, int bufferSize = 8192 * 1024)
        {
#if UNITY_2019 || UNITY_2020
            throw new NotSupportedException("当前Brotli压缩功能仅支持Unity 2021及以上版本，当前版本不支持。");
#else
            // 参数验证
            if (string.IsNullOrWhiteSpace(sourceFilePath))
                throw new ArgumentNullException(nameof(sourceFilePath));

            if (string.IsNullOrWhiteSpace(outputFilePath))
                throw new ArgumentNullException(nameof(outputFilePath));

            if (!File.Exists(sourceFilePath))
                throw new FileNotFoundException($"源压缩文件不存在: {sourceFilePath}", sourceFilePath);

            if (File.Exists(outputFilePath) && !overwrite)
                throw new IOException($"目标文件已存在: {outputFilePath}");

            // 确保输出目录存在
            var outputDir = Path.GetDirectoryName(outputFilePath);
            if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            // 同步流处理
            using (var inputStream = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, FileOptions.SequentialScan))
            using (var outputStream = new FileStream(outputFilePath, overwrite ? FileMode.Create : FileMode.CreateNew, FileAccess.Write, FileShare.None, bufferSize))
            using (var compressor = new BrotliStream(outputStream, compressionLevel, leaveOpen: false))
            {
                inputStream.CopyTo(compressor, bufferSize);
            }

            // 验证输出文件
            if (new FileInfo(outputFilePath).Length == 0)
                throw new InvalidOperationException("压缩失败，输出文件为空");
#endif
        }
    }
}
#endif