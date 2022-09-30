using QRCodeGenerator.Model;
using QRCodeGenerator.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Markup;

namespace QRCodeGenerator.Repository
{
    public interface ILanguageRepository
    {
        List<LanguageModel> Languages { get; }

        void ChangeLanguage(int language);

        void GetLanguage();

        bool GetIs24Hours();
    }

    public class LanguageRepository : ILanguageRepository
    {
        public LanguageRepository()
        {
            Languages = new List<LanguageModel> {
                new LanguageModel{
                   Name = Properties.Resources.Portuguese,
                   LanguageEnum = LanguageEnum.PTBR,
                },
                new LanguageModel{
                   Name = Properties.Resources.English,
                   LanguageEnum = LanguageEnum.ENUS,
                },
            };

            GetLanguage();
        }

        public bool GetIs24Hours()
        {
            return (LanguageEnum)Properties.Settings.Default.language switch
            {
                LanguageEnum.PTBR => true,
                LanguageEnum.ENUS => false,
                _ => true,
            };
        }

        public void ChangeLanguage(int index)
        {
            Properties.Settings.Default.language = index;

            Properties.Settings.Default.Save();
        }

        public void GetLanguage()
        {
            Properties.Settings.Default.Reload();
            string language = ((LanguageEnum)Properties.Settings.Default.language).GetEnumDescription();
            CultureInfo culture = new CultureInfo(language);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            try
            {
                FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
            }
            catch (Exception)
            {
            }
        }

        public List<LanguageModel> Languages { get; }
    }
}