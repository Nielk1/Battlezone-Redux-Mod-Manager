using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace BZRModManager.Controls
{
    public class ManageModItemPanel : TemplatedControl
    {
        public static readonly StyledProperty<string> TitleProperty =
            AvaloniaProperty.Register<MainNavButton, string>(nameof(Title), "Title", defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

        public string Title
        {
            get => GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly StyledProperty<IImage?> ImageSourceProperty =
            AvaloniaProperty.Register<MainNavButton, IImage?>(nameof(ImageSource), defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);
        public IImage? ImageSource
        {
            get => GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }
    }
}
