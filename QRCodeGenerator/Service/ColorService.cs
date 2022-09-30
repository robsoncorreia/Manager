using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace QRCodeGenerator.Service
{
    public interface IColorService
    {
        BaseTheme BaseTheme { get; set; }
        IList<SolidColorBrush> DefautColors { get; set; }
        string Foreground { get; set; }
        string PrimarySoftwareColor { get; set; }

        event PropertyChangedEventHandler PropertyChanged;
    }

    public class ColorService : INotifyPropertyChanged, IColorService
    {
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IList<SolidColorBrush> DefautColors { get; set; }

        public ColorService()
        {
            DefautColors = new List<SolidColorBrush> {
                (SolidColorBrush)new BrushConverter().ConvertFrom("#213e3b"),
                (SolidColorBrush)new BrushConverter().ConvertFrom("#41aea9"),
                (SolidColorBrush)new BrushConverter().ConvertFrom("#a6f6f1"),
                (SolidColorBrush)new BrushConverter().ConvertFrom("#e8ffff"),
                (SolidColorBrush)new BrushConverter().ConvertFrom("#f05454"),
                (SolidColorBrush)new BrushConverter().ConvertFrom("#af2d2d"),
                (SolidColorBrush)new BrushConverter().ConvertFrom("#ce6262"),
                (SolidColorBrush)new BrushConverter().ConvertFrom("#321f28"),
                (SolidColorBrush)new BrushConverter().ConvertFrom("#734046"),
                (SolidColorBrush)new BrushConverter().ConvertFrom("#a05344"),
                (SolidColorBrush)new BrushConverter().ConvertFrom("#e79e4f"),
                (SolidColorBrush)new BrushConverter().ConvertFrom("#39311d"),
                (SolidColorBrush)new BrushConverter().ConvertFrom("#7e7474"),
                (SolidColorBrush)new BrushConverter().ConvertFrom("#c4b6b6"),
                (SolidColorBrush)new BrushConverter().ConvertFrom("#ffdd93"),
                (SolidColorBrush)new BrushConverter().ConvertFrom("#060930"),
                (SolidColorBrush)new BrushConverter().ConvertFrom("#333456"),
                (SolidColorBrush)new BrushConverter().ConvertFrom("#595b83"),
                (SolidColorBrush)new BrushConverter().ConvertFrom("#f4abc4"),
                (SolidColorBrush)new BrushConverter().ConvertFrom("#709fb0"),
                (SolidColorBrush)new BrushConverter().ConvertFrom("#a0c1b8"),
                (SolidColorBrush)new BrushConverter().ConvertFrom("#f4ebc1")
            };
        }

        public string PrimarySoftwareColor
        {
            get => Properties.Settings.Default.primarySoftwareColor;
            set
            {
                Properties.Settings.Default.primarySoftwareColor = value;
                Properties.Settings.Default.Save();

                Foreground = PerceivedBrightness((Color)ColorConverter.ConvertFromString(value)) > 130 ? "#000" : "#FFF";
                BaseTheme = PerceivedBrightness((Color)ColorConverter.ConvertFromString(value)) > 130 ? BaseTheme.Light : BaseTheme.Dark;
                NotifyPropertyChanged();
            }
        }

        private int PerceivedBrightness(Color c)
        {
            return (int)Math.Sqrt(
            (c.R * c.R * .241) +
            (c.G * c.G * .691) +
            (c.B * c.B * .068));
        }

        public string Foreground
        {
            get => Properties.Settings.Default.foreground;
            set
            {
                Properties.Settings.Default.foreground = value;
                Properties.Settings.Default.Save();
                NotifyPropertyChanged();
            }
        }

        public BaseTheme BaseTheme
        {
            get => (BaseTheme)Properties.Settings.Default.baseTheme;
            set
            {
                Properties.Settings.Default.baseTheme = (int)value;
                Properties.Settings.Default.Save();
                NotifyPropertyChanged();
            }
        }
    }
}