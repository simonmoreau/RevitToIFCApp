using MudBlazor;

namespace WebClient.Layout
{
    public partial class MainLayout
    {
        MudTheme appTheme = new MudTheme()
        {
            Palette = new PaletteLight()
            {
                Primary =  "#1c364a",
                Secondary = "#e74727",
                Warning = "#e74727",
                Tertiary = "#e4e7e9",
            },
            Typography = new Typography()
            {
                Default = new Default()
                {
                    FontFamily = new[] { "Titillium Web", "sans-serif" }
                }
            }
        };
    }
}
