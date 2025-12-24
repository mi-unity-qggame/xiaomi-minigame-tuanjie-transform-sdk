using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace mi
{
    [Serializable]
    public class AudioParam
    {
        public float startTime;

        public bool autoplay;

        public bool loop;

        public float volume;

        public string url;
    }

    public class QGBasePlayer
    {

        public static Dictionary<string, QGBasePlayer> QGPlayers = new Dictionary<string, QGBasePlayer>();

        public string playerId;
        public Action onPlayAction;
        public Action onCanPlayAction;
        public Action onPauseAction;
        public Action onStopAction;
        public Action onEndedAction;
        public Action onTimeUpdateAction;
        public Action onErrorAction;
        public Action onWaitingAction;
        public Action onSeekingAction;
        public Action onSeekedAction;

        public QGBasePlayer(string playerId)
        {
            this.playerId = playerId;
            QGPlayers.Add(playerId, this);
        }

        public virtual void Play()
        {
            MiAudioPlayer.PlayMedia(playerId);
        }

        public virtual void Pause()
        {
            MiAudioPlayer.PauseMedia(playerId);
        }

        public virtual void Stop()
        {
            MiAudioPlayer.StopMedia(playerId);
        }

        public virtual void Seek(float time)
        {
            MiAudioPlayer.SeekMedia(playerId, time);
        }

        public void Destroy()
        {
            MiAudioPlayer.DestroyMedia(playerId);
            QGPlayers.Remove(playerId);
        }

        public virtual void OnPlay(Action onPlay)
        {
            onPlayAction += onPlay;
        }

        public virtual void OffPlay(Action offPlay)
        {
            onPlayAction -= offPlay;
        }

        public virtual void OnCanPlay(Action onCanPlay)
        {
            onCanPlayAction += onCanPlay;
        }

        public virtual void OffCanPlay(Action offCanPlay)
        {
            onCanPlayAction -= offCanPlay;
        }

        public virtual void OnPause(Action onPause)
        {
            onPauseAction += onPause;
        }

        public virtual void OffPause(Action offPause)
        {
            onPauseAction -= offPause;
        }

        public virtual void OnStop(Action onStop)
        {
            onStopAction += onStop;
        }

        public virtual void OffStop(Action offStop)
        {
            onStopAction -= offStop;
        }

        public virtual void OnEnded(Action onEnded)
        {
            onEndedAction += onEnded;
        }

        public virtual void OffEnded(Action offEnded)
        {
            onEndedAction -= offEnded;
        }
        public virtual void OnTimeUpdate(Action onTimeUpdate)
        {
            onTimeUpdateAction += onTimeUpdate;
        }

        public virtual void OffTimeUpdate(Action offTimeUpdate)
        {
            onTimeUpdateAction -= offTimeUpdate;
        }
        public virtual void OnError(Action onError)
        {
            onErrorAction += onError;
        }

        public virtual void OffError(Action offError)
        {
            onErrorAction -= offError;
        }
        public virtual void OnWaiting(Action onWaiting)
        {
            onWaitingAction += onWaiting;
        }

        public virtual void OffWaiting(Action offWaiting)
        {
            onWaitingAction -= offWaiting;
        }
        public virtual void OnSeeking(Action onSeeking)
        {
            onSeekingAction += onSeeking;
        }

        public virtual void OffSeeking(Action offSeeking)
        {
            onSeekingAction -= offSeeking;
        }
        public virtual void OnSeeked(Action onSeeked)
        {
            onSeekedAction += onSeeked;
        }

        public virtual void OffSeeked(Action offSeeked)
        {
            onSeekedAction -= offSeeked;
        }
    }

    public class QGAudioPlayer : QGBasePlayer
    {
        public QGAudioPlayer(string playerId) : base(playerId)
        {

        }
    }

    public class MiAudioPlayer
    {
        private static int id = 0;

        [DllImport("__Internal")]
        private static extern void QGPlayMedia(string a);
        [DllImport("__Internal")]
        private static extern void QGPauseMedia(string a);
        [DllImport("__Internal")]
        private static extern void QGStopMedia(string a);
        [DllImport("__Internal")]
        private static extern void QGDestroyMedia(string a);
        [DllImport("__Internal")]
        private static extern void QGSeekMedia(string a, float time);
        [DllImport("__Internal")]
        private static extern void QGPlayAudio(string a, string b);

        private static int GenarateId()
        {
            if (id > 1000000)
            {
                id = 0;
            }
            id++;

            return id;
        }

        public static string GetKey()
        {
            int id = GenarateId();
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var time = Convert.ToInt64(ts.TotalSeconds);
            return (time.ToString() + '-' + id);
        }

        public static QGAudioPlayer PlayAudio(AudioParam param)
        {
            var playerId = GetKey();
            Debug.Log("playerId: " + playerId);
            QGAudioPlayer ap = new QGAudioPlayer(playerId);
            QGPlayAudio(playerId, JsonUtility.ToJson(param));
            return ap;
        }

        public static void PlayMedia(string playerId)
        {
            QGPlayMedia(playerId);
        }

        public static void PauseMedia(string playerId)
        {
            QGPauseMedia(playerId);
        }

        public static void StopMedia(string playerId)
        {
            QGStopMedia(playerId);
        }

        public static void DestroyMedia(string playerId)
        {
            QGDestroyMedia(playerId);
        }

        public static void SeekMedia(string playerId, float time)
        {
            QGSeekMedia(playerId, time);
        }

        public void pdOnPlayCallBack(string msg)
        {
            var res = JsonUtility.FromJson<QGBaseResponse>(msg);
            var pd = QGBasePlayer.QGPlayers[res.callbackId];
            if (pd != null)
            {
                pd.onPlayAction?.Invoke();
            }
        }

        public void pdOnCanPlayCallBack(string msg)
        {
            var res = JsonUtility.FromJson<QGBaseResponse>(msg);
            var pd = QGBasePlayer.QGPlayers[res.callbackId];
            if (pd != null)
            {
                pd.onCanPlayAction?.Invoke();
            }
        }

        public void pdOnPauseCallBack(string msg)
        {
            var res = JsonUtility.FromJson<QGBaseResponse>(msg);
            var pd = QGBasePlayer.QGPlayers[res.callbackId];
            if (pd != null)
            {
                pd.onPauseAction?.Invoke();
            }
        }

        public void pdOnStopCallBack(string msg)
        {
            var res = JsonUtility.FromJson<QGBaseResponse>(msg);
            var pd = QGBasePlayer.QGPlayers[res.callbackId];
            if (pd != null)
            {
                pd.onStopAction?.Invoke();
            }
        }

        public void pdOnEndedCallBack(string msg)
        {
            var res = JsonUtility.FromJson<QGBaseResponse>(msg);
            var pd = QGBasePlayer.QGPlayers[res.callbackId];
            if (pd != null)
            {
                pd.onEndedAction?.Invoke();
            }
        }

        public void pdOnTimeUpdateCallBack(string msg)
        {
            var res = JsonUtility.FromJson<QGBaseResponse>(msg);
            var pd = QGBasePlayer.QGPlayers[res.callbackId];
            if (pd != null)
            {
                pd.onTimeUpdateAction?.Invoke();
            }
        }

        public void pdOnErrorCallBack(string msg)
        {
            var res = JsonUtility.FromJson<QGBaseResponse>(msg);
            var pd = QGBasePlayer.QGPlayers[res.callbackId];
            if (pd != null)
            {
                pd.onErrorAction?.Invoke();
            }
        }

        public void pdOnWaitingCallBack(string msg)
        {
            var res = JsonUtility.FromJson<QGBaseResponse>(msg);
            var pd = QGBasePlayer.QGPlayers[res.callbackId];
            if (pd != null)
            {
                pd.onWaitingAction?.Invoke();
            }
        }

        public void pdOnSeekingCallBack(string msg)
        {
            var res = JsonUtility.FromJson<QGBaseResponse>(msg);
            var pd = QGBasePlayer.QGPlayers[res.callbackId];
            if (pd != null)
            {
                pd.onSeekingAction?.Invoke();
            }
        }

        public void pdOnSeekedCallBack(string msg)
        {
            var res = JsonUtility.FromJson<QGBaseResponse>(msg);
            var pd = QGBasePlayer.QGPlayers[res.callbackId];
            if (pd != null)
            {
                pd.onSeekedAction?.Invoke();
            }
        }
    }
}
