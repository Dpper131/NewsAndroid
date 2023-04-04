using AutoMapper;
using FreshMvvm;
using NewsApp.Constants;
using NewsApp.Data;
using NewsApp.Data.Contracts;
using NewsApp.Data.Mapping;
using NewsApp.Data.Repositories;
using NewsApp.Services.Contracts;
using NewsApp.Services.FilterService;
using NewsApp.Services.NewsService;
using NewsApp.Services.SortingService;
using System;
using System.IO;
using NewsApp.PageModels;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace NewsApp
{
	public partial class App
	{
		private static NewsSqliteDatabase _database;

		
		public App()
		{
			InitializeComponent();
			//InitializeCrashlytics();
			InitDiContainer();
			InitializeAutomapper();
			InitDatabase();
			Current.UserAppTheme = OSAppTheme.Light;
			InitNavigation();
		}

		/// <summary>
		/// Add here to Register Services in the Dependency Container
		/// </summary>
		private static void InitDiContainer()
		{
			FreshIOC.Container.Register<ISqliteConnectionManager, SqliteConnectionManager>();
			FreshIOC.Container.Register<IUserRepository, UserRepository>();
			FreshIOC.Container.Register<INewsProviderService, NewsProviderService>();
			FreshIOC.Container.Register<IFilterOptionsService, FilterOptionsService>();
			FreshIOC.Container.Register<ISortingService, SortingService>();
		}

		private static void InitDatabase()
		{
			if (_database == null)
			{
				var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppContants.DbName);
				Preferences.Set(PreferencesKey.DatabasePath, dbPath);
				SecureStorage.SetAsync(PreferencesKey.DbKey, AppContants.DbSecretKey).GetAwaiter().GetResult();
				_database = new NewsSqliteDatabase(dbPath);
			}
		}

		private void InitNavigation()
		{
			var isLoggedIn = Preferences.Get(PreferencesKey.IsLoggedIn, false);

			if (!isLoggedIn)
			{
				var page = FreshPageModelResolver.ResolvePageModel<SignInPageModel>();
				MainPage = new FreshNavigationContainer(page);
			}
			else
			{
				var page = FreshPageModelResolver.ResolvePageModel<HomePageModel>();
				MainPage = new FreshNavigationContainer(page);
			}
		}

		private void InitializeAutomapper()
		{
			Mapper.Initialize(c => c.AddProfile<MappingProfile>());
			Mapper.AssertConfigurationIsValid();
		}

		private static void InitializeCrashlytics()
		{
			var userEmail = Preferences.Get(PreferencesKey.IsUserEmail, string.Empty);
		}

		protected override void OnStart()
		{
		}

		protected override void OnSleep()
		{
		}

		protected override void OnResume()
		{
		}
	}
}