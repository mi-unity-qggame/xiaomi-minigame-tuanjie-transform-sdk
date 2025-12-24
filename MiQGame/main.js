require('./qgame-adapter.js')
import {UnityModule} from "./build.framework"

window["toolVersion"] = ""
var localData = true
var localWasm = true
var wasmBrotli = false
var dataBrotli = false
var gameName = ""
var gameVersion = ""
var fileCachePath = "internal://files/unity/"
var dataFileMd5 = ""
var wasmFileMd5 = ""
var loadingUrl = ""
var dataUrl = ""
var wasmUrl = ""
var MB = 1024 * 1024
var constFileLimitSize = 5 * MB
var dataFileCount = 0
var wasmFileCount = 0
var constDataCacheKey = "key_" + dataFileMd5
var constWasmCacheKey = "key_" + wasmFileMd5

console.log(`${gameName} ${gameVersion}`)
console.log("mi-toolVersion", toolVersion)

var canShowLoading = true
var canShowLoadingHandler = null

function printLog(log) {
    console.log("[unity]", log)
}

const fileManager = qg.getFileSystemManager()

var jsonData = "{\"preRun\":[null],\"postRun\":[null],\"canvas\":{},\"webglContextAttributes\":{\"preserveDrawingBuffer\":false,\"powerPreference\":2},\"streamingAssetsUrl\":null,\"downloadProgress\":{\"dataUrl\":{\"started\":true,\"finished\":true,\"lengthComputable\":true,\"total\":5379814,\"loaded\":5379814}},\"deinitializers\":[null,null],\"intervals\":{},\"disabledCanvasEvents\":[\"contextmenu\",\"dragstart\"],\"dataUrl\":\"Build/build.data\",\"frameworkUrl\":\"Build/build.framework.js\",\"codeUrl\":\"Build/build.wasm\",\"companyName\":\"DefaultCompany\",\"productName\":\"XMQuickGame\",\"productVersion\":\"0.1\",\"SystemInfo\":{\"width\":1080,\"height\":1920,\"userAgent\":\"\",\"browser\":\"Unknown browser\",\"browserVersion\":\"Unknown version\",\"mobile\":true,\"os\":\"Unknown OS\",\"osVersion\":\"Unknown OS Version\",\"gpu\":\"ANGLE (Intel(R) Iris(R) Xe Graphics Direct3D11 vs_5_0 ps_5_0)\",\"language\":\"cn\",\"hasWebGL\":2,\"hasCursorLock\":false,\"hasFullscreen\":true,\"hasThreads\":true,\"hasWasm\":true,\"hasWasmThreads\":false}}";
let configFile = fileManager.readFileSync("/subpackage/config.json", "utf8");
if (configFile.data !== null && configFile.data !== undefined) {
    jsonData = configFile.data
}

var c = JSON.parse(jsonData)
var unityInstance = null
c["postRun"].push(function () {
    canShowLoading = false
    if (c["SendMessage"] === undefined || c["SendMessage"] === null) {
        printLog("SendMessage == null")
    } else {
        printLog("SendMessage != null")
        unityInstance = {
            Module: c,
            SetFullscreen: function () {
                if (c.SetFullscreen)
                    return c.SetFullscreen.apply(c, arguments);
                c.print("Failed to set Fullscreen mode: Player not loaded yet.");
            },
            SendMessage: function () {
                if (c.SendMessage)
                    return c.SendMessage.apply(c, arguments);
                c.print("Failed to execute SendMessage: Player not loaded yet.");
            },
            Quit: function () {
                return new Promise(function (e, t) {
                    (c.shouldQuit = !0), (c.onQuit = e);
                });
            },
        }
        window["UnityInstance"] = unityInstance
        if (window.UnityInstance === undefined || window.UnityInstance === null) {
            printLog("window.UnityInstance == null")
        } else {
            printLog("window.UnityInstance != null")
        }
    }
})

c.streamingAssetsUrl = "https://x.x.x.x/StreamingAssets"
c.dataUrl = "http://x.x.x.x/Build/build.data"
c.frameworkUrl = "http://x.x.x.x/Build/build.framework.js"
c.codeUrl = "http://x.x.x.x/Build/build.wasm"

c.setInterval = function (e, t) {
    e = window.setInterval(e, t);
    return (c.intervals[e] = !0), e;
}

c.clearInterval = function (e) {
    delete c.intervals[e], window.clearInterval(e);
}

