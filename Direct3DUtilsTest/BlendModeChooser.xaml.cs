using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Direct3DUtils;
using System.Windows.Media;

namespace Direct3DUtilsTest
{
    

    public partial class BlendModeChooser : UserControl
    {
        Color[] BColors = new Color[] {Colors.Red, Colors.Black, Colors.Blue, Colors.Brown, Colors.Cyan, Colors.DarkGray, Colors.Gray, Colors.Green, Colors.LightGray, Colors.Magenta, Colors.Orange, Colors.Purple,  Colors.White, Colors.Yellow };

        public BlendModeChooser()
        {
            InitializeComponent();
            ignoreSelection = true;
            foreach (var item in Enum.GetValues(typeof(BlendMode)))
            {
                var rad = new RadioButton();
                rad.Tag = item;
                rad.Content = Enum.GetName(typeof(BlendMode), item);
                rad.Checked += blendMode_Checked;
                StackPanel.AddChild(rad);
            }
            var arr = Enum.GetValues(typeof(FillMode));
            foreach (var item in arr)
            {
                var rad = new RadioButton();
                rad.Tag = item;
                rad.Content = Enum.GetName(typeof(FillMode), item);
                rad.Checked += fillMode_Checked;
                FillModePanel.AddChild(rad,0);
            }
            foreach (var item in BColors)
            {
                var rad = new RadioButton();
                rad.Tag = item;
                rad.Content = new Border() { Width = 150, Height=50, BorderBrush = new SolidColorBrush(Colors.White), BorderThickness = new Thickness(2), Child = new Grid() {Background = new SolidColorBrush(item)}};
                rad.Checked += color_Checked;
                ColorsPanel.AddChild(rad);
            }
            ignoreSelection = false;
        }

        public struct ResetParams
        {
            public static ResetParams ResetParamsZiro()
            {
                return new ResetParams()
                {
                    ColorButton = null,
                    BlendModeButton = null,
                    FillModeButton = null,
                    BlendColorAlpha = 150,
                    SpriteAlpha = 255
                };
            }
            public RadioButton ColorButton{get; internal set;}
            public RadioButton BlendModeButton { get; internal set; }
            public RadioButton FillModeButton { get; internal set; }
            public double BlendColorAlpha { get; set; }
            public double SpriteAlpha { get; set; }
        }

        ResetParams currentParrams = new ResetParams();

        public ResetParams CurrentParrams
        {
            set
            {
                currentParrams = value;
                if (currentParrams.ColorButton == null)
                {
                    currentParrams.ColorButton = FirstRadioButton(ColorsPanel);
                }
                if (currentParrams.FillModeButton == null)
                {
                    currentParrams.FillModeButton = FirstRadioButton(FillModePanel);
                }
                if (currentParrams.BlendModeButton == null)
                {
                    currentParrams.BlendModeButton = FirstRadioButton(StackPanel);
                }
                ignoreSelection = true;
                try
                {
                    currentParrams.BlendModeButton.IsChecked = true;
                    currentParrams.FillModeButton.IsChecked = true;
                    currentParrams.ColorButton.IsChecked = true;
                    alphaSlider.Value = currentParrams.BlendColorAlpha;
                    SpriteAlphaSlider.Value = currentParrams.SpriteAlpha;
                }
                finally
                {
                    ignoreSelection = false;
                }
            }
            get { return currentParrams; }
        }

        RadioButton FirstRadioButton(StackPanel panel)
        {
            foreach (var item in panel.Children)
            {
                if (item is RadioButton)
                {
                    return item as RadioButton;
                }
            }
            return null;
        }

        bool ignoreSelection;

        void blendMode_Checked(object sender, RoutedEventArgs e)
        {
            
            if (ignoreSelection)
            {
                ignoreSelection = false;
                return;
            }
            currentParrams.BlendModeButton = sender as RadioButton;
            if (SelectedBlendMode!=null)
            {
                SelectedBlendMode(this, (BlendMode)(sender as RadioButton).Tag);
            }
        }

        void fillMode_Checked(object sender, RoutedEventArgs e)
        {

            if (ignoreSelection)
            {
                ignoreSelection = false;
                return;
            }
            currentParrams.FillModeButton = sender as RadioButton;
            if (SelectedFillMode != null)
            {
                SelectedFillMode(this, (FillMode)(sender as RadioButton).Tag);
            }
        }

        void color_Checked(object sender, RoutedEventArgs e)
        {

            if (ignoreSelection)
            {
                ignoreSelection = false;
                return;
            }
            currentParrams.ColorButton = sender as RadioButton;
            UpdateColor();
        }

        void UpdateColor()
        {
            if (SelectedColor != null)
            {
                Color currentColor = (Color)(currentParrams.ColorButton).Tag;
                SelectedColor(this, Color.FromArgb((byte)currentParrams.BlendColorAlpha, currentColor.R, currentColor.G, currentColor.B));
            }
        }

        public event EventHandler<BlendMode> SelectedBlendMode;
        public event EventHandler<FillMode> SelectedFillMode;
        public event EventHandler<double> SelectedSpriteAlpha;
        public event EventHandler<Color> SelectedColor;

        private void LayoutRoot_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            e.Handled = true;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ignoreSelection)
            {
                ignoreSelection = false;
                return;
            }
            currentParrams.BlendColorAlpha =  e.NewValue;
            UpdateColor();
        }

        private void SpriteAlphaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ignoreSelection)
            {
                ignoreSelection = false;
                return;
            }
            currentParrams.SpriteAlpha = e.NewValue;
            if (SelectedSpriteAlpha!=null)
            {
                SelectedSpriteAlpha(this, currentParrams.SpriteAlpha);
            }
        }

    }
}
