#if TUANJIE_1_4_OR_NEWER
using UnityEditor.Build.Profile;
using UnityEngine;

namespace Mi
{
    internal class XiaomiMiniGameSettings : MiniGameSettings
    {
        [SerializeField] public string gameName = "";
        [SerializeField] public string packageName = "";
        [SerializeField] public string version = "";
        [SerializeField] public int versionCode = 0;
        [SerializeField] public string loadingBG = "";
        [SerializeField] public int landscape = 0;
        [SerializeField] public int isWebGL2 = 0;
        [SerializeField] public int removeOpenSSLProvider = 0;
        [SerializeField] public int release = 0;
        [SerializeField] public string signPath = "";
        [SerializeField] public bool brotliWasm = true;
        [SerializeField] public bool brotliData = true;
        [SerializeField] public string dataPath = "";
        [SerializeField] public string wasmPath = "";
        [SerializeField] public bool streamRes = false;
        [SerializeField] public string streamAssetPath = "";

        public XiaomiMiniGameSettings(MiniGameSettingsEditor editor) : base(editor)
        {
        }
    }
}
#endif