function x(e, t) {
    if ("symbolsUrl" != e) {
        var r = c.downloadProgress[e],
            n =
                ((r =
                    r ||
                    (c.downloadProgress[e] = {
                        started: !1,
                        finished: !1,
                        lengthComputable: !1,
                        total: 0,
                        loaded: 0,
                    })),
                "object" != typeof t ||
                ("progress" != t.type && "load" != t.type) ||
                (r.started ||
                ((r.started = !0), (r.lengthComputable = t.lengthComputable)),
                    (r.total = t.total),
                    (r.loaded = t.loaded),
                "load" == t.type && (r.finished = !0)),
                    0),
            o = 0,
            a = 0,
            s = 0,
            i = 0;
        for (e in c.downloadProgress) {
            if (!(r = c.downloadProgress[e]).started) return;
            a++,
                r.lengthComputable
                    ? ((n += r.loaded), (o += r.total), s++)
                    : r.finished || i++;
        }
        //d(0.9 * (a ? (a - i - (o ? (s * (o - n)) / o : 0)) / a : 0));
    }
}

function progress1(e, t) {
    var r = function () { };
    return (
        t && t.onProgress && (r = t.onProgress),
            fetch(e, t).then(function (e) {
                return c.readBodyWithProgress(e, r, t.enableStreamingDownload);
            })
    );
}

window["showloading"] = function () {
    if (qg.showGameInternalSplashLoading !== undefined) {
        qg.showGameInternalSplashLoading({
            url: loadingUrl
        })
    }
}
var loadingShow = false
window["showloading"]()

window["hideloading"] = function () {
    if (loadingShow) {
        return;
    }
    loadingShow = true;
    if (qg.hideGameInternalSplashLoading !== undefined) {
        qg.hideGameInternalSplashLoading()
    }
}

//两分钟自动销毁loading，保底逻辑，基本不可能走到这里！！！
var hideLoadingTimer = setTimeout(() => {
    console.log("自动销毁loading页面！")
    hideLoadingTimer = null
    window["hideloading"]()
}, 120 * 1000)

c.readBodyWithProgress = function (a, s, i) {
    var e = a.body ? a.body.getReader() : void 0,
        d = void 0 !== a.headers.get("Content-Length"),
        l = (function (e, t) {
            if (!t) return 0;
            var t = e.headers.get("Content-Encoding"),
                r = parseInt(e.headers.get("Content-Length"));
            switch (t) {
                case "br":
                    return Math.round(5 * r);
                case "gzip":
                    return Math.round(4 * r);
                default:
                    return r;
            }
        })(a, d),
        u = new Uint8Array(l),
        c = [],
        h = 0,
        f = 0;
    return (
        d ||
        console.warn(
            "[UnityCache] Response is served without Content-Length header. Please reconfigure server to include valid Content-Length for better download performance."
        ),
            (function o() {
                return void 0 === e
                    ? a.arrayBuffer().then(function (e) {
                        var t = new Uint8Array(e);
                        return (
                            s({
                                type: "progress",
                                response: a,
                                total: e.length,
                                loaded: 0,
                                lengthComputable: d,
                                chunk: i ? t : null,
                            }),
                                t
                        );
                    })
                    : e.read().then(function (e) {
                        if (e.done) {
                            if (h === l) return u;
                            if (h < l) return u.slice(0, h);
                            for (
                                var t = new Uint8Array(h), r = (t.set(u, 0), f), n = 0;
                                n < c.length;
                                ++n
                            )
                                t.set(c[n], r), (r += c[n].length);
                            return t;
                        }
                        return (
                            h + e.value.length <= u.length
                                ? (u.set(e.value, h), (f = h + e.value.length))
                                : c.push(e.value),
                                (h += e.value.length),
                                s({
                                    type: "progress",
                                    response: a,
                                    total: Math.max(l, h),
                                    loaded: h,
                                    lengthComputable: d,
                                    chunk: i ? e.value : null,
                                }),
                                o()
                        );
                    });
            })().then(function (e) {
                return (
                    s({
                        type: "load",
                        response: a,
                        total: e.length,
                        loaded: e.length,
                        lengthComputable: d,
                        chunk: null,
                    }),
                        (a.parsedBody = e),
                        a
                );
            })
    );
}

var r = "dataUrl", e, t, n

