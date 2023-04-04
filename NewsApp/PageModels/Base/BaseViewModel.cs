using System.Threading.Tasks;
using FreshMvvm;

namespace NewsApp.PageModels.Base
{
    public class BaseViewModel : FreshBasePageModel
    {
        private bool isBusy;

        public bool IsBusy
        {
            get => isBusy;
            set
            {
                if (isBusy == value) return;
                isBusy = value;
                RaisePropertyChanged();
            }
        }

        public virtual Task InitializePopupAsync()
        {
            return Task.FromResult(false);
        }
    }
}