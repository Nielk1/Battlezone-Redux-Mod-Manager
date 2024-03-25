using Avalonia.Controls.Documents;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BZRModManager.Controls
{
    public class OutlinedTextBlock : Shape
    {
        static OutlinedTextBlock()
        {
            AffectsGeometry<OutlinedTextBlock>(
                BoundsProperty,
                TextProperty,
                FillProperty,
                StrokeProperty,
                StrokeThicknessProperty);
        }

        private Geometry _textGeometry;

        protected override Geometry? CreateDefiningGeometry()
        {
            CreateTextGeometry();
            return _textGeometry;
        }

        private void CreateTextGeometry()
        {
            // somehow this is getting a NULL text value from the TaskNode, so just force it to empty to hack fix for now
            var formattedText = new FormattedText(Text ?? string.Empty, Thread.CurrentThread.CurrentUICulture, FlowDirection.LeftToRight,
                                    new Typeface(FontFamily, FontStyle, FontWeight, FontStretch.Normal), FontSize, Brushes.Black)
            {
                MaxTextWidth = Width,
                TextAlignment = TextAlignment,
            };
            _textGeometry = formattedText.BuildGeometry(new Point(0, 0));
        }

        /// <summary>
        /// Defines the <see cref="Text"/> property.
        /// </summary>
        public static readonly StyledProperty<string?> TextProperty =
            AvaloniaProperty.Register<OutlinedTextBlock, string?>(nameof(Text));

        /// <summary>
        /// Defines the <see cref="FontFamily"/> property.
        /// </summary>
        public static readonly StyledProperty<FontFamily> FontFamilyProperty =
            TextElement.FontFamilyProperty.AddOwner<OutlinedTextBlock>();

        /// <summary>
        /// Defines the <see cref="FontSize"/> property.
        /// </summary>
        public static readonly StyledProperty<double> FontSizeProperty =
            TextElement.FontSizeProperty.AddOwner<OutlinedTextBlock>();

        /// <summary>
        /// Defines the <see cref="FontStyle"/> property.
        /// </summary>
        public static readonly StyledProperty<FontStyle> FontStyleProperty =
            TextElement.FontStyleProperty.AddOwner<OutlinedTextBlock>();

        /// <summary>
        /// Defines the <see cref="FontWeight"/> property.
        /// </summary>
        public static readonly StyledProperty<FontWeight> FontWeightProperty =
            TextElement.FontWeightProperty.AddOwner<OutlinedTextBlock>();

        public static readonly StyledProperty<TextAlignment> TextAlignmentProperty =
            AvaloniaProperty.Register<OutlinedTextBlock, TextAlignment>(nameof(TextAlignment));

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string? Text
        {
            get => GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        /// <summary>
        /// Gets or sets the font family used to draw the control's text.
        /// </summary>
        public FontFamily FontFamily
        {
            get { return GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        /// <summary>
        /// Gets or sets the size of the control's text in points.
        /// </summary>
        public double FontSize
        {
            get { return GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the font style used to draw the control's text.
        /// </summary>
        public FontStyle FontStyle
        {
            get { return GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the font weight used to draw the control's text.
        /// </summary>
        public FontWeight FontWeight
        {
            get { return GetValue(FontWeightProperty); }
            set { SetValue(FontWeightProperty, value); }
        }

        public TextAlignment TextAlignment
        {
            get { return GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }
    }
}
