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

namespace Direct3DUtilsTest
{
    public partial class BlendModeChooser : UserControl
    {
        public BlendModeChooser()
        {
            InitializeComponent();
            foreach (var item in Enum.GetValues(typeof(BlendMode)))
            {
                var rad = new RadioButton();
                rad.Tag = item;
                rad.Content = Enum.GetName(typeof(BlendMode), item);
                rad.Checked += rad_Checked;
                StackPanel.AddChild(rad);
            }
        }

        bool ignoreSelection;

        void rad_Checked(object sender, RoutedEventArgs e)
        {
            if (ignoreSelection)
            {
                ignoreSelection = false;
                return;
            }
            if (Selected!=null)
            {
                Selected(this, (BlendMode)(sender as RadioButton).Tag);
            }
        }

        public event EventHandler<BlendMode> Selected;

    }
}
