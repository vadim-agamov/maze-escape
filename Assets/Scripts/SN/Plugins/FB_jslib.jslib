mergeInto(LibraryManager.library, {

  FbGetUserId: function () 
  {
    var returnStr = FBInstant.player.getID() || "";
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
  },
  
  FbGetLang: function () 
  {
    var returnStr = FBInstant.getLocale();
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
  },
  
  FbPreloadInterstitial: function()
  {
      var placementId = "476711777795456_555460746587225"; //Pointer_stringify(rewardedPlacementId);
      console.log("FbPreloadInterstitial: " + placementId);
      FBInstant.getInterstitialAdAsync(placementId)
        .then(function(interstitial)
        {
            preloadedInterstitial = interstitial;
            console.log("FbPreloadInterstitial: getInterstitialAdAsync");
            return preloadedInterstitial.loadAsync();
        })
        .then(function() 
        {
            console.log("FbPreloadInterstitial OnInterstitialLoaded");
            unityInstance.SendMessage('FbBridge', 'OnInterstitialLoaded');
        })
        .catch(function(error) 
        {
            console.log("FbPreloadInterstitial OnInterstitialNotLoaded: " + error.message);
            unityInstance.SendMessage('FbBridge', 'OnInterstitialNotLoaded');
        });
  },
    
  FbShowInterstitial: function()
  {
     if(preloadedInterstitial == null)
        return;
  
     preloadedInterstitial.showAsync()
        .then(function() 
        {
            preloadedInterstitial = null;
            console.log("FbShowInterstitial OnInterstitialShown");
            unityInstance.SendMessage('FbBridge', 'OnInterstitialShown');
        })
        .catch(function(error) 
        {
            preloadedInterstitial = null;
            console.log("FbShowInterstitial : " + error.message)
        });
  },
  
  FbPreloadRewarded: function()
  {
    var placementId = "476711777795456_555445043255462"; //Pointer_stringify(rewardedPlacementId);
    
    console.log("FbPreloadRewarded: " + placementId);
    FBInstant.getRewardedVideoAsync(placementId)
      .then(function(rewarded) 
      {
        console.log("FbPreloadRewarded: rewarded.loadAsync()");
        preloadedRewardedVideo = rewarded;
        return preloadedRewardedVideo.loadAsync();
      })
      .then(function() 
      {
        console.log("FbPreloadRewarded OnRewardedLoaded");
        unityInstance.SendMessage('FbBridge', 'OnRewardedLoaded');
      })
      .catch(function(error) 
      {
        console.log("FbPreloadRewarded OnRewardedNotLoaded: " + error.message);
        unityInstance.SendMessage('FbBridge', 'OnRewardedNotLoaded');
      });
  },
  
   FbShowRewarded: function()
   {
     if(preloadedRewardedVideo == null)
     {
        unityInstance.SendMessage('FbBridge', 'OnRewardedNotShown', 'no preloaded ad');
        return;
     }
   
     preloadedRewardedVideo.showAsync()
        .then(function() 
        {
            preloadedRewardedVideo = null;
            console.log("FbShowRewarded OnRewardedShown");
            unityInstance.SendMessage('FbBridge', 'OnRewardedShown');
        })
        .catch(function(error) 
        {
            preloadedRewardedVideo = null;
            console.log("FbShowRewarded OnRewardedNotShown: " + error.message);
            unityInstance.SendMessage('FbBridge', 'OnRewardedNotShown', error.message);
        });
   },
      
  FBSetData: function(dataStr)
  {
    var dataRaw = UTF8ToString(dataStr);
    var dataObject = { data: dataRaw };
    console.log('FBSetData begin: ' + dataRaw);
    FBInstant.player
      .setDataAsync(dataObject)
      .then(FBInstant.player.flushDataAsync)
      .then(function() 
      {
        console.log('FBSetData done: ' + dataRaw);
      });
  },

  FBGetData: function()
  {  
    console.log('FBGetData: begin');
    
    FBInstant.player
      .getDataAsync(["data"])
      .then(function(response) 
      {
         //console.log('FBGetData: end, : ' + response);
         //console.log(JSON.stringify(response))
         //var responseValidated = JSON.parse(JSON.stringify(response));
         var data = JSON.stringify(response["data"]);
         console.log('FBGetData: loaded, : ' + data);
         unityInstance.SendMessage('FbBridge', 'OnPlayerProgressLoaded', data);
      })
      .catch(function(error) 
      {
         console.log("FBGetData not loaded: " + error.message);
         unityInstance.SendMessage('FbBridge', 'OnPlayerProgressLoaded', "{}");
      });
  }
});