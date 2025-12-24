var MiVideo = {
    $CONSTANT: {
        ACTION_CALL_BACK_CLASS_NAME_DEFAULT: "MiVideo",
        ACTION_CALL_BACK_METHOD_NAME_VIDEO_PLAY: "videoOnPlayCallBack",
        ACTION_CALL_BACK_METHORD_NAME_VIDEO_PAUSE: "videoOnPauseCallBack",
        ACTION_CALL_BACK_METHORD_NAME_VIDEO_ENDED: "videoOnEndedCallBack",
        ACTION_CALL_BACK_METHORD_NAME_VIDEO_TIMEUPDATE: "videoOnTimeUpdateCallBack",
        ACTION_CALL_BACK_METHORD_NAME_VIDEO_ERROR: "videoOnErrorCallBack",
        ACTION_CALL_BACK_METHORD_NAME_VIDEO_WAITING: "videoOnWaitingCallBack",
    },

    $mVideoMap: {},

    //创建视频
    QGCreateVideo: function (playerId, param) {
        console.log("[unity]", "QGCreateVideo , playerId: ", UTF8ToString(playerId));
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }
        var paramStr = UTF8ToString(param);
        var paramData = JSON.parse(paramStr);
        console.log("[unity]", "paramData: ", paramData);
        var video = qg.createVideo(paramData)
        if (!(mVideoMap instanceof Map)) {
            mVideoMap = new Map();
        }
        var pdIdStr = UTF8ToString(playerId);
        mVideoMap.set(pdIdStr, video);

        video.onProgress(function (res) {
            console.log("[unity]", "onProgress", res);
            var json = JSON.stringify({
                callbackId: pdIdStr,
            });
            if (res.buffered == 100) {
                window.UnityInstance.SendMessage(
                    "MiVideo",
                    "videoOnProgressCallBack",
                    json
                );
            }
        });
    },

    //播放视频
    QGPlayVideo: function (playerId) {
        if (!(mVideoMap instanceof Map)) {
            mVideoMap = new Map();
        }
        console.log("[unity]", "QGPlayVideo , playerId: ", UTF8ToString(playerId));
        var pdIdStr = UTF8ToString(playerId);
        var video = mVideoMap.get(pdIdStr);
        if (!video || video == null || video == "undefined") {
            console.log("[unity]", "video is null");
            return;
        }

        video.play();

        video.onPlay(function () {
            console.log("[unity]", "onPlay");
            var json = JSON.stringify({
                callbackId: pdIdStr,
            });
            window.UnityInstance.SendMessage(
                "MiVideo",
                "videoOnPlayCallBack",
                json
            );
        });

        video.onPause(function () {
            console.log("[unity]", "onPause");
            var json = JSON.stringify({
                callbackId: pdIdStr,
            });
            window.UnityInstance.SendMessage(
                "MiVideo",
                "videoOnPauseCallBack",
                json
            );
        });

        video.onEnded(function () {
            console.log("[unity]", "onEnded");
            var json = JSON.stringify({
                callbackId: pdIdStr,
            });
            window.UnityInstance.SendMessage(
                "MiVideo",
                "videoOnEndedCallBack",
                json
            );
        });

        video.onTimeUpdate(function (res) {
            console.log("[unity]", "onTimeUpdate", res);
            var json = JSON.stringify({
                callbackId: pdIdStr,
            });
            window.UnityInstance.SendMessage(
                "MiVideo",
                "videoOnTimeUpdateCallBack",
                json
            );
        });

        video.onError(function (res) {
            console.log("[unity]", "onError", res);
            var json = JSON.stringify({
                callbackId: pdIdStr,
                errMsg: res.errMsg,
            });
            window.UnityInstance.SendMessage(
                "MiVideo",
                "videoOnErrorCallBack",
                json
            );
        });

        video.onWaiting(function () {
            console.log("[unity]", "onWaiting");
            var json = JSON.stringify({
                callbackId: pdIdStr,
            });
            window.UnityInstance.SendMessage(
                "MiVideo",
                "videoOnWaitingCallBack",
                json
            );
        });
    },

    //暂停视频
    QGPauseVideo: function (playerId) {
        if (!(mVideoMap instanceof Map)) {
            mVideoMap = new Map();
        }
        console.log("[unity]", "QGPauseVideo , playerId: ", UTF8ToString(playerId));
        var pdIdStr = UTF8ToString(playerId);
        var video = mVideoMap.get(pdIdStr);
        if (!video || video == null || video == "undefined") {
            console.log("[unity]", "video is null");
            return;
        }

        video.pause();
    },

    //停止视频
    QGStopVideo: function (playerId) {
        if (!(mVideoMap instanceof Map)) {
            mVideoMap = new Map();
        }
        console.log("[unity]", "QGStopVideo , playerId: ", UTF8ToString(playerId));
        var pdIdStr = UTF8ToString(playerId);
        var video = mVideoMap.get(pdIdStr);
        if (!video || video == null || video == "undefined") {
            console.log("[unity]", "video is null");
            return;
        }

        video.stop();
    },

    //视频跳转
    QGSeekVideo: function (playerId, time) {
        if (!(mVideoMap instanceof Map)) {
            mVideoMap = new Map();
        }
        console.log("[unity]", "QGSeekVideo, playerId: ", UTF8ToString(playerId), "time: ", time);
        var pdIdStr = UTF8ToString(playerId);
        var video = mVideoMap.get(pdIdStr);
        if (!video || video == null || video == "undefined") {
            console.log("[unity]", "video is null");
            return;
        }

        video.seek(time);
    },

    //视频全屏
    QGRequestFullScreenVideo: function (playerId, direction) {
        if (!(mVideoMap instanceof Map)) {
            mVideoMap = new Map();
        }
        console.log("[unity]", "QGRequestFullScreenVideo , playerId: ", UTF8ToString(playerId));
        var pdIdStr = UTF8ToString(playerId);
        var video = mVideoMap.get(pdIdStr);
        if (!video || video == null || video == "undefined") {
            console.log("[unity]", "video is null");
            return;
        }

        video.requestFullScreen(direction);
    },

    //视频退出全屏
    QGExitFullScreenVideo: function (playerId) {
        if (!(mVideoMap instanceof Map)) {
            mVideoMap = new Map();
        }
        console.log("[unity]", "QGExitFullScreenVideo , playerId: ", UTF8ToString(playerId));
        var pdIdStr = UTF8ToString(playerId);
        var video = mVideoMap.get(pdIdStr);
        if (!video || video == null || video == "undefined") {
            console.log("[unity]", "video is null");
            return;
        }

        video.exitFullScreen();
    },

    //销毁视频
    QGDestroyVideo: function (playerId) {
        if (!(mVideoMap instanceof Map)) {
            mVideoMap = new Map();
        }
        console.log("[unity]", "QGDestroyVideo , playerId: ", UTF8ToString(playerId));
        var pdIdStr = UTF8ToString(playerId);
        var video = mVideoMap.get(pdIdStr);
        if (!video || video == null || video == "undefined") {
            console.log("[unity]", "video is null");
            return;
        }

        video.destroy();
    },
};

autoAddDeps(MiVideo, "$CONSTANT");
autoAddDeps(MiVideo, "$mVideoMap");

mergeInto(LibraryManager.library, MiVideo);
