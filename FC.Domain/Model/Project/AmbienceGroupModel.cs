namespace FC.Domain.Model.Project
{
    //public enum AmbienceGroupOrderByEnum
    //{
    //    [Description("Create At")]
    //    CreatedAt,

    //    [Description("Updated At")]
    //    UpdatedAt,

    //    [Description("Name")]
    //    Name
    //}

    //public class AmbienceGroupModel : ModelBase
    //{
    //    public ObservableCollection<AmbienceModel> Ambiences { get; set; }

    //    public ICollectionView AmbiencesCollectionView
    //    {
    //        get => CollectionViewSource.GetDefaultView(Ambiences);
    //        set => _ambiencesCollectionView = value;
    //    }

    //    public string Filter
    //    {
    //        get => _filter;
    //        set
    //        {
    //            _filter = value;

    //            AmbiencesCollectionView.Filter = w =>
    //            {
    //                using AmbienceModel model = w as AmbienceModel;

    //                return model.Name.IndexOf(value, StringComparison.OrdinalIgnoreCase) != -1;
    //            };
    //        }
    //    }

    //    public bool IsEditable
    //    {
    //        get => _isEditable;
    //        set
    //        {
    //            _isEditable = value;
    //            NotifyPropertyChanged();
    //        }
    //    }

    //    public string Name
    //    {
    //        get => _name;
    //        set
    //        {
    //            _name = value;
    //            NotifyPropertyChanged();
    //        }
    //    }

    //    public ParseObject ParseObject { get;  set; }

    //    public int SelectedIndexAmbienceOrderBy
    //    {
    //        get { return _selectedIndexAmbienceOrderBy; }
    //        set
    //        {
    //            _selectedIndexAmbienceOrderBy = value;
    //            NotifyPropertyChanged();
    //        }
    //    }

    //    public AmbienceGroupModel()
    //    {
    //        Ambiences = new ObservableCollection<AmbienceModel>();
    //    }

    //    public override string ToString()
    //    {
    //        return $"{Name} {Properties.Resources.CreateAt.ToLower()} {ParseObject?.CreatedAt:MM/dd/yyyy}";
    //    }

    //    private ICollectionView _ambiencesCollectionView;
    //    private string _filter;
    //    private bool _isEditable = true;
    //    private string _name;
    //    private int _selectedIndexAmbienceOrderBy;
    //}
}