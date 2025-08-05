namespace AuthService
{
	public class AppConfigurations
	{
		public static IConfigurationRoot Configuration { get; }

		static AppConfigurations()
		{
			Configuration = new ConfigurationBuilder()
				.SetBasePath(AppContext.BaseDirectory)
				.AddJsonFile("appsettings.json")
				.Build();
		}

		public static string GetConnectionString(string name) =>
			Configuration.GetConnectionString(name);
	}
}
