using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CoAPExplorer.WPF.Controls.Behaviors
{
    public enum TrippleClickBehavior
    {
        None,
        SelectAll
    }

    public static class TextFieldBehavior
    {
        public static readonly DependencyProperty TripleClickBehaviorProperty = DependencyProperty.RegisterAttached(
            "TripleClickBehavior", typeof(TrippleClickBehavior), typeof(TextFieldBehavior), new PropertyMetadata(TrippleClickBehavior.None, OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject element, DependencyPropertyChangedEventArgs eventArgs)
        {
            if (element is TextBox textBox)
            {
                var behavior = (TrippleClickBehavior)eventArgs.NewValue;
                if (behavior != TrippleClickBehavior.None)
                {
                    textBox.PreviewMouseLeftButtonDown += OnTextBoxMouseDown;
                }
                else
                {
                    textBox.PreviewMouseLeftButtonDown -= OnTextBoxMouseDown;
                }
            }
        }

        private static void OnTextBoxMouseDown(object sender, MouseButtonEventArgs eventArgs)
        {
            if (eventArgs.ClickCount != 3)
                return;

            switch (GetTripleClickBehavior(sender as DependencyObject))
            {
                case TrippleClickBehavior.SelectAll:
                    ((TextBox)sender).SelectAll();
                    break;
            }
        }

        public static void SetTripleClickBehavior(DependencyObject element, TrippleClickBehavior value)
        {
            element.SetValue(TripleClickBehaviorProperty, value);
        }

        public static TrippleClickBehavior GetTripleClickBehavior(DependencyObject element)
        {
            return (TrippleClickBehavior)element.GetValue(TripleClickBehaviorProperty);
        }
    }
}
