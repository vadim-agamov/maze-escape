// using System;
// using System.Collections.Generic;
// using System.Net;
// using System.Text;
// using System.Threading.Tasks;
// using System.Web;
// using Cysharp.Threading.Tasks;
// using Helpers;
// using PP;
// using UnityEngine;
// using UnityEngine.Networking;
// using Utils;
//
// namespace SN.HostingerUtils
// {
//     public static class HostingerUtils
//     {
//         //LOAD
//         public static async UniTask<PlayerProgress> LoadPP(string serverPath, string userId)
//         {
//             var pp = await LoadPPBytes(serverPath, userId);
//             if (pp?.Length > 0)
//             {
//                 try
//                 {
//                     var ppText = Encoding.UTF8.GetString(pp);
//                     var ppObject = JsonHelper.Deserialize<PlayerProgress>(ppText);
//                     return ppObject;
//                 }
//                 catch (Exception e)
//                 {
//                     Debug.LogError(e);
//                 }
//             }
//
//             return null;
//         }
//
//         private static async UniTask<byte[]> LoadPPBytes(string serverPath, string userId)
//         {
//             var url = serverPath + "load.php?id=" + userId;
//             url = url.Replace("+", "%2B");
//
//             var webReq = UnityWebRequest.Get(url);
//             var op = await webReq.SendWebRequest();
//
//             if (!string.IsNullOrEmpty(op.error))
//             {
//                 Debug.LogError("LoadPP failed: " + op.error);
//                 return await LoadPPBytes(serverPath, userId);
//             }
//
//             var data = op.downloadHandler.data.RemoveBOM();
//             return data;
//         }
//
//         //SAVE
//         public static async UniTask<bool> SavePP(string serverPath, string userId, PlayerProgress playerProgress)
//         {
//             var ppText = JsonHelper.SerializeToString(playerProgress);
//             var ppValue = Encoding.UTF8.GetBytes(ppText);
//
//             Debug.LogWarning("SavePP: " + userId);
//
//             try
//             {
//                 var wwwForm = new WWWForm();
//                 wwwForm.AddBinaryData("val", ppValue);
//                 wwwForm.AddField("id", userId);
//
//                 var webReq = UnityWebRequest.Post(serverPath + "save.php", wwwForm);
//                 var op = await webReq.SendWebRequest();
//
//                 return string.IsNullOrEmpty(op.error);
//
//             }
//             catch (Exception e)
//             {
//                 Debug.LogError("SavePP failed: " + e.Message + " " + e.Data);
//                 return false;
//             }
//         }
//
//         public static async UniTask<byte[]> PerformGet(string uri, Dictionary<string, string> parms = null, int attempts = Int32.MaxValue)
//         {
//             var url = uri;
//             if (parms != null)
//             {
//                 var paramString = "";
//                 foreach (var kv in parms)
//                 {
//                     paramString += kv.Key + "=" + WebUtility.UrlEncode(kv.Value) + "&";
//                 }
//
//                 url = url + "?" + paramString;
//             }
//             
//             var webReq = UnityWebRequest.Get(url);
//             
//             webReq.SetRequestHeader("Access-Control-Allow-Headers","Authorization, Content-Type");
//             webReq.SetRequestHeader("Access-Control-Allow-Origin", "*");
//             webReq.SetRequestHeader("content-type", "application/json; charset=utf-8");
//
//
//                 var op = await webReq.SendWebRequest();
//
//             if (!string.IsNullOrEmpty(op.error))
//             {
//                 Debug.LogError("PerformGet: " + url + " failed: " + op.error);
//                 if (attempts > 0)
//                 {
//                     attempts--;
//                     return await PerformGet(uri, parms, attempts);
//                 }
//             }
//
//             var data = op.downloadHandler.data.RemoveBOM();
//             return data;
//         }
//
//         public static async UniTask<Texture2D> DownloadTexture(string url, int attempts = Int32.MaxValue)
//         {
//             var webReq = UnityWebRequestTexture.GetTexture(url);
//             
//             webReq.SetRequestHeader("Access-Control-Allow-Origin", "*");
//             webReq.SetRequestHeader("Access-Control-Allow-Headers", "*");
//             webReq.SetRequestHeader("content-type", "image/jpeg");
//             
//             var op = await webReq.SendWebRequest();
//             
//             if (!string.IsNullOrEmpty(op.error) || op.responseCode < 200 || op.responseCode > 299)
//             {
//                 Debug.LogError("PerformGet: " + url + " failed: " + op.error);
//                 if (attempts > 0)
//                 {
//                     attempts--;
//                     return await DownloadTexture(url, attempts);
//                 }
//             }
//             var textureAvatar = DownloadHandlerTexture.GetContent(op);
//             
//             return textureAvatar;
//         }
//     }
// }