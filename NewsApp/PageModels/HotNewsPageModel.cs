using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using NewsAPI.Constants;
using NewsAPI.Models;
using NewsApp.Helpers;
using NewsApp.Models.AppModels.News;
using NewsApp.PageModels.Base;
using NewsApp.Services.Contracts;
using Xamarin.Forms;

namespace NewsApp.PageModels
{
    public class HotNewsPageModel : BaseViewModel
    {
        private readonly INewsProviderService _newsService;
        private NewsModel selectedNewsModel;
        private ObservableCollection<NewsModel> topNewsList;

        public HotNewsPageModel(INewsProviderService newsService)
        {
            _newsService = newsService;
            topNewsList = new ObservableCollection<NewsModel>();
            ICommandBackButtonCommand = new Command(async () => await BackButtonTapped());
            ICommandNewsSelectionCommand = new Command<object>(NewsSelectionChanged);
        }

        public ObservableCollection<NewsModel> TopNewsList
        {
            get => topNewsList;
            set
            {
                if (topNewsList == value) return;
                topNewsList = value;
                RaisePropertyChanged();
            }
        }

        public NewsModel SelectedNewsModel
        {
            get => selectedNewsModel;
            set
            {
                if (selectedNewsModel == value) return;
                selectedNewsModel = value;
                RaisePropertyChanged();
            }
        }

        public ICommand ICommandBackButtonCommand { get; set; }
        public ICommand ICommandNewsSelectionCommand { get; set; }

        public override async void Init(object initData)
        {
            IsBusy = true;
            await RetriveHotUpdates();
            base.Init(initData);
            IsBusy = false;
        }

        private async Task RetriveHotUpdates()
        {
            var articles = await _newsService.GetTopNewsUpdates(new TopHeadlinesRequest
            {
                Category = Categories.Technology,
                Language = Languages.EN,
                Page = 1,
                PageSize = 10
            });

            if (articles?.Articles != null && articles.Articles.Any())
            {
                var newsList = Mapper.Map<List<Article>, List<NewsModel>>(articles.Articles);
                TopNewsList = newsList.ToObservableCollection();
            }
        }

        private async void NewsSelectionChanged(object selectedNews)
        {
            if (selectedNews == null) return;
            var selectedItem = (NewsModel)selectedNews;
            await CoreMethods.PushPageModel<DetailPageModel>(selectedItem);
            SelectedNewsModel = null;
        }

        private async Task BackButtonTapped()
        {
            await CoreMethods.PopPageModel();
        }
    }
}