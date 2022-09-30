using FC.Domain._Base;
using Parse;
using System.ComponentModel;
using System.Windows.Media;

namespace FC.Domain.Model.Project
{
    public class AmbienceImageModel : ModelBase, IDataErrorInfo
    {
        public byte[] BytesImage { get; set; }

        public string Error => null;

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                NotifyPropertyChanged();
            }
        }

        public ImageSource ImageSource
        {
            get => _imageSource;
            set
            {
                _imageSource = value;
                NotifyPropertyChanged();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NotifyPropertyChanged();
            }
        }

        public ParseFile ParseFile { get; set; }
        public ParseObject ParseObject { get; set; }
        public string Url { get; set; }
        private int _id;

        private ImageSource _imageSource;

        private string _name;

        public string this[string columnName]
        {
            get
            {
                if (columnName is null)
                {
                    return string.Empty;
                }

                string result = string.Empty;

                if (columnName.Equals(nameof(ImageSource)))
                {
                    if (ImageSource is null)
                    {
                        result = Properties.Resources.Choose_an_image;
                    }
                }

                return result;
            }
        }
    }
}