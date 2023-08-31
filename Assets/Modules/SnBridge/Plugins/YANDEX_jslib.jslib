mergeInto(LibraryManager.library, {
  InitPurchases: function() {
    initPayments();
  },

  InitializeYSDK: function() {
   initialize();
  },
  
  Purchase: function(id) {
    buy(Pointer_stringify(id) );
  },

  AuthenticateUser: function() {
    auth();
  },

  GetUserData: function() {
    getUserData();
  },

  ShowFullscreenAd: function () {
    showFullscrenAd();
  },

  ShowRewardedAd: function(placement) {
    showRewardedAd(Pointer_stringify(placement) );
  },

  OpenWindow: function(link){
    var url = Pointer_stringify(link);
      document.onmouseup = function()
      {
        window.open(url);
        document.onmouseup = null;
      }
  },
  
  ShowRateUsPopup: function()
  {
     showRateUs();
  }
    
  
});