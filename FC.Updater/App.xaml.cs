using Parse;
using System.Windows;

namespace FC.Updater
{
    /// <summary>
    /// Interação lógica para App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string Version = "1.0.0.28";

        public App()
        {
            ParseClient.Initialize(new ParseClient.Configuration
            {
#if DEBUG
                ApplicationId = "GjKbaiaVLfSuO9fO1qnJZAwQsZEFlXHX",
                Server = "https://flex-cloud-us-01-mbdlxel4b6gjo.herokuapp.com/parse/"
#else
                ApplicationId = "p49yBEN5DkGVCVa7TT2Yg7xmAqVEMhQj",
                Server = "https://flex-cloud-us-01-ic32l6urlch2a.herokuapp.com/parse/"
#endif
            });
        }
    }
}