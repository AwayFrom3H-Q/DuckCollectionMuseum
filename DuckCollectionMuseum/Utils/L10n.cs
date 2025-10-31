using SodaCraft.Localizations;
using UnityEngine;

namespace DuckCollectionMuseum.Utils
{
    public static class L10n
    {
        public static string GetLabel(string buttonType)
        {
            SystemLanguage language = LocalizationManager.CurrentLanguage;
            switch (language)
            {
                case SystemLanguage.ChineseSimplified:
                    return buttonType switch
                    {
                        "文本"   => "文本",
                        _ => buttonType
                    };

                case SystemLanguage.ChineseTraditional:
                    return buttonType switch
                    {
                        "文本" => "文本",

                        _ => buttonType
                    };

                case SystemLanguage.Japanese:
                    return buttonType switch
                    {
                        "文本" => "文本",

                        _ => buttonType
                    };

                case SystemLanguage.German:
                    return buttonType switch
                    {
                        "文本" => "文本",

                        _ => buttonType
                    };

                case SystemLanguage.Russian:
                    return buttonType switch
                    {
                        "文本" => "文本",
                        _ => buttonType
                    };

                case SystemLanguage.Spanish:
                    return buttonType switch
                    {
                        "文本" => "文本",
                        _ => buttonType
                    };

                case SystemLanguage.Korean:
                    return buttonType switch
                    {
                        "文本" => "文本",
                        _ => buttonType
                    };

                case SystemLanguage.French:
                    return buttonType switch
                    {
                        "文本" => "文本",
                        _ => buttonType
                    };

                case SystemLanguage.Portuguese:
                    return buttonType switch
                    {
                        "文本" => "文本",
                        _ => buttonType
                    };

                case SystemLanguage.English:
                default:
                    return buttonType switch
                    {
                        "文本" => "文本",
                        _ => buttonType
                    };
            }
        }
    }
}
