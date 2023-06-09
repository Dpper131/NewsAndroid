﻿using NewsApp.Models.AppModels;

namespace NewsApp.Constants
{
	public static class AppContants
	{
		public const string DbName = "newsapp.db3";
		public const string DbSecretKey = "NewsAppR1x";
		public const string NewsApiKey = "5305bcf42f6c4264ba96a8be733bc9b7";

		public const SQLite.SQLiteOpenFlags SqlQueryFlags =
					SQLite.SQLiteOpenFlags.ReadWrite |
					SQLite.SQLiteOpenFlags.SharedCache;

		public const SQLite.SQLiteOpenFlags DbCreateFlags =
							SQLite.SQLiteOpenFlags.ReadWrite |
							SQLite.SQLiteOpenFlags.Create |
							SQLite.SQLiteOpenFlags.SharedCache;

		public static SortingModel SelectedSortingOption { get; set; }
	}
}