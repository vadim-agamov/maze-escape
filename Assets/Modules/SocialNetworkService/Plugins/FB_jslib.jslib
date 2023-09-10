mergeInto(LibraryManager.library, {

    FbGetUserId: function () {
        var returnStr = FBInstant.player.getID() || "";
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        return buffer;
    },

    FbGetLang: function () {
        var returnStr = FBInstant.getLocale();
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        return buffer;
    },

    FBStartGame: function () {
        console.log("FBStartGame");
        FBInstant.startGameAsync()
            .then(function () {
                console.log("FBStartGame: OnGameStarted");
                unityInstance.SendMessage('FbBridge', 'OnGameStarted');
            })
            .catch(function (error) {
                console.log("FBStartGame: OnGameNotStarted: " + error.message);
                unityInstance.SendMessage('FbBridge', 'OnGameNotStarted', error.message);
            });
    },

    // Interstitial
    FbPreloadInterstitial: function (id) {
        var placementId = UTF8ToString(id);
        console.log("getInterstitialAdAsync: begin " + placementId);
        FBInstant.getInterstitialAdAsync(placementId)
            .then(function (ad) {
                preloadedInterstitial = ad;
                console.log("getInterstitialAdAsync: end");
                return preloadedInterstitial.loadAsync();
            })
            .then(function () {
                console.log("preloadedInterstitial loaded");
                unityInstance.SendMessage('FbBridge', 'OnInterstitialLoaded');
            })
            .catch(function (error) {
                console.log("preloadedInterstitial not loaded " + error.message);
                unityInstance.SendMessage('FbBridge', 'OnInterstitialNotLoaded', error.message);
            });
    },

    FbShowInterstitial: function () {
        if (preloadedInterstitial == null) {
            console.log("FbShowInterstitial not shown: no preloaded ad");
            unityInstance.SendMessage('FbBridge', 'OnInterstitialNotShown', 'no preloaded ad');
            return;
        }

        console.log("preloadedInterstitial.showAsync begin");
        preloadedInterstitial.showAsync()
            .then(function () {
                console.log("preloadedInterstitial.showAsync end");
                preloadedInterstitial = null;
                unityInstance.SendMessage('FbBridge', 'OnInterstitialShown');
            })
            .catch(function (error) {
                preloadedInterstitial = null;
                console.log("preloadedInterstitial.showAsync not shown: " + error.message);
                unityInstance.SendMessage('FbBridge', 'OnInterstitialNotShown', error.message);
            });
    },

    // RewardedInterstitial
    FbPreloadRewardedInterstitial: function (id) {
        var placementId = UTF8ToString(id);
        console.log("getRewardedInterstitialAsync begin " + placementId);
        FBInstant.getRewardedInterstitialAsync(placementId)
            .then(function (ad) {
                console.log("getRewardedInterstitialAsync end " + placementId);
                preloadedRewardedInterstitial = ad;
                return preloadedRewardedInterstitial.loadAsync();
            })
            .then(function () {
                console.log("preloadedRewardedInterstitial.loadAsync loaded");
                unityInstance.SendMessage('FbBridge', 'OnRewardedInterstitialLoaded');
            })
            .catch(function (error) {
                console.log("preloadedRewardedInterstitial.loadAsync not loaded " + error.message);
                unityInstance.SendMessage('FbBridge', 'OnRewardedInterstitialNotLoaded');
            });
    },

    FbShowRewardedInterstitial: function () {
        if (preloadedRewardedInterstitial == null) {
            console.log("FbShowRewardedInterstitial not shown: no preloaded ad");
            unityInstance.SendMessage('FbBridge', 'OnRewardedInterstitialNotShown', 'no preloaded ad');
            return;
        }

        console.log("preloadedRewardedInterstitial.showAsync begin");
        preloadedRewardedInterstitial.showAsync()
            .then(function () {
                preloadedRewardedInterstitial = null;
                console.log("preloadedRewardedInterstitial.showAsync end");
                unityInstance.SendMessage('FbBridge', 'OnRewardedInterstitialShown');
            })
            .catch(function (error) {
                preloadedRewardedInterstitial = null;
                console.log("preloadedRewardedInterstitial.showAsync not shown: " + error.message);
                unityInstance.SendMessage('FbBridge', 'OnRewardedInterstitialNotShown', error.message);
            });
    },

    // RewardedVideo
    FbPreloadRewardedVideo: function (id) {
        var placementId = UTF8ToString(id);
        console.log("getRewardedVideoAsync: begin " + placementId);
        FBInstant.getRewardedVideoAsync(placementId)
            .then(function (ad) {
                preloadedRewardedVideo = ad;
                console.log("getRewardedVideoAsync done");
                return preloadedRewardedVideo.loadAsync();
            })
            .then(function () {
                console.log("getRewardedVideoAsync loaded");
                unityInstance.SendMessage('FbBridge', 'OnRewardedVideoLoaded');
            })
            .catch(function (error) {
                console.log("getRewardedVideoAsync not loaded: " + error.message);
                unityInstance.SendMessage('FbBridge', 'OnRewardedVideoNotLoaded', error.message);
            });
    },

    FbShowRewardedVideo: function () {
        if (preloadedRewardedVideo == null) {
            console.log("FbShowRewardedVideo not shown: no preloaded ad");
            unityInstance.SendMessage('FbBridge', 'OnRewardedVideoNotShown', 'no preloaded ad');
            return;
        }

        console.log("preloadedRewardedVideo.showAsync begin");
        preloadedRewardedVideo.showAsync()
            .then(function () {
                console.log("preloadedRewardedVideo.showAsync done");
                preloadedRewardedVideo = null;
                unityInstance.SendMessage('FbBridge', 'OnRewardedVideoShown');
            })
            .catch(function (error) {
                preloadedInterstitial = null;
                console.log("preloadedRewardedVideo.showAsync not shown: " + error.message);
                unityInstance.SendMessage('FbBridge', 'OnRewardedVideoNotShown', error.message);
            });
    },

    FBSetData: function (dataStr) {
        var dataRaw = UTF8ToString(dataStr);
        var dataObject = {data: dataRaw};
        console.log('FBSetData begin: ' + dataRaw);
        FBInstant.player
            .setDataAsync(dataObject)
            .then(FBInstant.player.flushDataAsync)
            .then(function () {
                console.log('FBSetData done: ' + dataRaw);
            });
    },

    FBGetData: function () {
        console.log('FBGetData: begin');

        FBInstant.player
            .getDataAsync(["data"])
            .then(function (response) {
                var data = JSON.stringify(response["data"]);
                console.log('FBGetData: loaded, : ' + data);
                unityInstance.SendMessage('FbBridge', 'OnPlayerProgressLoaded', data);
            })
            .catch(function (error) {
                console.log("FBGetData not loaded: " + error.message);
                unityInstance.SendMessage('FbBridge', 'OnPlayerProgressLoaded', "{}");
            });
    },

    FBLogEvent: function (eventName) {
        var jsEventName = UTF8ToString(eventName);
        console.log('FBLogEvent begin: ' + jsEventName);
        var logged = FBInstant.logEvent(jsEventName);
        console.log('FBLogEvent end: ' + logged);
    }
});