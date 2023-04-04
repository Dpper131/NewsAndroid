using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using NewsApp.Models.AppModels;
using NewsApp.PageModels.Base;
using NewsApp.Services.Contracts;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace NewsApp.PageModels
{
    public class BottomSheetPopupViewModel : BaseViewModel
    {
        private readonly ISortingService _sortingService;
        private SortingModel _selectedSortingOption;
        private List<SortingModel> _sortingOptions;
        public object ReturnValue;

        public BottomSheetPopupViewModel(ISortingService sortingService)
        {
            _sortingService = sortingService;
            ICommandSortingOptionSelectionCommand = new Command<object>(FilterOptionSelectionChanged);
            ICommandSaveTappedCommand = new Command(() => SaveTappedCommand());
            ReturnValue = null;
        }

        public List<SortingModel> SortingOptions
        {
            get => _sortingOptions;
            set
            {
                if (_sortingOptions == value) return;
                _sortingOptions = value;
                RaisePropertyChanged();
            }
        }

        public SortingModel SelectedSortingOption
        {
            get => _selectedSortingOption;
            set
            {
                if (_selectedSortingOption == value) return;
                _selectedSortingOption = value;
                RaisePropertyChanged();
            }
        }

        public ICommand ICommandSortingOptionSelectionCommand { get; set; }
        public ICommand ICommandSaveTappedCommand { get; set; }

        private void SaveTappedCommand()
        {
            PopupNavigation.Instance.PopAsync();
        }

        public override Task InitializePopupAsync()
        {
            IsBusy = true;

            var sortingList = _sortingService.GetSortingOptions();

            if (sortingList != null && sortingList.Any()) SortingOptions = sortingList;

            IsBusy = false;
            return Task.FromResult(false);
        }

        private void FilterOptionSelectionChanged(object selectedSorting)
        {
            if (selectedSorting == null) return;
            var selectedItem = (SortingModel)selectedSorting;
            if (string.IsNullOrEmpty(selectedItem.SortingOption)) return;

            IsBusy = true;

            //Resetting Filter Options
            foreach (var sorting in SortingOptions) sorting.IsSelected = sorting.SortingOption.ToLower() == selectedItem.SortingOption.ToLower();

            ReturnValue = SortingOptions.FirstOrDefault(x => x.IsSelected);
            IsBusy = false;
        }
    }
}