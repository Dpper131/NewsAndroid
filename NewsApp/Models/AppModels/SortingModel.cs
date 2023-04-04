using Xamarin.CommunityToolkit.ObjectModel;

namespace NewsApp.Models.AppModels
{
	public class SortingModel : ObservableObject
	{
		private bool _isSelected;
		public string SortingOption { get; set; }

		public bool IsSelected
		{
			get => _isSelected;
			set
			{
				if (_isSelected == value) return;
				_isSelected = value;
				OnPropertyChanged();
			}
		}
	}
}