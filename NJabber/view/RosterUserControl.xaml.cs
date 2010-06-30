using System;
using System.Collections;
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

namespace NJabber.view
{
    /// <summary>
    /// Interaction logic for RosterUserControl.xaml
    /// </summary>
    public partial class RosterUserControl : UserControl,IDisplay
    {
        public RosterUserControl()
        {
            InitializeComponent();
            DataContextChanged += RosterUserControlDataContextChanged;
        }

        void RosterUserControlDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            listbox.ItemsSource = (IEnumerable) DataContext;
            
        }
     
    }
}
