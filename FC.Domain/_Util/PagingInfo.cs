using FC.Domain._Base;
using System;
using System.ComponentModel;

namespace FC.Domain._Util
{
    public enum ItemsPerPageEnum
    {
        [Description("10")]
        PerPage10 = 10,

        [Description("20")]
        PerPage20 = 20,

        [Description("30")]
        PerPage30 = 30,

        [Description("40")]
        PerPage40 = 40,

        [Description("50")]
        PerPage50 = 50,

        [Description("100")]
        PerPage100 = 100,

        [Description("1000")]
        PerPage1000 = 1000
    }

    public class PagingInfo : ModelBase
    {
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (Equals(value, _currentPage))
                {
                    return;
                }

                IsLastPage = value >= _totalPages;

                _currentPage = value;

                NotifyPropertyChanged();
            }
        }

        public bool IsLastPage
        {
            get => _isLastPage;
            set
            {
                _isLastPage = value;
                NotifyPropertyChanged();
            }
        }

        public int ItemsPerPage
        {
            get => _itemsPerPage;
            set
            {
                if (Equals(_itemsPerPage, value))
                {
                    return;
                }

                _itemsPerPage = value;

                TotalPages = (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);

                NotifyPropertyChanged();
            }
        }

        public int TotalItems
        {
            get => _totalItems;
            set
            {
                if (Equals(_totalItems, value))
                {
                    return;
                }
                _totalItems = value;
                TotalPages = (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);
                NotifyPropertyChanged();
            }
        }

        public int TotalPages
        {
            get => _totalPages;
            set
            {
                _totalPages = value;
                IsLastPage = _currentPage >= value;
                NotifyPropertyChanged();
            }
        }

        private int _currentPage = 1;
        private bool _isLastPage;
        private int _itemsPerPage = (int)ItemsPerPageEnum.PerPage10;
        private int _totalItems;
        private int _totalPages = 1;
    }
}