c.preRun.push(function () {
    var ec = c
    printLog("unity preRun")
    var data = ec["unitydata"]

    let e = new Uint8Array(data, 0, data.byteLength);
    var t = new DataView(data, data.byteOffset, data.byteLength);

    var r = 0;
    var n = "UnityWebData1.0\0";
    if (!String.fromCharCode.apply(null, e.subarray(r, r + n.length)) == n) {
        throw "unknown data format";
    }
    var o = t.getUint32((r += n.length), !0);
    for (r += 4; r < o;) {
        var a = t.getUint32(r, !0),
            s = ((r += 4), t.getUint32(r, !0)),
            i = ((r += 4), t.getUint32(r, !0)),
            d = ((r += 4), String.fromCharCode.apply(null, e.subarray(r, r + i)));
        r += i;
        for (
            var l = 0, u = d.indexOf("/", l) + 1;
            0 < u;
            l = u, u = d.indexOf("/", l) + 1
        )
            c.FS_createPath(d.substring(0, l), d.substring(l, u - 1), !0, !0);
        c.FS_createDataFile(d, null, e.subarray(a, a + s), !0, !0, !0);
        console.log("FS_createDataFile", d)
    }
});

function ensureDirectoryExists(dirPath) {
    return new Promise((resolve, reject) => {
        var fileAccess = fileManager.accessSync(dirPath);
        if (fileAccess === "Response { code=0 content=success }") {
            // console.log(`${dirPath} 目录已存在`);
            resolve();
            return;
        }

        console.log(`${dirPath} 目录不存在, 进行创建`);
        try {
            fileManager.mkdirSync(dirPath, true);
            resolve();
        } catch (e) {
            console.error("[unity] mkdirSync failure", e);
            reject(e);
        }
    })
}

function saveFileAsync(filePath, content, retry = 3) {
    return new Promise(async (resolve, reject) => {
        let attempts = 0;

        const tryWrite = () => {
            if (attempts > 0)
                console.log(`save ${filePath}, attempt ${attempts + 1}`);
            fileManager.writeFile({
                filePath: filePath,
                data: content,
                encoding: "binary",
                append: false,
                success: function () {
                    console.log(`save ${filePath} success`);
                    resolve();
                },
                fail: function (res) {
                    attempts++;
                    if (attempts < retry) {
                        console.log(`save failed, trying again... ${filePath}: ` + JSON.stringify(res));
                        setTimeout(tryWrite, 500); // 延迟重试
                    } else {
                        console.warn(`save ${filePath} failed! ` + JSON.stringify(res));
                        reject(JSON.stringify(res));
                    }
                }
            });
        };

        tryWrite();
    })
}

function saveFileSync(path, content, append) {
    console.log("append:", append)
    fileManager.writeFileSync(path, content, "binary", append)
    fileManager.saveFileSync()
}

function mergeArrayBuffers(buffers) {
    let totalLength = buffers.reduce((acc, buffer) => acc + buffer.byteLength, 0);
    let combinedBuffer = new ArrayBuffer(totalLength);
    let combinedView = new DataView(combinedBuffer);

    var index = 0
    var haveSetLen = 0


    buffers.forEach(function (buffer) {
        buffer = new ArrayBuffer(buffer)
        let sourceView = new DataView(buffer);
        console.log("source view")

        //复制数据
        for (let i = 0; i < buffer.byteLength; i++) {
            combinedView.setUint8(haveSetLen + i, sourceView.getUint8(i));
        }
        index += 1
        haveSetLen += buffer.byteLength;
    });
    return combinedBuffer;
}

function concatenateArrayBuffers(arrayBuffers) {
    let totalLength = 0;
    for (const buffer of arrayBuffers) {
        totalLength += buffer.byteLength;
    }

    // 创建一个新的ArrayBuffer来存储所有数据的合并结果
    const result = new Uint8Array(totalLength);
    let offset = 0;

    for (const buffer of arrayBuffers) {
        // 将每个ArrayBuffer的内容复制到新的ArrayBuffer中
        result.set(new Uint8Array(buffer), offset);
        offset += buffer.byteLength;
    }

    return result.buffer;
}

