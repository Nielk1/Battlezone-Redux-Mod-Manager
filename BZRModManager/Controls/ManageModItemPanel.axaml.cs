using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;
using BZRModManager.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BZRModManager.Controls
{
    public partial class ManageModItemPanel : TemplatedControl
    {
        public Task<IImage?> LiveImage => ModData?.LiveImage;
        public string Title => ModData?.Title;

        public static readonly StyledProperty<ModData?> ModDataProperty =
            AvaloniaProperty.Register<MainNavButton, ModData?>(nameof(ModData), defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);
        public ModData? ModData
        {
            get => GetValue(ModDataProperty);
            set => SetValue(ModDataProperty, value);
        }

        private Visual parent1;
        private Visual parent2;

        public ManageModItemPanel()
        {
            this.EffectiveViewportChanged += ManageModItemPanel_EffectiveViewportChanged;
        }

        private void ManageModItemPanel_EffectiveViewportChanged(object? sender, EffectiveViewportChangedEventArgs e)
        {
            parent1 ??= this.GetVisualParent();
            parent2 ??= parent1.GetVisualParent();

            //ModData?.UpdateVisibility(e.EffectiveViewport.Intersects(parent2.Bounds.WithY(parent1.Bounds.Y)));
            ModData?.UpdateVisibility(e.EffectiveViewport.Intersects(parent2.Bounds));
        }
    }
}
