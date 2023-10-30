using Modules.PlatformService;
using Modules.ServiceLocator;


/*
 * tr 0131, 00e7, 015, 00f6, 00fc, 011f, 0130, 00c7, 015e, 00d6, 00dc, 011e
 * cyr 0400–04FF
 * latin 0000–007F
 *
 *
 * 00,20-7E,401,410-44F,451,0131,00e7,015,00f6,00fc,011f,0130,00c7,015e,00d6,00dc,011e
 */

namespace Modules.LocalizationService
{
    public interface ILocalizationService: IService
    {
        void SetLanguage(Language language);
        void Register(LocalizationProviderConfig providerConfig);
        void Unregister(LocalizationProviderConfig providerConfig);
        string Localize(string key, params object[] args);
        Language CurrentLanguage { get; }
    }
}