var MiBridge = {
    $CONSTANT: {
        CMD_LOGIN: "cmd_push",
        UNITY_FILE_PATH: "internal://files/unity/",
    },

    $mAdMap: {},

    $mFileData: {},

    QGPrint: function (str) {
        console.log("[unity]", UTF8ToString(str))
    },

    QGPrintInt: function (num) {
        console.log("collect-funcs", num)
    },

    QGInit: function () {
        console.log("[unity]", "qg init")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }
        mAdMap = new Map()
        var fileManager = qg.getFileSystemManager()
        fileManager.mkdir({
            dirPath: CONSTANT.UNITY_FILE_PATH,
            success: function () {
                console.log("[unity]", "create unity path success")
            },
            fail: function (res) {
                console.log("[unity]", "create unity path failure " + res.errMsg)
            }
        })
    },

    //登录
    QGLogin: function (cmd) {
        console.log("[unity]", "quick game login")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }
        qg.login({
            success: function (res) {
                console.log("[unity]", "quick game login success:", JSON.stringify(res))
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: 0,
                    msg: "",
                    data: JSON.stringify(res.data),
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            },

            fail: function (res) {
                console.log("[unity]", "quick game login failure", JSON.stringify(res))
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: res.errCode,
                    msg: res.errMsg,
                    data: "",
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            },
            complete: function (res) {
                console.log("[unity]", 'quick game login complete')
            }
        });
    },

    //获取用户信息
    QGGetUserInfo: function (cmd) {
        console.log("[unity]", "quick get user info")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }
        qg.getUserInfo({
            success: function(res) {
                console.log("[unity]", 'nickName:', res.userInfo.nickName)
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: 0,
                    msg: "",
                    data: JSON.stringify({
                        userInfo: res.userInfo,
                    }),
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            },

            fail: function () {
                console.log("[unity]", 'user reject!')
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: -1,
                    msg: "",
                    data: "",
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            },
            complete: function (res) {
                console.log("[unity]", 'get userInfo complete')
            }
        });
    },

    //开启用户信息改变通知
    QGOnUserInfoChanged: function (cmd) {
        console.log("[unity]", "quick on user info changed")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }
        qg.onUserInfoChange(function (res) {
            console.log("[unity]", "on user info changed notify")
            var json = JSON.stringify({
                cmd: cmd,
                ret: 0,
                msg: "",
                data: JSON.stringify({
                    userInfo: res.userInfo,
                }),
            })
            window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
        })
    },

    //关闭用户信息监听
    QGOffUserInfoChanged: function () {
        console.log("[unity]", "quick off user info changed")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }
        qg.offUserInfoChange(function () {
            console.log("[unity]", "quick off user info changed notify")
        })
    },

    //支付
    QGPay: function (cmd, info) {
        console.log("[unity]", "pay, order info=" + UTF8ToString(info))
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }
        var orderInfo = JSON.parse(UTF8ToString(info))
        qg.pay({
            orderInfo: orderInfo,
            success: function (data) {
                console.log("[unity]", "pay success notify:", data)
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: 0,
                    msg: "",
                    data: JSON.stringify(data),
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            },
            fail: function (data) {
                console.log("[unity]", "pay failure:", data)
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: data.resultStatus,
                    msg: data.memo,
                    data: JSON.stringify(data),
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            }
        })
    },

    //判断是否已经创建桌面图标
    QGHasInstalled: function (cmd) {
        console.log("[unity]", "QGHasInstalled")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }
        var shortcut = qg.getShortcut()
        shortcut.hasInstalled({
            success: function (result) {
                console.log("[unity]", "has installed success")
                var retCode = 0
                if (!result) {
                    retCode = -1
                }
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: retCode,
                    msg: "",
                    data: "",
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            },
            fail: function () {
                console.log("[unity]", "has installed failure")
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: -1,
                    msg: "",
                    data: "",
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            }
        })
    },

    //创建桌面图标，每次创建都需要用户授权
    QGInstall: function (cmd, msg) {
        console.log("[unity]", "QGInstall")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }
        var shortcut = qg.getShortcut()
        shortcut.install({
            message: UTF8ToString(msg),
            success: function () {
                console.log("[unity]", "qg install success")
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: 0,
                    msg: "",
                    data: "",
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            },
            fail: function (code) {
                console.log("[unity]", "qg install failure:" + code)
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: code,
                    msg: "",
                    data: "",
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            }
        })
    },

    //横幅广告
    QGCreateBannerAd: function (cmd, left, top, width, height, adId) {
        console.log("[unity]", "QGCreateBannerAd")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return false
        }
        adId = UTF8ToString(adId)
        var ad = qg.createBannerAd({
            adUnitId: adId,
            style: {
                left: left,
                top: top,
                width: width,
                height: height
            }
        })

        if (ad === undefined || ad === null) {
            console.log("[unity]", "create banner ad failed！adId=" + adId)
            return false
        }

        mAdMap.set(adId, ad)
        ad.onLoad(function (res) {
            console.log("[unity]", "qg banner ad load ", JSON.stringify(res))
            var json = JSON.stringify({
                cmd: cmd,
                ret: 0,
                msg: "",
                data: JSON.stringify({
                    type: 0,
                    data: res
                }),
            })
            window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
        })

        ad.onClose(function () {
            console.log("[unity]", "qg banner ad close")
            var json = JSON.stringify({
                cmd: cmd,
                ret: 0,
                msg: "",
                data: JSON.stringify({
                    type: 1,
                    data: ""
                }),
            })
            window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
        })

        ad.onError(function (res) {
            console.log("[unity]", "qg banner ad error", JSON.stringify(res))
            var json = JSON.stringify({
                cmd: cmd,
                ret: res.errCode,
                msg: res.errMsg,
                data: JSON.stringify({
                    type: 2,
                    data: JSON.stringify(res)
                }),
            })
            window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
        })

        return true
    },

    //插屏广告
    QGCreateInterstitialAd: function (cmd, adId) {
        console.log("[unity]", "QGCreateInterstitialAd")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return false
        }
        adId = UTF8ToString(adId)
        var ad = qg.createInterstitialAd({
            adUnitId: adId
        })

        if (ad === undefined || ad === null) {
            console.log("[unity]", "create interstitial ad failed！adId=" + adId)
            return false
        }

        mAdMap.set(adId, ad)
        ad.onLoad(function (res) {
            console.log("[unity]", "qg interstitial ad load ", JSON.stringify(res))
            var json = JSON.stringify({
                cmd: cmd,
                ret: 0,
                msg: "",
                data: JSON.stringify({
                    type: 0,
                    data: JSON.stringify(res)
                }),
            })
            window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
        })

        ad.onClose(function () {
            console.log("[unity]", "qg interstitial ad close")
            var json = JSON.stringify({
                cmd: cmd,
                ret: 0,
                msg: "",
                data: JSON.stringify({
                    type: 1,
                    data: ""
                }),
            })
            window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
        })

        ad.onError(function (res) {
            console.log("[unity]", "qg interstitial ad error", JSON.stringify(res))
            var json = JSON.stringify({
                cmd: cmd,
                ret: res.errCode,
                msg: res.errMsg,
                data: JSON.stringify({
                    type: 2,
                    data: JSON.stringify(res)
                }),
            })
            window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
        })

        return true
    },

    //激励视频广告
    QGCreateRewardedVideoAd: function (cmd, adId) {
        console.log("[unity]", "QGCreateRewardedVideoAd")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return false
        }
        adId = UTF8ToString(adId)
        var ad = qg.createRewardedVideoAd({
            adUnitId: adId
        })

        if (ad === undefined || ad === null) {
            console.log("[unity]", "create reward ad failed！adId=" + adId)
            return false
        }

        mAdMap.set(adId, ad)
        ad.onLoad(function (res) {
            console.log("[unity]", "qg reward ad load ", JSON.stringify(res))
            var json = JSON.stringify({
                cmd: cmd,
                ret: 0,
                msg: "",
                data: JSON.stringify({
                    type: 0,
                    data: JSON.stringify(res)
                }),
            })
            window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
        })

        ad.onClose(function (res) {
            console.log("[unity]", "qg reward ad close")
            var json = JSON.stringify({
                cmd: cmd,
                ret: 0,
                msg: "",
                data: JSON.stringify({
                    type: 1,
                    data: "",
                    isEnd: res.isEnded
                }),
            })
            window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
        })

        ad.onError(function (res) {
            console.log("[unity]", "qg reward ad error", JSON.stringify(res))
            var json = JSON.stringify({
                cmd: cmd,
                ret: res.errCode,
                msg: res.errMsg,
                data: JSON.stringify({
                    type: 2,
                    data: JSON.stringify(res)
                }),
            })
            window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
        })

        return true
    },

    //原生模板广告
    QGCreateCustomAd: function (cmd, left, top, width, height, adId) {
        console.log("[unity]", "QGCreateCustomAd")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return false
        }
        adId = UTF8ToString(adId)
        var ad = qg.createCustomAd({
            adUnitId: adId,
            style: {
                left: left,
                top: top,
                width: width,
                height: height
            }
        })

        if (ad === undefined || ad === null) {
            console.log("[unity]", "create custom ad failed！adId=" + adId)
            return false
        }

        mAdMap.set(adId, ad)
        ad.onLoad(function (res) {
            console.log("[unity]", "qg custom ad load ", JSON.stringify(res))
            var json = JSON.stringify({
                cmd: cmd,
                ret: 0,
                msg: "",
                data: JSON.stringify({
                    type: 0,
                    data: "success",
                }),
            })
            window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
        })

        ad.onClose(function () {
            console.log("[unity]", "qg custom ad close")
            var json = JSON.stringify({
                cmd: cmd,
                ret: 0,
                msg: "",
                data: JSON.stringify({
                    type: 1,
                    data: ""
                }),
            })
            window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
        })

        ad.onError(function (res) {
            console.log("[unity]", "qg custom ad error", JSON.stringify(res))
            var json = JSON.stringify({
                cmd: cmd,
                ret: res.errCode,
                msg: res.errMsg,
                data: JSON.stringify({
                    type: 2,
                    data: JSON.stringify(res)
                }),
            })
            window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
        })

        return true
    },

    //拉取 {激励视频广告}
    QGLoadAd: function (cmd, adId) {
        console.log("[unity]", "QGLoadAd")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }
        adId = UTF8ToString(adId)
        if (!mAdMap.has(adId)) {
            return
        }
        var ad = mAdMap.get(adId)
        if (ad === undefined) {
            return
        }
        if (typeof ad.load === 'function') {
            ad.load()
        } else {
            console.log("[unity] has no load function")
        }
    },

    //展示 {bannerAd & 插屏广告 & 激励视频广告 & 原生模板广告)
    QGShowAd: function (cmd, adId) {
        console.log("[unity]", "QGShowAd")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }
        adId = UTF8ToString(adId)
        if (!mAdMap.has(adId)) {
            return
        }
        var ad = mAdMap.get(adId)
        if (ad === undefined) {
            return
        }
        if (typeof ad.show === 'function') {
            ad.show()
        } else {
            console.log("[unity] has no show function")
        }
    },

    //隐藏 {bannerAd & 原生模板广告}
    QGHideAd: function (cmd, adId) {
        console.log("[unity]", "QGHideAd")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }
        adId = UTF8ToString(adId)
        if (!mAdMap.has(adId)) {
            return
        }
        var ad = mAdMap.get(adId)
        if (ad === undefined) {
            return
        }
        if (typeof ad.hide === 'function') {
            ad.hide()
        } else {
            console.log("[unity] has no hide function")
        }
    },

    //销毁 {bannerAd & 插屏广告 & 原生模板广告}
    QGDestroyAd: function (cmd, adId) {
        console.log("[unity]", "QGDestroyAd")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }
        adId = UTF8ToString(adId)
        if (!mAdMap.has(adId)) {
            return
        }
        var ad = mAdMap.get(adId)
        if (ad === undefined) {
            return
        }
        if (typeof ad.destroy === 'function') {
            ad.destroy()
        } else {
            console.log("[unity] has no destroy function")
        }
    },

    // 展示互推盒子广告
    QGShowRecommendAd: function (cmd, adId, type) {
        console.log("[unity]", "QGShowRecommendAd")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }
        adId = UTF8ToString(adId)
        qg.displayAd({
            type: type,
            upid: adId,
            success: function (res) {
                console.log("[unity]", "qg recommend ad display success:", JSON.stringify(res))
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: res.errCode,
                    msg: res.errMsg,
                    data: "",
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            },
            fail: function (res) {
                console.log("[unity]", "qg recommend ad display failure:", JSON.stringify(res))
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: res.errCode,
                    msg: res.errMsg,
                    data: "",
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            }
        })
    },

    // 关闭互推盒子广告
    QGCloseRecommendAd: function (cmd, adId, style) {
        console.log("[unity]", "QGCloseRecommendAd")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }
        adId = UTF8ToString(adId)
        qg.closeAd({
            type: style,
            upid: adId,
            success: function (res) {
                console.log("[unity]", "qg recommend ad close success:", JSON.stringify(res))
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: res.errCode,
                    msg: res.errMsg,
                    data: "",
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            },
            fail: function (res) {
                console.log("[unity]", "qg recommend ad close failure:", JSON.stringify(res))
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: res.errCode,
                    msg: res.errMsg,
                    data: "",
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            }
        })
    },

    //判断文件/目录是否存在
    QGAccessFile: function (cmd, fileName) {
        console.log("[unity]", "QGAccessFile")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }
        fileName = UTF8ToString(fileName)
        var fileManager = qg.getFileSystemManager()
        var filePath = CONSTANT.UNITY_FILE_PATH + fileName
        fileManager.access({
            path: filePath,
            success: function () {
                console.log("[unity] access file successs", filePath + " file exists")
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: 0,
                    msg: "",
                    data: "",
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            },

            fail: function (res) {
                console.log("[unity] access file failure", res)
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: -1,
                    msg: res,
                    data: "",
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            }
        })
    },

    //判断文件/目录是否存在 的同步版本
    QGAccessFileSync: function (fileName) {
        console.log("[unity]", "QGAccessFileSync")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }
        fileName = UTF8ToString(fileName)
        var filePath = CONSTANT.UNITY_FILE_PATH + fileName
        var fileManager = qg.getFileSystemManager()
        var fileAccess = fileManager.accessSync(filePath)
        //成功 Response { code=0 content=success }' 失败 file does not exists
        if (fileAccess == "Response { code=0 content=success }") {
            console.log("[unity]", "access file success", filePath, "file exists,", fileAccess)
            return true;
        } else {
            console.log("[unity]", "access file failure", filePath, fileAccess)
            return false;
        }
    },

    //在文件结尾追加内容
    QGAppendFile: function (cmd, fileName, encoding, textStr, textData, length) {
        console.log("[unity]", "QGAppendFile")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }
        fileName = UTF8ToString(fileName)
        var filePath = CONSTANT.UNITY_FILE_PATH + fileName
        var encodingStr = UTF8ToString(encoding);
        textStr = UTF8ToString(textStr);
        var content = (encodingStr === "utf8") ? textStr : HEAPU8.slice(textData, length + textData).buffer;

        var fileManager = qg.getFileSystemManager()
        fileManager.appendFile({
            filePath: filePath,
            data: content,
            encoding: encodingStr,
            success: function () {
                console.log("[unity] appendFile success")
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: 0,
                    msg: "",
                    data: "",
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            },
            fail: function (res) {
                console.log("[unity] appendFile failure:", res)
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: -1,
                    msg: res.errMsg,
                    data: "",
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            }
        })
    },

    //在文件结尾追加内容 的同步版本
    QGAppendFileSync: function (fileName, encoding, textStr, textData, length) {
        console.log("[unity]", "QGAppendFileSync")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }
        fileName = UTF8ToString(fileName)
        var filePath = CONSTANT.UNITY_FILE_PATH + fileName
        var encodingStr = UTF8ToString(encoding);
        textStr = UTF8ToString(textStr);
        var content = (encodingStr === "utf8") ? textStr : HEAPU8.slice(textData, length + textData).buffer;

        var fileManager = qg.getFileSystemManager()
        try {
            fileManager.appendFileSync(filePath, content, encodingStr)
        } catch (e) {
            console.error("[unity] appendFileSync failure", e);
        }
    },

    //复制文件
    QGCopyFile: function (cmd, srcPath, destPath) {
        console.log("[unity]", "QGCopyFile")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }
        srcPath = CONSTANT.UNITY_FILE_PATH + UTF8ToString(srcPath)
        destPath = CONSTANT.UNITY_FILE_PATH + UTF8ToString(destPath)
        console.log("[unity] srcPath: " + srcPath + ", destPath: " + destPath);
        var fileManager = qg.getFileSystemManager()
        fileManager.copyFile({
            srcPath: srcPath,
            destPath: destPath,
            success: function () {
                console.log("[unity] copyFile succes");
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: 0,
                    msg: "",
                    data: "",
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            },
            fail: function (res) {
                console.log("[unity] copyFile failure", res);
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: -1,
                    msg: res.errMsg,
                    data: "",
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            }
        })
    },

    //复制文件 的同步版本
    QGCopyFileSync: function (srcPath, destPath) {
        console.log("[unity]", "QGCopyFileSync")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }
        srcPath = CONSTANT.UNITY_FILE_PATH + UTF8ToString(srcPath)
        destPath = CONSTANT.UNITY_FILE_PATH + UTF8ToString(destPath)
        console.log("[unity] srcPath: " + srcPath + ", destPath: " + destPath);
        var fileManager = qg.getFileSystemManager()
        return fileManager.copyFileSync(srcPath, destPath)
    },

    //创建目录
    QGMkDir: function (cmd, filePath) {
        console.log("[unity]", "QGMkDir")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }
        filePath = UTF8ToString(filePath)
        var dirPath = CONSTANT.UNITY_FILE_PATH + filePath
        console.log("[unity]", "dirPath:", dirPath)
        var fileManager = qg.getFileSystemManager()
        fileManager.mkdir({
            dirPath: dirPath,
            success: function () {
                console.log("[unity] mkdir succes");
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: 0,
                    msg: "",
                    data: "",
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            },
            fail: function (res) {
                console.log("[unity] mkdir failure", res);
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: -1,
                    msg: res.errMsg,
                    data: "",
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            }
        })
    },

    //创建目录 的同步版本
    QGMkDirSync: function (filePath, recursive) {
        console.log("[unity]", "QGMkDirSync")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }
        filePath = UTF8ToString(filePath)
        var dirPath = CONSTANT.UNITY_FILE_PATH + filePath
        console.log("[unity]", "dirPath:", dirPath)
        recursive = (recursive === 1);
        var fileManager = qg.getFileSystemManager()
        try {
            fileManager.mkdirSync(dirPath, recursive)
        } catch (e) {
            console.error("[unity] mkdirSync failure", e);
        }
    },

    //删除目录
    QGDeleteDir: function (cmd, filePath, recursive) {
        console.log("[unity]", "QGDeleteDir")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }
        filePath = UTF8ToString(filePath)
        var dirPath = CONSTANT.UNITY_FILE_PATH + filePath
        console.log("[unity]", "dirPath:", dirPath)
        recursive = (recursive === 1);
        var fileManager = qg.getFileSystemManager()
        fileManager.rmdir({
            dirPath: dirPath,
            recursive: recursive,
            success: function () {
                console.log("[unity] rmdir succes");
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: 0,
                    msg: "",
                    data: "",
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            },
            fail: function (res) {
                console.log("[unity] rmdir failure", res);
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: -1,
                    msg: res.errMsg,
                    data: "",
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            }
        })
    },

    //删除目录 的同步版本
    QGDeleteDirSync: function (filePath, recursive) {
        console.log("[unity]", "QGDeleteDirSync")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }
        filePath = UTF8ToString(filePath)
        var dirPath = CONSTANT.UNITY_FILE_PATH + filePath
        console.log("[unity]", "dirPath:", dirPath)
        recursive = (recursive === 1);
        var fileManager = qg.getFileSystemManager()
        try {
            fileManager.rmdirSync(dirPath, recursive)
        } catch (e) {
            console.error("[unity] rmdirSync failure", e);
        }
    },

    //读取本地文件内容
    QGReadFile: function (cmd, fileName, encoding) {
        console.log("[unity]", "QGReadFile")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }
        fileName = UTF8ToString(fileName)
        var filePath = CONSTANT.UNITY_FILE_PATH + fileName
        var encodingStr = UTF8ToString(encoding);
        var successID = cmd.toString();

        var fileManager = qg.getFileSystemManager()
        fileManager.readFile({
            filePath: filePath,
            encoding: encodingStr,
            success: function (res) {
                console.log("[unity] read file success:", res.data)
                if (encodingStr === "binary") {
                    mFileData[successID] = res.data;
                }
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: 0,
                    msg: encodingStr,
                    data: res.data,
                    readLen: res.data.byteLength
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            },

            fail: function (res) {
                console.log("[unity] read file failure:", res)
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: -1,
                    msg: res.errMsg,
                    data: "",
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            }
        })
    },

    //读取本地文件内容的同步版本
    QGReadFileSync: function (fileName, encoding) {
        console.log("[unity]", "QGReadFileSync")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }
        fileName = UTF8ToString(fileName)
        var filePath = CONSTANT.UNITY_FILE_PATH + fileName
        var encodingStr = UTF8ToString(encoding);
        var callbackId = 'callbackId' + Math.random().toString();

        var fileManager = qg.getFileSystemManager()
        try {
            const text = fileManager.readFileSync(filePath, encodingStr)
            var byteLength = text.data.byteLength
            console.log("[unity] read file sync success:", text.data)
            var result;
            if (encodingStr === "utf8") {
                result = JSON.stringify({
                    callbackId: callbackId,
                    textStr: text.data,
                    encoding: encodingStr,
                    byteLength: byteLength,
                });
            } else {
                mFileData[callbackId] = text.data;
                result = JSON.stringify({
                    callbackId: callbackId,
                    encoding: encodingStr,
                    byteLength: byteLength,
                });
            }
            var bufferSize = lengthBytesUTF8(result) + 1;
            var buffer = _malloc(bufferSize);
            stringToUTF8(result, buffer, bufferSize);
            return buffer;
        } catch (e) {
            console.log("[unity] read file sync failure:", e)
            return 0;
        }
    },

    //设置文件数据到内存
    QGGetFileBuffer: function (buffer, callBackId) {
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }

        var callBackIdStr = UTF8ToString(callBackId);
        HEAPU8.set(new Uint8Array(mFileData[callBackIdStr]), buffer);
        delete mFileData[callBackIdStr];
    },

    //写文件
    QGWriteFile: function (cmd, fileName, encoding, append, textStr, textData, length) {
        console.log("[unity]", "QGWriteFile")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }
        fileName = UTF8ToString(fileName)
        var filePath = CONSTANT.UNITY_FILE_PATH + fileName
        var encodingStr = UTF8ToString(encoding);
        textStr = UTF8ToString(textStr);
        var content = (encodingStr === "utf8") ? textStr : HEAPU8.slice(textData, length + textData).buffer;

        var fileManager = qg.getFileSystemManager()
        fileManager.writeFile({
            filePath: filePath,
            data: content,
            encoding: encodingStr,
            append: append,
            success: function () {
                console.log("[unity] write file success")
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: 0,
                    msg: "",
                    data: "",
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            },
            fail: function (res) {
                console.log("[unity] write file failure:", res)
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: -1,
                    msg: res.errMsg,
                    data: "",
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            }
        })
    },

    //写文件的同步版本
    QGWriteFileSync: function (fileName, encoding, append, textStr, textData, length) {
        console.log("[unity]", "QGWriteFileSync")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }
        fileName = UTF8ToString(fileName)
        var filePath = CONSTANT.UNITY_FILE_PATH + fileName
        var encodingStr = UTF8ToString(encoding);
        textStr = UTF8ToString(textStr);
        var content = (encodingStr === "utf8") ? textStr : HEAPU8.slice(textData, length + textData).buffer;

        var fileManager = qg.getFileSystemManager()
        return fileManager.writeFileSync(filePath, content, encodingStr, append)
    },

    //获取文件 Stats 对象
    QGStat: function (cmd, path) {
        console.log("[unity]", "QGStat")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }
        path = CONSTANT.UNITY_FILE_PATH + UTF8ToString(path)
        console.log("[unity]", "path:", path)
        var fileManager = qg.getFileSystemManager()
        fileManager.stat({
            path: path,
            success: function (res) {
                console.log("[unity] stat succes:", res.stats);

                var lastAccessedTime = res.stats.lastAccessedTime;
                var date = new Date(lastAccessedTime * 1000);
                lastAccessedTime = date.getFullYear() + "-" + String(date.getMonth() + 1).padStart(2, '0') + "-" +
                    String(date.getDate()).padStart(2, '0') + " " + String(date.getHours()).padStart(2, '0') + ":" +
                    String(date.getMinutes()).padStart(2, '0') + ":" + String(date.getSeconds()).padStart(2, '0');
                var lastModifiedTime = res.stats.lastModifiedTime;
                date = new Date(lastModifiedTime * 1000);
                lastModifiedTime = date.getFullYear() + "-" + String(date.getMonth() + 1).padStart(2, '0') + "-" +
                    String(date.getDate()).padStart(2, '0') + " " + String(date.getHours()).padStart(2, '0') + ":" +
                    String(date.getMinutes()).padStart(2, '0') + ":" + String(date.getSeconds()).padStart(2, '0');

                var response = JSON.stringify({
                    isDirectory: res.stats.isDirectory(),
                    size: res.stats.size,
                    birthtime: lastAccessedTime,
                    lastModifiedTime: lastModifiedTime,
                })
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: 0,
                    msg: "",
                    data: response,
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            },
            fail: function (res) {
                console.log("[unity] stat failure", res);
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: -1,
                    msg: res.errMsg,
                    data: "",
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            }
        })
    },

    //获取文件 Stats 对象 的同步版本
    QGStatSync: function (path, recursive) {
        console.log("[unity]", "QGStatSync")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }
        path = CONSTANT.UNITY_FILE_PATH + UTF8ToString(path)
        console.log("[unity]", "path:", path)
        // recursive = (recursive === 1);

        var fileManager = qg.getFileSystemManager()
        try {
            var stats = fileManager.statSync(path, false)
            console.log("[unity] stat sync:", stats)

            var lastAccessedTime = stats.lastAccessedTime;
            var date = new Date(lastAccessedTime * 1000);
            lastAccessedTime = date.getFullYear() + "-" + String(date.getMonth() + 1).padStart(2, '0') + "-" +
                String(date.getDate()).padStart(2, '0') + " " + String(date.getHours()).padStart(2, '0') + ":" +
                String(date.getMinutes()).padStart(2, '0') + ":" + String(date.getSeconds()).padStart(2, '0');
            var lastModifiedTime = stats.lastModifiedTime;
            date = new Date(lastModifiedTime * 1000);
            lastModifiedTime = date.getFullYear() + "-" + String(date.getMonth() + 1).padStart(2, '0') + "-" +
                String(date.getDate()).padStart(2, '0') + " " + String(date.getHours()).padStart(2, '0') + ":" +
                String(date.getMinutes()).padStart(2, '0') + ":" + String(date.getSeconds()).padStart(2, '0');

            var response = JSON.stringify({
                isDirectory: stats.isDirectory(),
                size: stats.size,
                birthtime: lastAccessedTime,
                lastModifiedTime: lastModifiedTime,
            })
            var bufferSize = lengthBytesUTF8(response) + 1;
            var buffer = _malloc(bufferSize);
            stringToUTF8(response, buffer, bufferSize);
            return buffer;
        } catch (e) {
            console.log("[unity] stat sync failure:", e)
            return 0;
        }
    },

    //删除文件
    QGDeleteFile: function (cmd, fileName) {
        console.log("[unity]", "QGDeleteFile")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }
        fileName = UTF8ToString(fileName)
        var fileManager = qg.getFileSystemManager()
        fileManager.unlink({
            filePath: CONSTANT.UNITY_FILE_PATH + fileName,
            success: function () {
                console.log("[unity]", "delete file success, name=" + fileName)
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: 0,
                    msg: "",
                    data: "",
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            },

            fail: function (res) {
                console.log("[unity] delete file failure:", res)
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: -1,
                    msg: res.errMsg,
                    data: "",
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            }
        })
    },

    //删除文件 的同步版本
    QGDeleteFileSync: function (fileName) {
        console.log("[unity]", "QGDeleteFileSync")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }
        fileName = UTF8ToString(fileName)
        var filePath = CONSTANT.UNITY_FILE_PATH + fileName
        var fileManager = qg.getFileSystemManager()
        try {
            fileManager.unlinkSync(filePath)
        } catch (e) {
            console.error("[unity] unlinkSync failure", e);
        }
    },

    //保存数据 key-value
    QGSetItem: function (key, value) {
        console.log("[unity]", "QGSetItem")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }
        var sKey = UTF8ToString(key)
        var sValue = UTF8ToString(value)
        localStorage.setItem(sKey, sValue)
        console.log("[unity] set kv success:", sKey, sValue);
    },

    //获取数据
    QGGetItem: function (key) {
        console.log("[unity]", "QGGetItem")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            var buffer = _malloc(1);
            stringToUTF8("", buffer, 1);
            return buffer;
        }
        var sKey = UTF8ToString(key)
        var sValue = localStorage.getItem(sKey)
        console.log("[unity] get kv success: key-value=", sKey, sValue)
        if (sValue === null || sValue === undefined) {
            var buffer = _malloc(1);
            stringToUTF8("", buffer, 1);
            return buffer;
        }
        var bufferSize = lengthBytesUTF8(sValue) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(sValue, buffer, bufferSize);
        return buffer;
    },

    //删除数据
    QGDeleteItem: function (key) {
        console.log("[unity]", "QGDeleteItem")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }
        var sKey = UTF8ToString(key)
        localStorage.removeItem(sKey)
        console.log("[unity] delete kv success:", sKey);
    },

    //删除所有kv存储
    QGClearAllKV: function () {
        console.log("[unity]", "QGClearAllKV")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }
        localStorage.clear()
    },

    QGHttpPost: function (cmd, timeout, url, headers, body) {
        console.log("[unity]", "QGHttpPost", timeout, UTF8ToString(url), UTF8ToString(headers), UTF8ToString(body))
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }

        var headerDic = JSON.parse(UTF8ToString(headers))

        var xhr = new XMLHttpRequest();
        xhr.open("POST", UTF8ToString(url), true);

        xhr.timeout = timeout;

        setTimeout(function () {
            if (xhr.readyState === 4) {
                return
            }
            xhr.abort();
            console.log("[unity]", "http post timeout")
            var json = JSON.stringify({
                cmd: cmd,
                ret: -1,
                msg: "timeout",
                data: "",
            })
            window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
        }, timeout);

        for (var key in headerDic) {
            var item = headerDic[key];
            xhr.setRequestHeader(key, item);
        }

        xhr.onload = function () {
            console.log("[unity]", "http post success", xhr.status, xhr.statusText, xhr.responseText)
            if (xhr.readyState === 4) {
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: xhr.status,
                    msg: xhr.statusText,
                    data: xhr.responseText,
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            }
        };

        xhr.send(UTF8ToString(body));
    },

    QGHttpGet: function (cmd, timeout, url, headers, param) {
        console.log("[unity]", "QGHttpGet", timeout, UTF8ToString(url), UTF8ToString(headers), UTF8ToString(param))
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }

        var paramStr = ""
        var paramsDic = JSON.parse(UTF8ToString(param))
        for (var key in paramsDic) {
            var item = paramsDic[key];
            if (paramStr !== "") {
                paramStr = paramStr + "&" + key + "=" + item;
            } else {
                paramStr = paramStr + key + "=" + item;
            }
        }

        var urlStr = UTF8ToString(url)
        if (urlStr.endsWith("?")) {
            urlStr = urlStr + paramStr
        } else {
            urlStr = urlStr + "?" + paramStr
        }

        var xhr = new XMLHttpRequest();
        xhr.open("GET", urlStr);

        xhr.timeout = timeout;

        var headerDic = JSON.parse(UTF8ToString(headers))
        for (var key in headerDic) {
            var item = headerDic[key];
            xhr.setRequestHeader(key, item);
        }

        setTimeout(function () {
            if (xhr.readyState === 4) {
                return
            }
            xhr.abort();
            console.log("[unity]", "http get timeout")
            var json = JSON.stringify({
                cmd: cmd,
                ret: -1,
                msg: "timeout",
                data: "",
            })
            window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
        }, timeout);

        xhr.onreadystatechange = function () {
            if (xhr.readyState === 4) {
                console.log("[unity]", "http get success", xhr.status, xhr.statusText, xhr.responseText)
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: xhr.status,
                    msg: xhr.statusText,
                    data: xhr.responseText,
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            }
        };

        xhr.send();
    },

    QGExitApp: function (cmd) {
        console.log("[unity]", "QGExitApp")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }

        qg.exitApplication({
            success: function () {
                console.log("[unity]", "exit app success")
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: 0,
                    msg: "",
                    data: "",
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            },
            fail: function () {
                console.log("[unity]", "exit app failure")
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: -1,
                    msg: "",
                    data: "",
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            },
            complete: function (res) {
                console.log("[unity]", "exit app complete", res)
            }
        })
    },

    //设置剪切板内容
    QGSetClipboardData: function (cmd, clipData) {
        console.log("[unity]", "QGSetClipboardData")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }

        var data = UTF8ToString(clipData)
        qg.setClipboardData({
            data: data,
            success: function () {
                console.log("[unity]", "clip data success:", data)
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: 0,
                    msg: "",
                    data: "",
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            },
            fail: function () {
                console.log("[unity]", "clip data failure")
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: -1,
                    msg: "",
                    data: "",
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            },
            complete: function () {
                console.log("[unity]", "clip data complete")
            }
        })
    },

    //获取剪切板内容
    QGGetClipboardData: function (cmd) {
        console.log("[unity]", "QGGetClipboardData")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }

        qg.getClipboardData({
            success: function (res) {
                console.log("[unity]", "get clip success", JSON.stringify(res))
                var byteArray = new Uint8Array(res.data, 0, res.data.byteLength);
                const decoder = new TextDecoder();
                const text = decoder.decode(byteArray);
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: 0,
                    msg: "",
                    data: text,
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            },
            fail: function () {
                console.log("[unity]", "get clip data failure")
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: -1,
                    msg: "",
                    data: "",
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            },
            complete: function () {
                console.log("[unity]", "get clip data complete")
            }
        })
    },

    //获取系统信息
    QGGetSystemInfo: function (cmd) {
        console.log("[unity]", "QGGetSystemInfo")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }

        qg.getSystemInfo({
            success: function (res) {
                console.log("[unity]", "get system info success", res)
                const serializedObject = JSON.stringify(res);
                const encoded = new TextEncoder().encode(serializedObject);
                const arrayBuffer = new ArrayBuffer(encoded.length);
                const byteArray = new Uint8Array(arrayBuffer);
                byteArray.set(encoded);
                const decoder = new TextDecoder();
                const text = decoder.decode(byteArray);
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: 0,
                    msg: "",
                    data: text,
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            },
            fail: function () {
                console.log("[unity]", "get system info failure")
                var json = JSON.stringify({
                    cmd: cmd,
                    ret: -1,
                    msg: "",
                    data: "",
                })
                window.UnityInstance.SendMessage("MiBridge", "QGCallBack", json)
            },
            complete: function () {
                console.log("[unity]", "get system info complete")
            }
        })
    },

    //同步获取系统信息
    QGGetSystemInfoSync: function (cmd) {
        console.log("[unity]", "QGGetSystemInfoSync")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }

        var systemInfo = qg.getSystemInfoSync()
        if (systemInfo === "undefined" || systemInfo === null) {
            console.log("[unity]", "getSystemInfoSync failure")
            return
        } else {
            console.log("[unity]", "getSystemInfoSync success", systemInfo)
            var response = JSON.stringify(systemInfo)

            var bufferSize = lengthBytesUTF8(response) + 1;
            var buffer = _malloc(bufferSize);
            stringToUTF8(response, buffer, bufferSize);
            return buffer;
        }
    },

    //获取菜单按钮（右上角胶囊按钮）的布局位置信息
    QGGetMenuButtonBoundingClientRect: function (cmd) {
        console.log("[unity]", "QGGetMenuButtonBoundingClientRect")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }

        var clientRect = qg.getMenuButtonBoundingClientRect()
        if (clientRect === "undefined" || clientRect === null) {
            console.log("[unity]", "get menu button rect failure")
            return
        } else {
            console.log("[unity]", "get menu button rect success:", clientRect)
            var response = JSON.stringify(clientRect)

            var bufferSize = lengthBytesUTF8(response) + 1;
            var buffer = _malloc(bufferSize);
            stringToUTF8(response, buffer, bufferSize);
            return buffer;
        }
    },

    //修改渲染帧率
    QGSetPreferredFramesPerSecond: function (fps) {
        console.log("[unity]", "QGSetPreferredFramesPerSecond")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }
        console.log("[unity]", "set fps", fps)
        qg.setPreferredFramesPerSecond(fps)
    },

    QGHideLoading: function () {
        console.log("[unity]", "QGHideLoading")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }
        if (window["hideloading"] !== undefined) {
            window["hideloading"]()
        }
    },

    //主动检查更新快游戏, [onUpdateReady]:build.framework.js实现
    QGUpdateGame: function () {
        console.log("[unity]", "QGUpdateGame")
        if (typeof (qg) == 'undefined') {
            console.log("qg is undefined")
            return
        }

        var updateManager = qg.getUpdateManager()
        updateManager.onCheckForUpdate(function (res) {
            console.log("[unity]", "onCheckForUpdate", res)
            // if (res.hasUpdate) {}
        });

        updateManager.onUpdateReady(function () {
            console.log("[unity]", "onUpdateReady")
            qg.showModal({
                title: "Update Game",
                content: "The new version is ready, restart the game?",
                success: function (res) {
                    if (res.confirm) {
                        updateManager.applyUpdate()
                    }
                }
            })
        });

        updateManager.onUpdateFailed(function () {
            console.error("[unity]", "onUpdateFailed");
        });
    },
};

autoAddDeps(MiBridge, '$mAdMap');
autoAddDeps(MiBridge, '$CONSTANT');
autoAddDeps(MiBridge, '$mFileData');

mergeInto(LibraryManager.library, MiBridge);