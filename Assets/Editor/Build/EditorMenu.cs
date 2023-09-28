using System;
using System.IO;
using System.Linq;
using System.Web;
using UnityEditor;
using UnityEngine;

namespace Build
{
    public static class EditorMenu
    {
//         [MenuItem("DEV/Editor/Del Persistent")]
//         public static void DeleteSaves()
//         {
//             FileStorageUtils.DeletePersistentDirectory("");
//             PlayerPrefs.DeleteAll();
//
//             var url = "https://m3decor.net/v3/ya/server/load.php?id=t+i+g+r+a+++++n";
//             var bytes = System.Text.Encoding.Default.GetBytes(url);
//             var bytesString = "";
//             foreach (var b in bytes)
//             {
//                 bytesString += b;
//             }
//             Debug.LogWarning(bytesString);
//
// var encoded = System.Text.Encoding.UTF8.GetString(bytes);
// Debug.LogWarning(encoded);
//
//         }

        [MenuItem("DEV/Editor/Time 0.1")]
        public static void TS01()
        {
            Time.timeScale = 0.1f;
        }

        [MenuItem("DEV/Editor/Time 1")]
        public static void TS1()
        {
            Time.timeScale = 1f;
        }
        
        [MenuItem("DEV/Editor/Time 10")]
        public static void TS10()
        {
            Time.timeScale = 10f;
        }

       [MenuItem("DEV/Editor/Fix/Delete ALL _DevFolder")]
       public static void DeleteDevFolders()
       {
           var devEndingEnabled = "_DevFolder";

           var dirs = Directory.GetDirectories("Assets", "*", SearchOption.AllDirectories).Where(d => d.EndsWith(devEndingEnabled));

           foreach (var path in dirs)
           {
               Directory.Delete(path);
           }
           
           AssetDatabase.Refresh();
       }

       [MenuItem("DEV/Editor/Fix/Delete ALL _DevFolder invis")]
       public static void DeleteDevFildersInvosoble()
       {
           var devEndingEnabled = "_DevFolder~";

           var dirs = Directory.GetDirectories("Assets", "*", SearchOption.AllDirectories).Where(d => d.EndsWith(devEndingEnabled));

           foreach (var path in dirs)
           {
               Directory.Delete(path);
           }
           
           AssetDatabase.Refresh();
       }

   }
}

