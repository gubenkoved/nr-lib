using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace NRLib
{
    /// <summary>
    /// Interaction logic for ToogleImageButton.xaml
    /// </summary>
    public partial class ToogleImageButton : Button
    {
        public ImageSource DefaultImage
        {
            get { return (ImageSource)GetValue(DefaultImageProperty); }
            set { SetValue(DefaultImageProperty, value); }
        }
        public static readonly DependencyProperty DefaultImageProperty = DependencyProperty.Register("DefaultImage", typeof(ImageSource), typeof(ToogleImageButton), new UIPropertyMetadata(null));

        public ImageSource ToogledImage
        {
            get { return (ImageSource)GetValue(ToogledImageProperty); }
            set { SetValue(ToogledImageProperty, value); }
        }
        public static readonly DependencyProperty ToogledImageProperty = DependencyProperty.Register("ToogledImage", typeof(ImageSource), typeof(ToogleImageButton), new UIPropertyMetadata(null));

        public bool IsToogled
        {
            get { return (bool)GetValue(IsToogledProperty); }
            set { SetValue(IsToogledProperty, value); }
        }
        public static readonly DependencyProperty IsToogledProperty = DependencyProperty.Register("IsToogled", typeof(bool), typeof(ToogleImageButton), new UIPropertyMetadata(false, OnToogledChanged));
        private static void OnToogledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ToogleImageButton)d).OnToogledChanged();
        }
        private void OnToogledChanged()
        {
            var defaultImageControl = (Image)Template.FindName("DefaultImageControl", this);
            var toogledImageControl = (Image)Template.FindName("ToogledImageControl", this);
            
            if (IsToogled == false)
            {
                toogledImageControl.BeginAnimation(Image.OpacityProperty, m_fadeOutAnimation);
                defaultImageControl.BeginAnimation(Image.OpacityProperty, m_fadeInAnimation);
            }
            else
            {
                toogledImageControl.BeginAnimation(Image.OpacityProperty, m_fadeInAnimation);
                defaultImageControl.BeginAnimation(Image.OpacityProperty, m_fadeOutAnimation);
            }
        }        

        private DoubleAnimation m_fadeOutAnimation = new DoubleAnimation(0.0, TimeSpan.FromSeconds(0.1));
        private DoubleAnimation m_fadeInAnimation = new DoubleAnimation(1.0, TimeSpan.FromSeconds(0.1));

        public ToogleImageButton()
        {
            InitializeComponent();
        }
    }
}
