using System.Threading.Tasks;
using System.Windows.Input;
using NewsApp.Models.AppModels.News;
using NewsApp.PageModels.Base;
using Xamarin.Forms;

namespace NewsApp.PageModels
{
    public class DetailPageModel : BaseViewModel
    {
        private NewsModel newsModel;

        public DetailPageModel()
        {
            ICommandBackButtonCommand = new Command(async () => await BackButtonTapped());
        }

        public NewsModel NewsModel
        {
            get => newsModel;
            set
            {
                if (newsModel == value) return;
                newsModel = value;
                RaisePropertyChanged();
            }
        }

        public ICommand ICommandBackButtonCommand { get; set; }

        public override void Init(object initData)
        {
            if (initData != null) NewsModel = (NewsModel)initData;

            base.Init(initData);
        }

        private async Task BackButtonTapped()
        {
            await CoreMethods.PopPageModel();
        }
    }
}