function readSplitFileData(filePath, count) {
    console.log("readSplitFileData: ", filePath, count);

    const bufferList = [];
    for (let index = 0; index < count; index++) {
        const curFile = filePath + index.toString();

        var fileAccess = fileManager.accessSync(curFile)
        if (fileAccess === "Response { code=0 content=success }") {
            let readBuffer = fileManager.readFileSync(curFile, "binary");

            if (readBuffer.data instanceof ArrayBuffer) {
                bufferList.push(readBuffer.data);
            } else {
                console.warn(`读取到无效数据: ${curFile}`, typeof readBuffer);
                return null;
            }
        } else {
            console.warn(`File not found or access error for ${curFile}:`, fileAccess);
            return null;
        }
    }

    const combinedBuffer = concatenateArrayBuffers(bufferList);
    if (combinedBuffer !== null) {
        console.log("cache buffer len: ", combinedBuffer.byteLength)
    }
    return combinedBuffer;
}

function writeSplitFileData(path, content, splitCount) {
    return new Promise(async (resolve, reject) => {
        var dataLen = content.byteLength
        var saveSuccessCount = 0

        try {
            // 确保目录存在
            await ensureDirectoryExists(fileCachePath);
            console.log("目录检查完成,开始缓存文件...");

            for (let index = 0; index < splitCount; index++) {
                const maxSize = Math.min((index + 1) * constFileLimitSize, dataLen);
                const fileBuffer = content.slice(index * constFileLimitSize, maxSize);
                const fullPath = path + index.toString();

                try {
                    await saveFileAsync(fullPath, fileBuffer);
                    saveSuccessCount += 1;
                    if (saveSuccessCount === splitCount) {
                        console.log("所有文件缓存完成");
                        resolve();
                    }
                } catch (err) {
                    console.warn(`缓存文件 ${fullPath} 失败,停止缓存`);
                    reject(err);
                    return;
                }
            }
        } catch (e) {
            console.warn("writeSplitFileData failure:" + e);
            reject(e);
        }
    })
}

c["AssistWasmCacheFunc"] = saveFileSync

function loadData() {
    return new Promise((resolve, reject) => {
        //data加载前时间戳
        var startTime = performance.now();
        var loadTime = 0;

        if (localData) {
            if (!dataBrotli) {
                try {
                    var buffer = fileManager.readFileSync('/subpackage/unity/build.data', "binary")
                    c["unitydata"] = buffer.data
                    console.log("load unity data success", buffer.data.byteLength)

                    //data加载后时间戳
                    let endTime = performance.now();
                    loadTime = endTime - startTime;
                    console.warn("data本地加载耗时: ", loadTime, "ms");

                    resolve(true)
                } catch (e) {
                    console.error("read unity data failed!", e)
                    reject(e)
                }
            } else {
                fileManager.readCompressedFile({
                    filePath: "/subpackage/unity/build.data.br",
                    compressionAlgorithm: "br",
                    success(res) {
                        console.log("readCompressedFile data.br success");
                        c["unitydata"] = res.data;
                        console.log("load unity data success", res.data.byteLength);

                        //data加载后时间戳
                        let endTime = performance.now();
                        loadTime = endTime - startTime;
                        console.warn("data.br本地加载和解压耗时: ", loadTime, "ms");

                        resolve(true);
                    },
                    fail(res) {
                        console.error("readCompressedFile fail", res);
                        reject(false);
                    }
                });
            }
        } else {
            const cacheFilePath = fileCachePath + dataFileMd5 + ".data"
            var count = localStorage.getItem(constDataCacheKey)
            var cacheLen = localStorage.getItem(constDataCacheKey + "_len")

            var dataBuffer = new ArrayBuffer(0);

            if (count && cacheLen) {
                dataFileCount = parseInt(count);
                cacheLen = parseInt(cacheLen);

                dataBuffer = readSplitFileData(cacheFilePath, dataFileCount);
            }

            if (dataBuffer === null || dataBuffer === undefined || dataBuffer.byteLength !== cacheLen) {
                if (dataBrotli) {
                    const downPath = fileCachePath + "build.data.br";
                    console.log(`[开始下载data.br资源] ${dataUrl} → ${downPath}`);

                    qg.downloadFile({
                        url: dataUrl,
                        filePath: downPath,
                        success: () => {
                            console.log("unity data.br download success!", dataUrl)

                            //data下载后时间戳
                            let endDownloadTime = performance.now();
                            loadTime = endDownloadTime - startTime;
                            console.warn("data.br下载耗时: ", loadTime, "ms");

                            fileManager.readCompressedFile({
                                filePath: downPath,
                                compressionAlgorithm: "br",
                                success(res) {
                                    console.log("readCompressedFile data.br success");
                                    c["unitydata"] = res.data;
                                    console.log("load unity data success", res.data.byteLength);

                                    //data加载后时间戳
                                    let endTime = performance.now();
                                    loadTime = endTime - startTime;
                                    console.warn("data.br加载和解压耗时: ", loadTime, "ms");

                                    var dataLen = c["unitydata"].byteLength;
                                    var spliceCount = Math.ceil(dataLen / constFileLimitSize);
                                    writeSplitFileData(cacheFilePath, c["unitydata"], spliceCount).then(() => {
                                        console.log("cache data completed!", spliceCount)
                                        localStorage.setItem(constDataCacheKey, spliceCount)
                                        localStorage.setItem(constDataCacheKey + "_len", dataLen)
                                    }).catch((err) => {
                                        console.warn("cache data failure:", err);
                                        reject(err);
                                    });

                                    resolve(true)
                                },
                                fail(res) {
                                    console.error("readCompressedFile fail", res);
                                    reject(false);
                                }
                            });
                        },
                        fail: (e) => {
                            console.error(`[下载失败] ${dataUrl}: ${JSON.stringify(e)}`);
                            reject(false);
                        }
                    });
                } else {
                    console.log(`[开始下载data资源] ${dataUrl}`)
                    var dataClient = new XMLHttpRequest();
                    dataClient.open("GET", dataUrl);
                    dataClient.responseType = "arraybuffer";
                    dataClient.onreadystatechange = () => {
                        if (dataClient.readyState === 4) {
                            console.log("unity data request success!")

                            //data下载后时间戳
                            let endDownloadTime = performance.now();
                            loadTime = endDownloadTime - startTime;
                            console.warn("data下载耗时: ", loadTime, "ms");

                            c["unitydata"] = dataClient.response;

                            //data加载后时间戳
                            let endTime = performance.now();
                            loadTime = endTime - endDownloadTime;
                            console.warn("data加载耗时: ", loadTime, "ms");

                            var dataLen = c["unitydata"].byteLength;
                            var spliceCount = Math.ceil(dataLen / constFileLimitSize);
                            writeSplitFileData(cacheFilePath, c["unitydata"], spliceCount).then(() => {
                                console.log("cache data completed!", spliceCount)
                                localStorage.setItem(constDataCacheKey, spliceCount)
                                localStorage.setItem(constDataCacheKey + "_len", dataLen)
                            }).catch((err) => {
                                console.warn("cache data failure:", err);
                                reject(err);
                            });

                            resolve(true)
                        }
                    }
                    dataClient.send();
                }
            } else {
                console.log("data cache existed, load success!")
                c["unitydata"] = dataBuffer;

                //data加载后时间戳
                let endTime = performance.now();
                loadTime = endTime - startTime;
                console.warn("data缓存加载耗时: ", loadTime, "ms");

                resolve(true)
            }
        }
    });
}

