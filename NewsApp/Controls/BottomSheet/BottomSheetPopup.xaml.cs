using FreshMvvm;
using System.Threading.Tasks;
using NewsApp.PageModels;
using NewsApp.PageModels.Base;
using Xamarin.Forms.Xaml;

namespace NewsApp.Controls.BottomSheet
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BottomSheetPopup : Rg.Plugins.Popup.Pages.PopupPage
	{
		private TaskCompletionSource<object> _taskCompletionSource;
		public Task<object> PopupClosedTask => _taskCompletionSource.Task;

		public BottomSheetPopup()
		{
			InitializeComponent();

			var viewModel = FreshIOC.Container.Resolve<BottomSheetPopupViewModel>();

			BindingContext = viewModel;
			new Task(async () =>
			{
				await ((BaseViewModel)BindingContext).InitializePopupAsync();
			}).Start();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			_taskCompletionSource = new TaskCompletionSource<object>();
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			_taskCompletionSource.SetResult(((BottomSheetPopupViewModel)BindingContext).ReturnValue);
		}

		protected override bool OnBackButtonPressed()
		{
			return base.OnBackButtonPressed();
		}

		protected override bool OnBackgroundClicked()
		{
			return base.OnBackgroundClicked();
		}
	}
}