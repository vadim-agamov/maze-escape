mergeInto(LibraryManager.library, {

    YandexGetUserId: function () {
        var returnStr = "";
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        return buffer;
    },
    
    // initialize
    YandexStartGame: function () {
        console.log("YandexStartGame");

        ysdk.getPlayer().then(_player => {
            player = _player;
            console.log("YandexStartGame: player mode: " + player.getMode());
            unityInstance.SendMessage('Yandex', 'YandexOnGameStarted');
        }).catch(err => {
            unityInstance.SendMessage('Yandex', 'YandexOnGameNotStarted', error.message);
        });
    },

    // initialize
    YandexGameReady: function () {
        console.log("YandexStartGame");
        ysdk.features.LoadingAPI.ready();
    },

    // interstitial
    YandexShowInterstitial: function () {
        ysdk.adv.showFullscreenAdv({
            callbacks: {
                onClose: function(wasShown) {
                    console.log("YandexShowInterstitial shown: " + wasShown);
                    unityInstance.SendMessage('Yandex', 'YandexOnInterstitialShown');
                },
                onError: function(error) {
                    console.log("YandexShowInterstitial not shown: " + error);
                    unityInstance.SendMessage('Yandex', 'YandexOnInterstitialNotShown', error);
                }
            }
        })
    },

    // rewarded video
    YandexShowRewardedVideo: function () {

        ysdk.adv.showRewardedVideo({
            callbacks: {
                onOpen: () => {
                    console.log('Video ad open.');
                },
                onRewarded: () => {
                    console.log('Rewarded!');
                },
                onClose: () => {
                    console.log('Video ad closed.');
                    unityInstance.SendMessage('Yandex', 'YandexOnRewardedVideoShown');
                },
                onError: (e) => {
                    console.log('Error while open video ad:', e);
                    unityInstance.SendMessage('Yandex', 'YandexOnRewardedVideoNotShown', e);
                }
            }
        });
    },

    // Profile
    YandexSetData: function (dataStr) {
        var dataRaw = UTF8ToString(dataStr);
        var dataObject = JSON.parse(dataRaw);
        var result = {data: dataObject};
        console.log('SetData begin: ' + result);
        player
            .setData(result)
            .then(function () {
                console.log('SetData done');
            });
        localStorage.setItem('data', result);
    },

    YandexGetData: function () {
        player
            .getData(['data'])
            .then(function (response) {
                console.log('GetData: respose: ' + response);
                var data = {};
                if(response['data'] == null) {
                    data = JSON.stringify(localStorage.getItem('data') || {})
                    console.log('GetData: loaded from local storage: ' + data)
                }
                else {
                    data = JSON.stringify(response["data"] || {});
                    console.log('GetData: loaded from player: ' + data)
                }
                
                unityInstance.SendMessage('Yandex', 'YandexOnPlayerProgressLoaded', data);
            })
            .catch(function (error) {
                console.log("GetData not loaded: " + error.message);
                unityInstance.SendMessage('Yandex', 'YandexOnPlayerProgressLoaded', "{}");
            });
    },

    YandexGetLanguage: function () {
        var lang = ysdk.environment.i18n.lang || "en";
        console.log("GetLanguage: " + lang);
        var bufferSize = lengthBytesUTF8(lang) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(lang, buffer, bufferSize);
        return buffer;
    },
    
    // Event
    YandexLogEvent: function (eventName) {
    }
});