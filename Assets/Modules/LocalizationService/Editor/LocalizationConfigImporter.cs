using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Modules.PlatformService;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Modules.LocalizationService.Editor
{
    [ScriptedImporter(1, "locale")]
    public class LocalizationConfigImporter: ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var lines = File.ReadAllLines(ctx.assetPath);

            var locales = lines[0].Split(';').Skip(1).ToArray();
            var data = lines.Skip(1).ToArray();
            var providerConfig = ScriptableObject.CreateInstance<LocalizationProviderConfig>();
            ctx.AddObjectToAsset("provider", providerConfig);
            ctx.SetMainObject(providerConfig);
            
            for (var i = 0; i < locales.Length; i++)
            {
                var locale = locales[i];
                var config = ProcessLocale(locale, i, data);
                ctx.AddObjectToAsset(locale, config);
                providerConfig.Add(config);
            }
        }

        private LocalizationConfig ProcessLocale(string locale, int column, IEnumerable<string> data)
        {
            var keys = new List<LocalizationKey>();
            foreach (var t in data)
            {
                var line = t.Replace("; ", ";").Split(';');
                var item = new LocalizationKey(line[0], line[column + 1]);
                keys.Add(item);
            }

            var config = ScriptableObject.CreateInstance<LocalizationConfig>();
            config.Language = (Language)Enum.Parse(typeof(Language), locale);
            config.name = config.Language.ToString();
            config.Keys = keys.ToArray();
            return config;
        }
    }
}