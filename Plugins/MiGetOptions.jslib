var MiGetOptions = {
    $mOptionsMap: {
        lastId: 0,
        /* Event listeners */
        onShow: null,
        onHide: null,
    },

    MiGetOptionsInitEvent: function (id, onShow, onHide) {
        mOptionsMap.lastId = id;

        mOptionsMap.onShow = function (res) {
            console.log("[unity]", "onShow", JSON.stringify(res))
            dynCall_vii(onShow, mOptionsMap.lastId, getPtrFromString(JSON.stringify(res)));
        };
        mOptionsMap.onHide = function () {
            console.log("[unity]", "onHide")
            dynCall_vi(onHide, mOptionsMap.lastId);
        };

        function getPtrFromString(str) {
            var bufferSize = lengthBytesUTF8(str) + 1;
            var buffer = _malloc(bufferSize);
            stringToUTF8(str, buffer, bufferSize);
            return buffer;
        }
    },

    // 监听游戏切入前台事件
    MiGetOptionsOnShow: function () {
        qg.onShow(mOptionsMap.onShow);
    },

    // 监听游戏切入后台事件
    MiGetOptionsOnHide: function () {
        qg.onHide(mOptionsMap.onHide);
    },

    // 取消监听游戏切入前台
    MiGetOptionsOffShow: function () {
        qg.offShow(mOptionsMap.onShow);
    },

    // 取消监听游戏切入后台事件
    MiGetOptionsOffHide: function () {
        qg.offHide(mOptionsMap.onHide);
    },

    //获取快游戏启动时的参数
    QGGetLaunchOptionsSync: function (cmd) {
        console.log("[unity]", "QGGetLaunchOptionsSync")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }

        var launchInfo = qg.getLaunchOptionsSync()
        if (launchInfo === "undefined" || launchInfo === null) {
            console.log("[unity]", "get launch info failure")
            return
        } else {
            // const serializedObject = JSON.stringify(launchInfo);
            // const encoded = new TextEncoder().encode(serializedObject);
            // const arrayBuffer = new ArrayBuffer(encoded.length);
            // const byteArray = new Uint8Array(arrayBuffer);
            // byteArray.set(encoded);
            // const decoder = new TextDecoder();
            // const text = decoder.decode(byteArray);
            var response = JSON.stringify(launchInfo)
            console.log("[unity]", "get launch info success", response)

            var bufferSize = lengthBytesUTF8(response) + 1;
            var buffer = _malloc(bufferSize);
            stringToUTF8(response, buffer, bufferSize);
            return buffer;
        }
    },
};

autoAddDeps(MiGetOptions, '$mOptionsMap');
mergeInto(LibraryManager.library, MiGetOptions);