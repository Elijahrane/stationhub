using Avalonia;
using Avalonia.Markup.Xaml;

namespace UnitystationLauncher
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
   }
}