function loadWasm(url) {
    return new Promise((resolve, reject) => {
        //wasm加载前时间戳
        var startTime = performance.now();
        var loadTime = 0;

        if (localWasm) {
            if (!wasmBrotli) {
                try {
                    var buffer = fileManager.readFileSync('/subpackage/unity/build.wasm', "binary")
                    c["wasmBinary"] = buffer.data
                    console.log("load unity wasm success", buffer.data.byteLength)

                    //wasm加载后时间戳
                    let endTime = performance.now();
                    loadTime = endTime - startTime;
                    console.warn("wasm本地加载耗时: ", loadTime, "ms");

                    resolve(true)
                } catch (e) {
                    console.error("read unity wasm failed!", e)
                    reject(e)
                }
            } else {
                fileManager.readCompressedFile({
                    filePath: "/subpackage/unity/build.wasm.br",
                    compressionAlgorithm: "br",
                    success(res) {
                        console.log("readCompressedFile wasm.br success");
                        c["wasmBinary"] = res.data;
                        console.log("load unity wasm success", res.data.byteLength);

                        //wasm加载后时间戳
                        let endTime = performance.now();
                        loadTime = endTime - startTime;
                        console.warn("wasm.br本地加载和解压耗时: ", loadTime, "ms");

                        resolve(true);
                    },
                    fail(res) {
                        console.error("readCompressedFile fail", res);
                        reject(false);
                    }
                });
            }
        } else {
            const cacheFilePath = fileCachePath + wasmFileMd5 + ".wasm"
            var count = localStorage.getItem(constWasmCacheKey)
            var cacheLen = localStorage.getItem(constWasmCacheKey + "_len")

            var wasmBuffer = new ArrayBuffer(0);

            if (count && cacheLen) {
                wasmFileCount = parseInt(count);
                cacheLen = parseInt(cacheLen);

                wasmBuffer = readSplitFileData(cacheFilePath, wasmFileCount);
            }

            if (wasmBuffer === null || wasmBuffer === undefined || wasmBuffer.byteLength !== cacheLen) {
                if (wasmBrotli) {
                    const downPath = fileCachePath + "build.wasm.br";
                    console.log(`[开始下载wasm.br资源] ${url} → ${downPath}`);

                    qg.downloadFile({
                        url: url,
                        filePath: downPath,
                        success: () => {
                            console.log("unity wasm.br download success!", url)

                            //wasm下载后时间戳
                            let endDownloadTime = performance.now();
                            loadTime = endDownloadTime - startTime;
                            console.warn("wasm.br下载耗时: ", loadTime, "ms");

                            fileManager.readCompressedFile({
                                filePath: downPath,
                                compressionAlgorithm: "br",
                                success(res) {
                                    console.log("readCompressedFile wasm.br success");
                                    c["wasmBinary"] = res.data;
                                    console.log("load unity wasm success", res.data.byteLength);

                                    //wasm加载后时间戳
                                    let endTime = performance.now();
                                    loadTime = endTime - startTime;
                                    console.warn("wasm.br加载和解压耗时: ", loadTime, "ms");

                                    var wasmLen = c["wasmBinary"].byteLength;
                                    var spliceCount = Math.ceil(wasmLen / constFileLimitSize);
                                    writeSplitFileData(cacheFilePath, c["wasmBinary"], spliceCount).then(() => {
                                        console.log("cache wasm completed!")
                                        localStorage.setItem(constWasmCacheKey, spliceCount)
                                        localStorage.setItem(constWasmCacheKey + "_len", wasmLen)
                                    }).catch((err) => {
                                        console.warn("cache wasm failure:", err);
                                        reject(err);
                                    });

                                    resolve(true);
                                },
                                fail(res) {
                                    console.error("readCompressedFile fail", res);
                                    reject(false);
                                }
                            });
                        },
                        fail: (e) => {
                            console.error(`[下载失败] ${url}: ${JSON.stringify(e)}`);
                            reject(false);
                        }
                    });
                } else {
                    console.log(`[开始下载wasm资源] ${url}`);
                    var wasmClient = new XMLHttpRequest();
                    wasmClient.open("GET", url);
                    wasmClient.responseType = "arraybuffer";
                    wasmClient.onreadystatechange = () => {
                        if (wasmClient.readyState === 4) {
                            console.log("unity wasm request success!")

                            //wasm下载后时间戳
                            let endDownloadTime = performance.now();
                            loadTime = endDownloadTime - startTime;
                            console.warn("wasm下载耗时: ", loadTime, "ms");

                            c["wasmBinary"] = wasmClient.response;

                            //wasm加载后时间戳
                            let endTime = performance.now();
                            loadTime = endTime - endDownloadTime;
                            console.warn("wasm加载耗时: ", loadTime, "ms");

                            var wasmLen = c["wasmBinary"].byteLength;
                            var spliceCount = Math.ceil(wasmLen / constFileLimitSize);
                            writeSplitFileData(cacheFilePath, c["wasmBinary"], spliceCount).then(() => {
                                console.log("cache wasm completed!")
                                localStorage.setItem(constWasmCacheKey, spliceCount)
                                localStorage.setItem(constWasmCacheKey + "_len", wasmLen)
                            }).catch((err) => {
                                console.warn("cache wasm failure:", err);
                                reject(err);
                            });

                            resolve(true)
                        }
                    }
                    wasmClient.send();
                }
            } else {
                console.log("wasm cache existed, load success!")
                c["wasmBinary"] = wasmBuffer;

                //wasm加载后时间戳
                let endTime = performance.now();
                loadTime = endTime - startTime;
                console.warn("wasm缓存加载耗时: ", loadTime, "ms");

                resolve(true)
            }
        }
    });
}

function execUnity() {
    c.canvas = canvas;
    Promise.all([loadData(), loadWasm(wasmUrl)]).then(results => {
        if (results[0] && results[1]) {
            UnityModule(c)
        } else {
            console.error("加载untiy资源失败！请检查unity文件夹下资源大小,如果资源太大,请放到服务器加载!")
        }
    }).catch(error => {
        console.log("加载资源失败", error)
    })
}

execUnity()