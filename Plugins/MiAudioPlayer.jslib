var QgGameBridge = {
    $CONSTANT: {
        ACTION_CALL_BACK_CLASS_NAME_DEFAULT: "MiAudioPlayer",
        ACTION_CALL_BACK_METHORD_NAME_DEFAULT: "DefaultResponseCallback",
        ACTION_CALL_BACK_METHORD_NAME_AD_ERROR: "AdOnErrorCallBack",
        ACTION_CALL_BACK_METHORD_NAME_AD_LOAD: "AdOnLoadCallBack",
        //ACTION_CALL_BACK_METHORD_NAME_AD_SHOW: 'AdOnShowCallBack',
        ACTION_CALL_BACK_METHORD_NAME_AD_CLOSE: "AdOnCloseCallBack",
        ACTION_CALL_BACK_METHORD_NAME_AD_HIDE: "AdOnHideCallBack",
        ACTION_CALL_BACK_METHORD_NAME_AD_CLOSE_REWARDED: "RewardedVideoAdOnCloseCallBack",
        ACTION_CALL_BACK_METHORD_NAME_AD_LOAD_NATIVE: "NativeAdOnLoadCallBack",
        ACTION_CALL_BACK_METHORD_NAME_PD_PLAY: "pdOnPlayCallBack",
        ACTION_CALL_BACK_METHORD_NAME_PD_CANPLAY: "pdOnCanPlayCallBack",
        ACTION_CALL_BACK_METHORD_NAME_PD_PAUSE: "pdOnPauseCallBack",
        ACTION_CALL_BACK_METHORD_NAME_PD_STOP: "pdOnStopCallBack",
        ACTION_CALL_BACK_METHORD_NAME_PD_ENDED: "pdOnEndedCallBack",
        ACTION_CALL_BACK_METHORD_NAME_PD_TIMEUPDATE: "pdOnTimeUpdateCallBack",
        ACTION_CALL_BACK_METHORD_NAME_PD_ERROR: "pdOnErrorCallBack",
        ACTION_CALL_BACK_METHORD_NAME_PD_WAITING: "pdOnWaitingCallBack",
        ACTION_CALL_BACK_METHORD_NAME_PD_SEEKING: "pdOnSeekingCallBack",
        ACTION_CALL_BACK_METHORD_NAME_PD_SEEKED: "pdOnSeekedCallBack",
    },
    $mAdMap: {},

    $mFileData: {},

    $mKeyBoardData: null,

    QGPlayAudio: function (playerId, param) {
        console.log("[unity]", "QGPlayAudio");
        var paramStr = UTF8ToString(param);
        var paramData = JSON.parse(paramStr);
        var innerAudioContext = qg.createInnerAudioContext();
        innerAudioContext.startTime = paramData.startTime ? paramData.startTime : 0;
        innerAudioContext.autoplay = paramData.autoplay ? paramData.autoplay : false;
        innerAudioContext.loop = paramData.loop ? paramData.loop : false;
        innerAudioContext.autoplay = paramData.autoplay ? paramData.autoplay : false;
        innerAudioContext.volume = paramData.volume ? paramData.volume : 1;
        innerAudioContext.src = paramData.url;
        innerAudioContext.play();
        if (!(mAdMap instanceof Map)) {
            mAdMap = new Map();
        }
        var pdIdStr = UTF8ToString(playerId);
        mAdMap.set(pdIdStr, innerAudioContext);
    },

    QGPlayMedia: function (playerId) {
        if (!(mAdMap instanceof Map)) {
            mAdMap = new Map();
        }
        console.log("[unity]", "QGPlayMedia , playerId: ", playerId);
        var pdIdStr = UTF8ToString(playerId);
        var pd = mAdMap.get(pdIdStr);
        if (!pd || pd == null || pd == "undefined") {
            console.log("[unity]", "innerAudioContext is null");
            return;
        }

        pd.play();
        pd.onCanplay(function () {
            console.log("[unity]", "onCanplay success");
            var json = JSON.stringify({
                callbackId: pdIdStr,
            });
            unityInstance.SendMessage(
                CONSTANT.ACTION_CALL_BACK_CLASS_NAME_DEFAULT,
                CONSTANT.ACTION_CALL_BACK_METHORD_NAME_PD_CANPLAY,
                json
            );
        });

        pd.onPlay(function () {
            console.log("[unity]", "onPlay success");
            var json = JSON.stringify({
                callbackId: pdIdStr,
            });
            unityInstance.SendMessage(
                CONSTANT.ACTION_CALL_BACK_CLASS_NAME_DEFAULT,
                CONSTANT.ACTION_CALL_BACK_METHORD_NAME_PD_PLAY,
                json
            );
        });

        pd.onPause(function () {
            console.log("[unity]", "onPause success");
            var json = JSON.stringify({
                callbackId: pdIdStr,
            });
            unityInstance.SendMessage(
                CONSTANT.ACTION_CALL_BACK_CLASS_NAME_DEFAULT,
                CONSTANT.ACTION_CALL_BACK_METHORD_NAME_PD_PAUSE,
                json
            );
        });

        pd.onStop(function () {
            console.log("[unity]", "onStop success");
            var json = JSON.stringify({
                callbackId: pdIdStr,
            });
            unityInstance.SendMessage(
                CONSTANT.ACTION_CALL_BACK_CLASS_NAME_DEFAULT,
                CONSTANT.ACTION_CALL_BACK_METHORD_NAME_PD_STOP,
                json
            );
        });

        pd.onEnded(function () {
            console.log("[unity]", "onEndedfunction success");
            var json = JSON.stringify({
                callbackId: pdIdStr,
            });
            unityInstance.SendMessage(
                CONSTANT.ACTION_CALL_BACK_CLASS_NAME_DEFAULT,
                CONSTANT.ACTION_CALL_BACK_METHORD_NAME_PD_ENDED,
                json
            );
        });

        pd.onTimeUpdate(function () {
            console.log("[unity]", "onTimeUpdate success");
            var json = JSON.stringify({
                callbackId: pdIdStr,
            });
            unityInstance.SendMessage(
                CONSTANT.ACTION_CALL_BACK_CLASS_NAME_DEFAULT,
                CONSTANT.ACTION_CALL_BACK_METHORD_NAME_PD_TIMEUPDATE,
                json
            );
        });

        pd.onError(function (res) {
            console.log("[unity]", "onError success:", res);
            var json = JSON.stringify({
                callbackId: pdIdStr,
                errMsg: res.errMsg,
                errCode: res.errCode,
            });
            unityInstance.SendMessage(
                CONSTANT.ACTION_CALL_BACK_CLASS_NAME_DEFAULT,
                CONSTANT.ACTION_CALL_BACK_METHORD_NAME_PD_ERROR,
                json
            );
        });

        pd.onWaiting(function () {
            console.log("[unity]", "onWaiting success");
            var json = JSON.stringify({
                callbackId: pdIdStr,
            });
            unityInstance.SendMessage(
                CONSTANT.ACTION_CALL_BACK_CLASS_NAME_DEFAULT,
                CONSTANT.ACTION_CALL_BACK_METHORD_NAME_PD_WAITING,
                json
            );
        });

        pd.onSeeking(function () {
            console.log("[unity]", "onSeeking success");
            var json = JSON.stringify({
                callbackId: pdIdStr,
            });
            unityInstance.SendMessage(
                CONSTANT.ACTION_CALL_BACK_CLASS_NAME_DEFAULT,
                CONSTANT.ACTION_CALL_BACK_METHORD_NAME_PD_SEEKING,
                json
            );
        });

        pd.onSeeked(function () {
            console.log("[unity]", "onSeeked success");
            var json = JSON.stringify({
                callbackId: pdIdStr,
            });
            unityInstance.SendMessage(
                CONSTANT.ACTION_CALL_BACK_CLASS_NAME_DEFAULT,
                CONSTANT.ACTION_CALL_BACK_METHORD_NAME_PD_SEEKED,
                json
            );
        });
    },

    QGPauseMedia: function (playerId) {
        console.log("[unity]", "QGPauseMedia");
        if (typeof qg == "undefined") {
            console.log("[unity]", "qg.minigame.jslib  qg is undefined");
            return;
        }
        if (!(mAdMap instanceof Map)) {
            mAdMap = new Map();
        }
        var pdIdStr = UTF8ToString(playerId);
        var pd = mAdMap.get(pdIdStr);
        if (pd) {
            pd.pause();
        }
    },

    QGStopMedia: function (playerId) {
        console.log("[unity]", "QGStopMedia");
        if (typeof qg == "undefined") {
            console.log("[unity]", "qg.minigame.jslib  qg is undefined");
            return;
        }
        if (!(mAdMap instanceof Map)) {
            mAdMap = new Map();
        }
        var pdIdStr = UTF8ToString(playerId);
        var pd = mAdMap.get(pdIdStr);
        if (pd) {
            pd.stop();
        }
    },

    QGDestroyMedia: function (playerId) {
        console.log("[unity]", "QGDestroyMedia");
        if (typeof qg == "undefined") {
            console.log("[unity]", "qg.minigame.jslib  qg is undefined");
            return;
        }
        if (!(mAdMap instanceof Map)) {
            mAdMap = new Map();
        }
        var pdIdStr = UTF8ToString(playerId);
        var pd = mAdMap.get(pdIdStr);
        if (pd) {
            pd.destroy();
        }
    },

    QGSeekMedia: function (playerId, time) {
        console.log("[unity]", "QGSeekMedia");
        if (typeof qg == "undefined") {
            console.log("[unity]", "qg.minigame.jslib  qg is undefined");
            return;
        }
        if (!(mAdMap instanceof Map)) {
            mAdMap = new Map();
        }
        var pdIdStr = UTF8ToString(playerId);
        var pd = mAdMap.get(pdIdStr);
        if (pd) {
            pd.seek(time);
        }
    },
};

autoAddDeps(QgGameBridge, "$mAdMap");
autoAddDeps(QgGameBridge, "$CONSTANT");
autoAddDeps(QgGameBridge, "$mFileData");

mergeInto(LibraryManager.library, QgGameBridge);
