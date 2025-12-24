var WebGLInput = {
    $WebGLMap: {
        lastId: 0,
        /* Event listeners */
        onKeyboardInput: null,
        onKeyboardConfirm: null,
        onKeyboardComplete: null,
    },

    WebGLInputInitEvent: function (OnChange, OnConfirm, OnComplete) {
        function _removeEvent() {
            qg.offKeyboardInput(WebGLMap.onKeyboardInput);
            qg.offKeyboardConfirm(WebGLMap.onKeyboardConfirm);
            qg.offKeyboardComplete(WebGLMap.onKeyboardComplete);
        }

        WebGLMap.onKeyboardInput = function (res) {
            console.log("[unity]", "onKeyboardInput", JSON.stringify(res))
            dynCall_vii(OnChange, WebGLMap.lastId, getPtrFromString(res.value));
        };
        WebGLMap.onKeyboardComplete = function (res) {
            console.log("[unity]", "onKeyboardComplete", JSON.stringify(res))
            dynCall_vii(OnComplete, WebGLMap.lastId, getPtrFromString(res.value));
            _removeEvent();
        };
        WebGLMap.onKeyboardConfirm = function (res) {
            console.log("[unity]", "onKeyboardConfirm", JSON.stringify(res))
            dynCall_vii(OnConfirm, WebGLMap.lastId, getPtrFromString(res.value));
        };

        function getPtrFromString(str) {
            var bufferSize = lengthBytesUTF8(str) + 1;
            var buffer = _malloc(bufferSize);
            stringToUTF8(str, buffer, bufferSize);
            return buffer;
        }
    },

    WebGLInputBeginEditing: function (id, text, maxLength, multiple, confirmHold) {
        WebGLMap.lastId = -1;

        (function _addEvent() {
            qg.onKeyboardInput(WebGLMap.onKeyboardInput);
            qg.onKeyboardConfirm(WebGLMap.onKeyboardConfirm);
            qg.onKeyboardComplete(WebGLMap.onKeyboardComplete);
        })();

        qg.showKeyboard({
            defaultValue: UTF8ToString(text),
            maxLength: maxLength,
            multiple: multiple == 1,
            confirmHold: confirmHold == 1,
            confirmType: "done",
            success: function (res) {
                WebGLMap.lastId = id;
                console.log("[unity]", "show keyboard success", JSON.stringify(res))
            },
            fail: function (res) {
                console.error("[unity]", "show keyboard failure: ", JSON.stringify(res));
            },
            complete: function () {
                console.log("showKeyboard complete");
            },
        });
    },

    WebGLInputEndEditing: function () {
        qg.hideKeyboard({
            success: function (res) {
                console.log("[unity]", "hide keyboard success", JSON.stringify(res))
            },
            fail: function (res) {
                console.error("[unity]", "hide keyboard failure", JSON.stringify(res));
            },
            complete: function (e) {
                console.log("hideKeyboard complete");
            },
        });
    },
};

autoAddDeps(WebGLInput, '$WebGLMap');
mergeInto(LibraryManager.library, WebGLInput);