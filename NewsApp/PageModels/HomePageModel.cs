using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using FreshMvvm;
using NewsAPI.Constants;
using NewsAPI.Models;
using NewsApp.Constants;
using NewsApp.Helpers;
using NewsApp.Models.AppModels;
using NewsApp.Models.AppModels.News;
using NewsApp.PageModels.Base;
using NewsApp.Services.Contracts;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace NewsApp.PageModels
{
    public class HomePageModel : BaseViewModel
    {
        private readonly IFilterOptionsService _filterService;
        private readonly INewsProviderService _newsService;
        private ObservableCollection<NewsModel> _bottomNewsList;
        private List<FilterModel> _filterOptions;
        private bool _isActivityBusy;
        private string _searchKey = string.Empty;
        private NewsModel _selectedBottomNewsModel;
        private FilterModel _selectedFilterOption;
        private NewsModel _selectedTopNewsModel;
        private ObservableCollection<NewsModel> _topNewsList;

        public HomePageModel(INewsProviderService newsService, IFilterOptionsService filterService)
        {
            _newsService = newsService;
            _filterService = filterService;
            _filterOptions = new List<FilterModel>();
            ICommandSeeAllTapped = new Command(async () => await SeeAllTapped());
            ICommandSearchBarTapped = new Command(async () => await SearchBarTapped());
            ICommandBottomNewsSelectionCommand = new Command<object>(BottomNewsSelectionChanged);
            ICommandTopNewsSelectionCommand = new Command<object>(TopNewsSelectionChanged);
            ICommandFilterOptionSelectionCommand = new Command<object>(FilterOptionSelectionChanged);
            ICommandLogoutCommand = new Command(async () => await OnLogoutTapped());
        }

        public bool IsActivityBusy
        {
            get => _isActivityBusy;
            set
            {
                if (_isActivityBusy == value) return;
                _isActivityBusy = value;
                RaisePropertyChanged();
            }
        }

        public List<FilterModel> FilterOptions
        {
            get => _filterOptions;
            set
            {
                if (_filterOptions == value) return;
                _filterOptions = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<NewsModel> TopNewsList
        {
            get => _topNewsList;
            set
            {
                if (_topNewsList == value) return;
                _topNewsList = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<NewsModel> BottomNewsList
        {
            get => _bottomNewsList;
            set
            {
                if (_bottomNewsList == value) return;
                _bottomNewsList = value;
                RaisePropertyChanged();
            }
        }

        public NewsModel SelectedBottomNewsModel
        {
            get => _selectedBottomNewsModel;
            set
            {
                if (_selectedBottomNewsModel == value) return;
                _selectedBottomNewsModel = value;
                RaisePropertyChanged();
            }
        }

        public NewsModel SelectedTopNewsModel
        {
            get => _selectedTopNewsModel;
            set
            {
                if (_selectedTopNewsModel == value) return;
                _selectedTopNewsModel = value;
                RaisePropertyChanged();
            }
        }

        public FilterModel SelectedFilterOption
        {
            get => _selectedFilterOption;
            set
            {
                if (_selectedFilterOption == value) return;
                _selectedFilterOption = value;
                RaisePropertyChanged();
            }
        }

        public string SearchKey
        {
            get => _searchKey;
            set
            {
                if (_searchKey == value) return;
                _searchKey = value;
                RaisePropertyChanged();
            }
        }

        public ICommand ICommandSeeAllTapped { get; set; }
        public ICommand ICommandSearchBarTapped { get; set; }
        public ICommand ICommandBottomNewsSelectionCommand { get; set; }
        public ICommand ICommandTopNewsSelectionCommand { get; set; }
        public ICommand ICommandFilterOptionSelectionCommand { get; set; }
        public ICommand ICommandLogoutCommand { get; set; }

        public override async void Init(object initData)
        {
            IsBusy = IsActivityBusy = true;
            var task1 = RetriveHotUpdates();
            var task2 = InitializeBottomList();

            await Task.WhenAll(task1, task2);

            FilterOptions = _filterService.GetFilterOptions();
            base.Init(initData);
            IsBusy = IsActivityBusy = false;
        }

        private async Task RetriveHotUpdates()
        {
            var articles = await _newsService.GetTopNewsUpdates(new TopHeadlinesRequest
            {
                Category = Categories.Business,
                Language = Languages.EN,
                Page = 1,
                PageSize = 5
            });

            if (articles?.Articles != null && articles.Articles.Any())
            {
                var newsList = Mapper.Map<List<Article>, List<NewsModel>>(articles.Articles);
                TopNewsList = newsList.ToObservableCollection();
            }
        }

        private async Task InitializeBottomList()
        {
            await RetrieveBottomList(Categories.Health);
        }

        private async Task RetrieveBottomList(Categories category)
        {
            var articles = await _newsService.GetTopNewsUpdates(new TopHeadlinesRequest
            {
                Category = category,
                Language = Languages.EN,
                Page = 1,
                PageSize = 10
            });

            if (articles?.Articles != null && articles.Articles.Any())
            {
                var newsList = Mapper.Map<List<Article>, List<NewsModel>>(articles.Articles);
                BottomNewsList = newsList.ToObservableCollection();
            }
        }

        private async Task SearchBarTapped()
        {
            if (string.IsNullOrEmpty(SearchKey)) return;

            await CoreMethods.PushPageModel<SearchListPageModel>(SearchKey);
        }

        private async Task SeeAllTapped()
        {
            await CoreMethods.PushPageModel<HotNewsPageModel>();
        }

        private async void BottomNewsSelectionChanged(object selectedNews)
        {
            if (selectedNews == null) return;
            await NavigateToDetailPage(selectedNews);
            SelectedBottomNewsModel = null;
        }

        private async void TopNewsSelectionChanged(object selectedNews)
        {
            if (selectedNews == null) return;
            await NavigateToDetailPage(selectedNews);
            SelectedTopNewsModel = null;
        }

        private async void FilterOptionSelectionChanged(object selectedFilter)
        {
            if (selectedFilter == null) return;
            var selectedItem = (FilterModel)selectedFilter;
            if (string.IsNullOrEmpty(selectedItem.FilterOption)) return;

            IsActivityBusy = true;

            //Resetting Filter Options
            foreach (var filter in FilterOptions) filter.IsSelected = string.Equals(filter.FilterOption, selectedItem.FilterOption, StringComparison.CurrentCultureIgnoreCase);

            //Refeteching New list
            BottomNewsList.Clear();
            await RetrieveBottomList(_filterService.GetFilterOptionsEnum(selectedItem.FilterOption));

            IsActivityBusy = false;
        }

        private async Task NavigateToDetailPage(object selectedNews)
        {
            var selectedItem = (NewsModel)selectedNews;
            await CoreMethods.PushPageModel<DetailPageModel>(selectedItem);
        }

        private async Task OnLogoutTapped()
        {
            var isLogout =
                await Application.Current.MainPage.DisplayAlert("Logout", "Do you want to Logout from the app", "Yes",
                    "No");
            if (isLogout)
            {
                Preferences.Set(PreferencesKey.IsLoggedIn, false);
                Preferences.Set(PreferencesKey.IsUserEmail, string.Empty);
                var page = FreshPageModelResolver.ResolvePageModel<SignInPageModel>();
                Application.Current.MainPage = new FreshNavigationContainer(page);
            }
        }
    }
}