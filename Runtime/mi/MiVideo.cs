using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace mi
{
    public class QGVideoBase
    {
        public static Dictionary<string, QGVideoBase> QGVideos = new Dictionary<string, QGVideoBase>();
        public string playerId;
        public Action onPlayAction;
        public Action onPauseAction;
        public Action onEndedAction;
        public Action onProgressAction;
        public Action onTimeUpdateAction;
        public Action<string> onErrorAction;
        public Action onWaitingAction;

        public QGVideoBase(string playerId)
        {
            this.playerId = playerId;
            QGVideos.Add(playerId, this);
        }
    }

    public class QGVideo : QGVideoBase
    {
        public QGVideo(string playerId) : base(playerId)
        {
        }

        /// <summary>
        /// 播放视频
        /// </summary>
        public void Play()
        {
            MiVideo.Instance.PlayVideo(playerId);
        }

        /// <summary>
        /// 暂停视频
        /// </summary>
        public void Pause()
        {
            MiVideo.Instance.PauseVideo(playerId);
        }

        /// <summary>
        /// 停止视频
        /// </summary>
        public void Stop()
        {
            MiVideo.Instance.StopVideo(playerId);
        }

        /// <summary>
        /// 视频跳转
        /// </summary>
        /// <param name="time">视频跳转到指定位置，单位为 s 秒</param>
        public void Seek(float time)
        {
            MiVideo.Instance.SeekVideo(playerId, time);
        }

        /// <summary>
        /// 视频全屏
        /// </summary>
        /// <param name="direction">设置全屏时视频的方向，0: 正常竖向，90：屏幕逆时针90度，-90：屏幕顺时针90度</param>
        public void RequestFullScreen(int direction = 0)
        {
            MiVideo.Instance.RequestFullScreenVideo(playerId, direction);
        }

        /// <summary>
        /// 视频退出全屏
        /// </summary>
        public void ExitFullScreen()
        {
            MiVideo.Instance.ExitFullScreenVideo(playerId);
        }

        /// <summary>
        /// 销毁视频
        /// </summary>
        public void Destroy()
        {
            MiVideo.Instance.DestroyVideo(playerId);
            QGVideos.Remove(playerId);
        }

        /// <summary>
        /// 监听视频播放事件
        /// </summary>
        public void OnPlay(Action onPlay)
        {
            onPlayAction += onPlay;
        }

        /// <summary>
        /// 移除视频播放事件的监听函数
        /// </summary>
        public void OffPlay(Action offPlay)
        {
            onPlayAction -= offPlay;
        }

        /// <summary>
        /// 监听视频暂停事件
        /// </summary>
        public void OnPause(Action onPause)
        {
            onPauseAction += onPause;
        }

        /// <summary>
        /// 移除视频暂停事件的监听函数
        /// </summary>
        public void OffPause(Action offPause)
        {
            onPauseAction -= offPause;
        }

        /// <summary>
        /// 监听视频播放到末尾事件
        /// </summary>
        public virtual void OnEnded(Action onEnded)
        {
            onEndedAction += onEnded;
        }

        /// <summary>
        /// 移除视频播放到末尾事件的监听函数
        /// </summary>
        public virtual void OffEnded(Action offEnded)
        {
            onEndedAction -= offEnded;
        }

        /// <summary>
        /// 视频下载（缓冲）事件的监听函数
        /// </summary>
        public virtual void OnProgress(Action onProgress)
        {
            onProgressAction += onProgress;
        }

        /// <summary>
        /// 移除视频下载（缓冲）事件的监听函数
        /// </summary>
        public virtual void OffProgress(Action offProgress)
        {
            onProgressAction -= offProgress;
        }

        /// <summary>
        /// 监听视频播放进度更新事件
        /// </summary>
        public virtual void OnTimeUpdate(Action onTimeUpdate)
        {
            onTimeUpdateAction += onTimeUpdate;
        }

        /// <summary>
        /// 移除视频播放进度更新事件的监听函数
        /// </summary>
        public virtual void OffTimeUpdate(Action offTimeUpdate)
        {
            onTimeUpdateAction -= offTimeUpdate;
        }

        /// <summary>
        /// 监听视频错误事件
        /// </summary>
        public virtual void OnError(Action<string> onError)
        {
            onErrorAction += onError;
        }

        /// <summary>
        /// 移除视频错误事件的监听函数
        /// </summary>
        public virtual void OffError(Action<string> offError)
        {
            onErrorAction -= offError;
        }

        /// <summary>
        /// 监听视频由于需要缓冲下一帧而停止时触发
        /// </summary>
        public virtual void OnWaiting(Action onWaiting)
        {
            onWaitingAction += onWaiting;
        }

        /// <summary>
        /// 移除视频由于需要缓冲下一帧而停止时触发的监听函数
        /// </summary>
        public virtual void OffWaiting(Action offWaiting)
        {
            onWaitingAction -= offWaiting;
        }
    }

    public class MiVideo : MonoBehaviour
    {
        private static MiVideo instance = null;
        public static MiVideo Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject(typeof(MiVideo).Name).AddComponent<MiVideo>();
                    DontDestroyOnLoad(instance.gameObject);
                }
                return instance;
            }
        }

        private static int id = 0;

        #region js函数

        [DllImport("__Internal")]
        private static extern void QGCreateVideo(string id, string param);
        [DllImport("__Internal")]
        private static extern void QGPlayVideo(string id);
        [DllImport("__Internal")]
        private static extern void QGPauseVideo(string id);
        [DllImport("__Internal")]
        private static extern void QGStopVideo(string id);
        [DllImport("__Internal")]
        private static extern void QGSeekVideo(string id, float time);
        [DllImport("__Internal")]
        private static extern void QGRequestFullScreenVideo(string id, int direction);
        [DllImport("__Internal")]
        private static extern void QGExitFullScreenVideo(string id);
        [DllImport("__Internal")]
        private static extern void QGDestroyVideo(string id);

        #endregion

        private static int GenarateId()
        {
            if (id > 1000000)
            {
                id = 0;
            }

            id++;

            return id;
        }

        private static string GetKey()
        {
            int id = GenarateId();
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var time = Convert.ToInt64(ts.TotalSeconds);
            return (time.ToString() + '-' + id);
        }

        /// <summary>
        /// 创建视频
        /// </summary>
        /// <returns></returns>
        public QGVideo CreateVideo(QGCreateVideoParam param)
        {
            var playerId = GetKey();
            Debug.Log("playerId: " + playerId);
            QGVideo video = new QGVideo(playerId);
            QGCreateVideo(playerId, JsonUtility.ToJson(param));
            return video;
        }

        #region js 函数

        public void PlayVideo(string playerId)
        {
            QGPlayVideo(playerId);
        }

        public void PauseVideo(string playerId)
        {
            QGPauseVideo(playerId);
        }

        public void StopVideo(string playerId)
        {
            QGStopVideo(playerId);
        }

        public void SeekVideo(string playerId, float time)
        {
            QGSeekVideo(playerId, time);
        }

        public void RequestFullScreenVideo(string playerId, int direction)
        {
            QGRequestFullScreenVideo(playerId, direction);
        }

        public void ExitFullScreenVideo(string playerId)
        {
            QGExitFullScreenVideo(playerId);
        }

        public void DestroyVideo(string playerId)
        {
            QGDestroyVideo(playerId);
        }

        #endregion

        #region 事件回调

        public void videoOnPlayCallBack(string msg)
        {
            MiBridge.Instance.QGLog("videoOnPlayCallBack: " + msg);
            var res = JsonUtility.FromJson<QGBaseResponse>(msg);
            QGVideoBase video = QGVideoBase.QGVideos[res.callbackId];
            if (video != null)
            {
                video.onPlayAction?.Invoke();
            }
        }

        public void videoOnPauseCallBack(string msg)
        {
            MiBridge.Instance.QGLog("videoOnPauseCallBack: " + msg);
            var res = JsonUtility.FromJson<QGBaseResponse>(msg);
            QGVideoBase video = QGVideoBase.QGVideos[res.callbackId];
            if (video != null)
            {
                video.onPauseAction?.Invoke();
            }
        }

        public void videoOnEndedCallBack(string msg)
        {
            MiBridge.Instance.QGLog("videoOnEndedCallBack: " + msg);
            var res = JsonUtility.FromJson<QGBaseResponse>(msg);
            QGVideoBase video = QGVideoBase.QGVideos[res.callbackId];
            if (video != null)
            {
                video.onEndedAction?.Invoke();
            }
        }

        public void videoOnProgressCallBack(string msg)
        {
            MiBridge.Instance.QGLog("videoOnProgressCallBack: " + msg);
            var res = JsonUtility.FromJson<QGBaseResponse>(msg);
            QGVideoBase video = QGVideoBase.QGVideos[res.callbackId];
            if (video != null)
            {
                video.onProgressAction?.Invoke();
            }
        }

        public void videoOnTimeUpdateCallBack(string msg)
        {
            MiBridge.Instance.QGLog("videoOnTimeUpdateCallBack: " + msg);
            var res = JsonUtility.FromJson<QGBaseResponse>(msg);
            QGVideoBase video = QGVideoBase.QGVideos[res.callbackId];
            if (video != null)
            {
                video.onTimeUpdateAction?.Invoke();
            }
        }

        public void videoOnErrorCallBack(string msg)
        {
            MiBridge.Instance.QGLog("videoOnErrorCallBack: " + msg);
            var res = JsonUtility.FromJson<QGBaseResponse>(msg);
            QGVideoBase video = QGVideoBase.QGVideos[res.callbackId];
            if (video != null)
            {
                video.onErrorAction?.Invoke(res.errMsg);
            }
        }

        public void videoOnWaitingCallBack(string msg)
        {
            MiBridge.Instance.QGLog("videoOnWaitingCallBack: " + msg);
            var res = JsonUtility.FromJson<QGBaseResponse>(msg);
            QGVideoBase video = QGVideoBase.QGVideos[res.callbackId];
            if (video != null)
            {
                video.onWaitingAction?.Invoke();
            }
        }

        #endregion
    }
}