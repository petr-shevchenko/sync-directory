using System.Windows;
using System.Windows.Media;

namespace ButtonStyle
{
    internal class ImageContentPresenter
    {
        #region Image dependency property

        /// <summary>
        /// An attached dependency property which provides an
        /// <see cref="ImageSource" /> for arbitrary WPF elements.
        /// </summary>
        public static readonly DependencyProperty ImageProperty;

        /// <summary>
        /// Gets the <see cref="ImageProperty"/> for a given
        /// <see cref="DependencyObject"/>, which provides an
        /// <see cref="ImageSource" /> for arbitrary WPF elements.
        /// </summary>
        public static ImageSource GetImage(DependencyObject obj)
        {
            return (ImageSource) obj.GetValue(ImageProperty);
        }

        /// <summary>
        /// Sets the attached <see cref="ImageProperty"/> for a given
        /// <see cref="DependencyObject"/>, which provides an
        /// <see cref="ImageSource" /> for arbitrary WPF elements.
        /// </summary>
        public static void SetImage(DependencyObject obj, ImageSource value)
        {
            obj.SetValue(ImageProperty, value);
        }


        /// <summary>
        /// An attached dependency property which provides an
        /// <see cref="ImageSource" /> for arbitrary WPF elements.
        /// </summary>
        public static readonly DependencyProperty InactiveModeImageProperty;

        /// <summary>
        /// Gets the <see cref="InactiveModeImageProperty"/> for a given
        /// <see cref="DependencyObject"/>, which provides an
        /// <see cref="ImageSource" /> for arbitrary WPF elements.
        /// </summary>
        public static ImageSource GetInactiveModeImage(DependencyObject obj)
        {
            return (ImageSource)obj.GetValue(InactiveModeImageProperty);
        }

        /// <summary>
        /// Sets the attached <see cref="InactiveModeImageProperty"/> for a given
        /// <see cref="DependencyObject"/>, which provides an
        /// <see cref="ImageSource" /> for arbitrary WPF elements.
        /// </summary>
        public static void SetInactiveModeImage(DependencyObject obj, ImageSource value)
        {
            obj.SetValue(InactiveModeImageProperty, value);
        }




        /// <summary>
        /// An attached dependency property which provides an
        /// <see cref="Thickness" /> for arbitrary WPF elements.
        /// </summary>
        public static readonly DependencyProperty ImageMarginProperty;

        /// <summary>
        /// Gets the <see cref="ImageMarginProperty"/> for a given
        /// <see cref="DependencyObject"/>, which provides an
        /// <see cref="Thickness" /> for arbitrary WPF elements.
        /// </summary>
        public static Thickness GetImageMargin(DependencyObject obj)
        {
            return (Thickness)obj.GetValue(ImageMarginProperty);
        }

        /// <summary>
        /// Sets the attached <see cref="ImageMarginProperty"/> for a given
        /// <see cref="DependencyObject"/>, which provides an
        /// <see cref="Thickness" /> for arbitrary WPF elements.
        /// </summary>
        public static void SetImageMargin(DependencyObject obj, Thickness value)
        {
            obj.SetValue(ImageMarginProperty, value);
        }

        #endregion

        static ImageContentPresenter()
        {
            //register attached dependency property
            var metadata = new FrameworkPropertyMetadata((ImageSource) null);
            var metadataForInactiveImage = new FrameworkPropertyMetadata((ImageSource) null);
            var metadataForImageMargin = new FrameworkPropertyMetadata(new Thickness());

            ImageProperty = DependencyProperty.RegisterAttached("Image",
                typeof (ImageSource),
                typeof (ImageContentPresenter), metadata);

            InactiveModeImageProperty = DependencyProperty.RegisterAttached("InactiveModeImage",
                typeof (ImageSource),
                typeof (ImageContentPresenter), metadataForInactiveImage);

            ImageMarginProperty = DependencyProperty.RegisterAttached("ImageMargin",
                typeof(Thickness),
                typeof(ImageContentPresenter), metadataForImageMargin);
        }
    }
}
