using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using BZRModManager.Models;
using System.ComponentModel;

namespace BZRModManager.Controls
{
    public class FilterButton : TemplatedControl
    {
        public static readonly StyledProperty<string> TextProperty =
            AvaloniaProperty.Register<FilterButton, string>(nameof(Text), defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

        public string Text
        {
            get => GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        //public static readonly StyledProperty<string> ToolTipProperty =
        //    AvaloniaProperty.Register<FilterButton, string>(nameof(ToolTip), defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);
        //
        //public string ToolTip
        //{
        //    get => GetValue(ToolTipProperty);
        //    set => SetValue(ToolTipProperty, value);
        //}

        public static readonly StyledProperty<bool?> IsCheckedProperty =
            AvaloniaProperty.Register<FilterButton, bool?>(nameof(IsChecked), defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

        public bool? IsChecked
        {
            get => GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            ToggleButton? BtnFilter = e.NameScope.Find<ToggleButton>("BtnFilter");
            if (BtnFilter != null)
            {
                BtnFilter.Click += ToggleButton_Click;
                BtnFilter.PointerReleased += BtnFilter_PointerReleased;
            }

            base.OnApplyTemplate(e);
        }

        private void BtnFilter_PointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (e.InitialPressMouseButton == MouseButton.Right)
            {
                if (IsChecked == null)
                    IsChecked = false;
                else if (IsChecked == false)
                    IsChecked = true;
                else if (IsChecked == true)
                    IsChecked = null;
            }
        }

        public void ToggleButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (IsChecked == null)
                IsChecked = false;
            else if (IsChecked == false)
                IsChecked = true;
            else if (IsChecked == true)
                IsChecked = null;
        }
    }
}
