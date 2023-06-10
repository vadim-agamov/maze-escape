using System;

namespace SN.FbBridge
{
    [Serializable]
    public class FbProduct
    {
        public string id;
        public string title;
        public string description;
        public string imageURI;
        public string price;
        public string priceValue;
        public string priceCurrencyCode;
    }
}