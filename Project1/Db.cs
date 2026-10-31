using System.Configuration;

namespace Project1
{
    // Single place the forms get the database connection string from.
    // The value lives in ConnectionStrings.config, which App.config pulls in and git ignores,
    // so the password never ends up on GitHub. See ConnectionStrings.example.config.
    internal static class Db
    {
        private const string Name = "railway";

        private const string SetupHelp =
            "The database connection is not set up.\n\n" +
            "In the Project1 folder, copy ConnectionStrings.example.config to ConnectionStrings.config, " +
            "put the real database password in it, then rebuild the app.";

        public static string ConnectionString
        {
            get
            {
                ConnectionStringSettings settings;
                try
                {
                    settings = ConfigurationManager.ConnectionStrings[Name];
                }
                catch (ConfigurationErrorsException ex)
                {
                    throw new ConfigurationErrorsException(SetupHelp + "\n\nDetails: " + ex.Message, ex);
                }

                if (settings == null || string.IsNullOrWhiteSpace(settings.ConnectionString))
                    throw new ConfigurationErrorsException(SetupHelp);

                return settings.ConnectionString;
            }
        }

        // Lets Program.Main show one clear message at startup instead of failing inside a form.
        public static bool IsConfigured(out string error)
        {
            try
            {
                string unused = ConnectionString;
                error = null;
                return true;
            }
            catch (ConfigurationErrorsException ex)
            {
                error = ex.Message;
                return false;
            }
        }
    }
}
