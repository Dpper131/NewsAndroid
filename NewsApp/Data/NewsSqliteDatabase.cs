using System;
using NewsApp.Constants;
using SQLite;
using Xamarin.Essentials;
using DO = NewsApp.Models.DataModels;

namespace NewsApp.Data
{
	public class NewsSqliteDatabase
	{
		public NewsSqliteDatabase(string dbPath)
		{
			var options = new SQLiteConnectionString(dbPath,
				   AppContants.DbCreateFlags,
				   true, key: SecureStorage.GetAsync(PreferencesKey.DbKey).GetAwaiter().GetResult());
			var connection = new SQLiteAsyncConnection(options);
			if (connection == null) throw new ArgumentNullException(nameof(connection));

			connection.CreateTableAsync<DO.UserDataModel>().GetAwaiter().GetResult();
		}
	}
}