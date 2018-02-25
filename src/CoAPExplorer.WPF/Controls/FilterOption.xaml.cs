﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;

namespace CoAPExplorer.WPF.Controls
{
    /// <summary>
    /// Interaction logic for FilterOption.xaml
    /// </summary>
    public partial class FilterOption : UserControl
    {
        public const PackIconKind Add = PackIconKind.Plus;
        public const PackIconKind Remove = PackIconKind.Minus;

        public FilterOption()
        {
            InitializeComponent();
        }
    }
}
