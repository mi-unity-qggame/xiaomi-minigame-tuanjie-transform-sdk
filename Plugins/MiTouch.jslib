var MiTouch = {
    $MiTouchMap: {
        onTouchStart: null,
        onTouchMove: null,
        onTouchEnd: null,
        onTouchCancel: null,
    },

    MiTouchInitEvent: function (OnTouchStart, OnTouchMove, OnTouchEnd, OnTouchCancel) {
        function _removeAllEvents() {
            qg.offTouchStart(MiTouchMap.onTouchStart);
            qg.offTouchMove(MiTouchMap.onTouchMove);
            qg.offTouchEnd(MiTouchMap.onTouchEnd);
            qg.offTouchCancel(MiTouchMap.onTouchCancel);
        }

        function getPtrFromString(str) {
            var bufferSize = lengthBytesUTF8(str) + 1;
            var buffer = _malloc(bufferSize);
            stringToUTF8(str, buffer, bufferSize);
            return buffer;
        }

        MiTouchMap.onTouchStart = function (res) {
            // console.log("[unity]", "onTouchStart", JSON.stringify(res))
            var json = JSON.stringify(res);
            dynCall_vi(OnTouchStart, getPtrFromString(json));
        };

        MiTouchMap.onTouchMove = function (res) {
            var json = JSON.stringify(res);
            dynCall_vi(OnTouchMove, getPtrFromString(json));
        };

        MiTouchMap.onTouchEnd = function (res) {
            var json = JSON.stringify(res);
            dynCall_vi(OnTouchEnd, getPtrFromString(json));
        };

        MiTouchMap.onTouchCancel = function (res) {
            var json = JSON.stringify(res);
            dynCall_vi(OnTouchCancel, getPtrFromString(json));
        };

        // remove previous
        _removeAllEvents();

        // register
        qg.onTouchStart(MiTouchMap.onTouchStart);
        qg.onTouchMove(MiTouchMap.onTouchMove);
        qg.onTouchEnd(MiTouchMap.onTouchEnd);
        qg.onTouchCancel(MiTouchMap.onTouchCancel);

        console.log("[MiTouch] JS Touch listeners initialized");
    },

    MiTouchRemoveAll: function () {
        qg.offTouchStart();
        qg.offTouchMove();
        qg.offTouchEnd();
        qg.offTouchCancel();
        console.log("[MiTouch] JS Touch listeners removed");
    },
};

autoAddDeps(MiTouch, '$MiTouchMap');
mergeInto(LibraryManager.library, MiTouch);