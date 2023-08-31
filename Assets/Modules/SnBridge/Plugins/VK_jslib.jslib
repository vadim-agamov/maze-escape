mergeInto(LibraryManager.library, {
	Init: function( )
	{
	     VK.init(
                 function() 
                 {
                    console.log("js success");
                    const queryString = window.location.search;
                    console.log("js " + queryString);
                    const urlParams = new URLSearchParams(queryString);
                    var userId = urlParams.get('viewer_id');
                    console.log("js " + "viewer_id : " + userId );
                         
                    unityInstance.SendMessage('VKBridge', 'OnVkInited', userId );
                  },
                   function()
                    {
                     console.log("js " + "fail");
                     unityInstance.SendMessage('VKBridge','onFail' );
                    }, 
                     '5.130');
                 
                 VK.addCallback('onOrderSuccess',  
                                 function f(orderId) 
                                 {
                                    console.log("js onOrderSuccess orderId: " + orderId);
                                    unityInstance.SendMessage('VKBridge','onOrderSuccessFromJs', orderId.toString());
                                 }
                                 );
                                 
                  VK.addCallback('onOrderCancel',  
                                                  function f() 
                                                  {
                                                     console.log("js " + "onOrderCancel");
                                                     unityInstance.SendMessage('VKBridge','onOrderCancelFromJs' );
                                                  });
                                          
                   VK.addCallback('onOrderFail',  
                                                function f(errorCode ) 
                                                {
                                                   console.log("js " + "onOrderFail");
                                                   unityInstance.SendMessage('VKBridge','onOrderFailFromJs', errorCode.toString()  );
                                                });
                 
	},
	
    ShowOrderBox: function(itemId)
     {
        var params = {
            type: 'item',
            item: Pointer_stringify(itemId)
        };
        VK.callMethod('showOrderBox', params);
    },
	 ShowSettingsBoxQuickLabel: function()
     {
       VK.callMethod("showSettingsBox", 256);
    },
	
	ShowInviteFriends: function()
         {
              VK.callMethod("showInviteBox");
        },
	             

	 ShowRewardedAdsToJs: function()
     {
           ShowRewardedAdHtml();
           console.log("js ShowRewardedAdsJs");
     }	,
    
    ShowInterAdsToJs: function()
    {
       ShowInterAdHtml();
       console.log("js ShowInterAdsToJs");
    }	
    
});

