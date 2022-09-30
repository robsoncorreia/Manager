using FC.Domain._Base;
using FC.Domain.Model.Device;
using FC.Domain.Model.User;
using Parse;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace FC.Domain.Model.Project
{
    public enum AmbienceOrderByEnum
    {
        CreatedAt,
        UpdatedAt,
        Name
    }

    public class AmbienceModel : ModelBase
    {
        public ParseUser ArchivedBy { get; set; }
        public IList<string> Blacklist { get; set; }
        public ObservableCollection<ParseUserCustom> BlacklistUsers { get; set; }
        public byte[] BytesImage { get; set; }
        public ParseUser DeletedBy { get; set; }
        public ObservableCollection<GatewayModel> Devices { get; set; }
        public ObservableCollection<ZwaveDevice> ZwaveDevices { get; set; }
        public IList<string> FiledFor { get; set; }
        public ParseFile ImageParseFile { get; set; }

        public ImageSource ImageSource
        {
            get => _imageSource;
            set
            {
                _imageSource = value;
                NotifyPropertyChanged();
            }
        }

        public string ImageUrl { get; set; }

        public bool IsCurrentUserOnBlacklist
        {
            get => _isCurrentUserOnBlacklist;
            set
            {
                _isCurrentUserOnBlacklist = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsDeleted
        {
            get => _isDeleted;
            set
            {
                _isDeleted = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsEnableBlackList
        {
            get => _isEnableBlackList;
            set
            {
                _isEnableBlackList = value;
                NotifyPropertyChanged();
            }
        }

        #region Colors

        public double Blue
        {
            get => _blue;
            set
            {
                _blue = value;
                NotifyPropertyChanged();
                ConvertColors();
            }
        }

        public double Green
        {
            get => _green;
            set
            {
                _green = value;
                NotifyPropertyChanged();
                ConvertColors();
            }
        }

        public double Red
        {
            get => _red;
            set
            {
                _red = value;
                NotifyPropertyChanged();
                ConvertColors();
            }
        }

        public void ConvertColors(string primaryColor)
        {
            try
            {
                if (string.IsNullOrEmpty(primaryColor))
                {
                    throw new ArgumentException(Properties.Resources.Primary_Color_Null, nameof(primaryColor));
                }

                if (primaryColor.Length != 7)
                {
                    return;
                }

                if (double.TryParse(Convert.ToInt32(primaryColor.Substring(1, 2), 16).ToString(), out double red))
                {
                    Red = red;
                }
                if (double.TryParse(Convert.ToInt32(primaryColor.Substring(3, 2), 16).ToString(), out double green))
                {
                    Green = green;
                }
                if (double.TryParse(Convert.ToInt32(primaryColor.Substring(5, 2), 16).ToString(), out double blue))
                {
                    Blue = blue;
                }
            }
            catch (Exception)
            {
            }
        }

        private void ConvertColors()
        {
            PrimaryColor = $"#{(int)Red:X2}{(int)Green:X2}{(int)Blue:X2}";
        }

        #endregion Colors

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NotifyPropertyChanged();
            }
        }

        public ParseObject ParseObject { get; set; }

        public string PrimaryColor
        {
            get => _primaryColor;
            set
            {
                _primaryColor = value;
                NotifyPropertyChanged();
            }
        }

        public int SelectedIndexDeviceModel
        {
            get => _selectedIndexDeviceModel;
            set
            {
                _selectedIndexDeviceModel = value;
                NotifyPropertyChanged();
            }
        }

        public ParseObject ParseObjectGatewayToRemove { get; set; }
        public ParseObject ParseObjectZwaveDeviceToRemove { get; set; }

        public AmbienceModel()
        {
            Devices = new ObservableCollection<GatewayModel>();
            BlacklistUsers = new ObservableCollection<ParseUserCustom>();
            Blacklist = new List<string>();
            FiledFor = new List<string>();
            ZwaveDevices = new ObservableCollection<ZwaveDevice>();
        }

        public override string ToString()
        {
            return $"{Name} {Properties.Resources.CreateAt.ToLower()} {ParseObject?.CreatedAt:MM/dd/yyyy}";
        }

        private double _blue = 60;
        private double _green = 60;
        private ImageSource _imageSource;
        private bool _isCurrentUserOnBlacklist;
        private bool _isDeleted;
        private bool _isEnableBlackList;
        private string _name;
        private string _primaryColor = "#606060";
        private double _red = 60;
        private int _selectedIndexDeviceModel;
    }
}