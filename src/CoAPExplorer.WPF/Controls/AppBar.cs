using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;

using CoAPExplorer.WPF.Extensions;
using System.Windows.Data;
using CoAPExplorer.WPF.Converters;
using System.Collections;
using System.Windows.Markup;

namespace CoAPExplorer.WPF.Controls
{
    [ContentProperty("Content")]
    public class AppBar : ItemsControl
    {
        //TODO: keep track of Child nodes that have attached AppBar properties
        //TODO: Rank child nodes based on depth. The more depth, the higher the rank
        //TODO: child nodes at equal rank should be compared by visiblity.
        //TODO: the highest ranking item that's visible will have precedence over items, colorzonemode, primary and secondary bindings.

        #region DependencyProperty PrimaryAction

        public static readonly DependencyProperty PrimaryActionProperty = DependencyProperty.Register(
            nameof(PrimaryAction), typeof(object), typeof(AppBar), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty PrimaryActionTemplateProperty = DependencyProperty.Register(
            nameof(PrimaryActionTemplate), typeof(DataTemplate), typeof(AppBar), new PropertyMetadata(default(DataTemplate)));

        public static readonly DependencyProperty PrimaryActionTemplateSelectorProperty = DependencyProperty.Register(
            nameof(PrimaryActionTemplateSelector), typeof(DataTemplateSelector), typeof(AppBar), new PropertyMetadata(default(DataTemplateSelector)));

        public object PrimaryAction { get => GetValue(PrimaryActionProperty) as object; set => SetValue(PrimaryActionProperty, value); }

        public DataTemplate PrimaryActionTemplate { get => GetValue(PrimaryActionTemplateProperty) as DataTemplate; set => SetValue(PrimaryActionTemplateProperty, value); }

        public DataTemplateSelector PrimaryActionTemplateSelector { get => GetValue(PrimaryActionTemplateSelectorProperty) as DataTemplateSelector; set => SetValue(PrimaryActionTemplateSelectorProperty, value); }

        #endregion

        #region DependencyProperty Content

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            nameof(Content), typeof(object), typeof(AppBar), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty ContentTemplateProperty = DependencyProperty.Register(
            nameof(ContentTemplate), typeof(DataTemplate), typeof(AppBar), new PropertyMetadata(default(DataTemplate)));

        public static readonly DependencyProperty ContentTemplateSelectorProperty = DependencyProperty.Register(
            nameof(ContentTemplateSelector), typeof(DataTemplateSelector), typeof(AppBar), new PropertyMetadata(default(DataTemplateSelector)));

        public object Content { get => GetValue(ContentProperty) as object; set => SetValue(ContentProperty, value); }

        public DataTemplate ContentTemplate { get => GetValue(ContentTemplateProperty) as DataTemplate; set => SetValue(ContentTemplateProperty, value); }

        public DataTemplateSelector ContentTemplateSelector { get => GetValue(ContentTemplateSelectorProperty) as DataTemplateSelector; set => SetValue(ContentTemplateSelectorProperty, value); }

        #endregion

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            nameof(Title), typeof(string), typeof(AppBar), new PropertyMetadata(string.Empty));

        public string Title { get => GetValue(TitleProperty) as string; set => SetValue(TitleProperty, value); }

        public static readonly DependencyProperty ColorZoneModeProperty = DependencyProperty.Register(
            nameof(ColorZoneMode), typeof(ColorZoneMode), typeof(AppBar), new PropertyMetadata(ColorZoneMode.PrimaryMid));

        public ColorZoneMode ColorZoneMode { get => (ColorZoneMode)GetValue(ColorZoneModeProperty); set => SetValue(ColorZoneModeProperty, value); }
    }

    public static class AppBarAssist
    {
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.RegisterAttached(
            "Items", typeof(object), typeof(AppBarAssist), new PropertyMetadata(null));

        #region Items Get/Set

        public static void SetItems(DependencyObject element, object value)
        {
            element.SetValue(ItemsProperty, value);
        }

        public static object GetItems(DependencyObject element)
        {
            return element.GetValue(ItemsProperty);
        }

        #endregion

        public static readonly DependencyProperty ColorZoneModeProperty = DependencyProperty.RegisterAttached(
            "ColorZoneMode", typeof(ColorZoneMode), typeof(AppBarAssist), new FrameworkPropertyMetadata(ColorZoneMode.PrimaryMid, ColorZoneModePropertyChanged));

        #region ColorZone Get/Set

        private static void ColorZoneModePropertyChanged(DependencyObject element, DependencyPropertyChangedEventArgs eventArgs)
        {
            if (TryFindAppBar(element, out var appBar))
                CreateColorZoneModeBinding(appBar, element);
            else if (element is FrameworkElement frameworkElement && !frameworkElement.IsLoaded)
                frameworkElement.Loaded += ColorZoneModeProperty_FrameworkElement_Loaded;
        }

        private static void ColorZoneModeProperty_FrameworkElement_Loaded(object sender, RoutedEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            frameworkElement.Loaded -= ColorZoneModeProperty_FrameworkElement_Loaded;

            if (TryFindAppBar(frameworkElement, out var appBar))
                CreateColorZoneModeBinding(appBar, frameworkElement);
        }

        private static void CreateColorZoneModeBinding(AppBar appBar, DependencyObject element)
        {
            var binding = new Binding
            {
                Source = element,
                Path = new PropertyPath(AppBarAssist.ColorZoneModeProperty),
                Converter = new UIElementVisibilityToUnsetValueConverter(), // Returns DependencyProperty.UnsetValue if (parameter as UIElement).IsVisible == false
                ConverterParameter = element,
                Mode = BindingMode.OneWay,
            };

            AddToAppBarPriorityBindings(appBar, AppBar.ColorZoneModeProperty, element, binding);
        }

        public static void SetColorZoneMode(DependencyObject element, ColorZoneMode value)
        {
            element.SetValue(ColorZoneModeProperty, value);
        }

        public static ColorZoneMode GetColorZoneMode(DependencyObject element)
        {
            return (ColorZoneMode)element.GetValue(ColorZoneModeProperty);
        }

        #endregion

        private static void UpdateAppBarPropertyBinding(DependencyObject element)
        {
            if (TryFindAppBar(element, out var appBar))
            {
                var dependencyProperties 
                    = typeof(AppBar).GetFields()
                     .Where(p => p.FieldType.IsAssignableFrom(typeof(DependencyProperty)) && p.IsStatic);

                foreach (var dependencyProperty in dependencyProperties)
                {
                    var bindingExpression = BindingOperations.GetPriorityBindingExpression(appBar, dependencyProperty.GetValue(null) as DependencyProperty);

                    if (bindingExpression == null)
                        continue;

                    foreach (var bindingBase in bindingExpression.BindingExpressions)
                        if (bindingBase is BindingExpression binding && binding.ResolvedSource == element)
                            binding.UpdateTarget();

                    bindingExpression.UpdateTarget();
                }
            }
        }

        private static void AddToAppBarPriorityBindings(AppBar appBar, DependencyProperty property, DependencyObject element, BindingBase binding)
        {
            var bindingSet = new PriorityBinding();
            var existingBindingSet = BindingOperations.GetPriorityBinding(appBar, property);
            if (existingBindingSet != null)
            {
                // TODO: order by ranking
                foreach (var existingBinding in existingBindingSet.Bindings)
                    bindingSet.Bindings.Add(existingBinding);
            }

            bindingSet.Bindings.Add(binding);
            appBar.SetBinding(property, bindingSet);

            // Hooks to ensure bindings are updated when visiblity changes
            if (element is UIElement ui)
            {
                ui.IsVisibleChanged -= SourcePropertyChanged; ;
                ui.IsVisibleChanged += SourcePropertyChanged; ;
            }

            if (element is FrameworkElement fe)
            {
                fe.Loaded -= SourcePropertyChanged;
                fe.Loaded += SourcePropertyChanged;
            }
        }

        private static bool TryFindAppBar(DependencyObject element, out AppBar appBar)
        {
            appBar = null;

            // Check if the element has been loaded
            if (element is FrameworkElement frameworkElement && !frameworkElement.IsLoaded)
                return false;
            

            // Try to find an AppBar in a parent
            var parent = element;
            while (appBar == null && parent != null)
            {
                // Try again in the next parent up
                var nextParent = parent.GetParent();
                if (nextParent == null || nextParent.Equals(parent))
                    break;
                parent = nextParent;

                foreach (var child in parent.GetChildren())
                {
                    if (child is AppBar)
                    {
                        appBar = child as AppBar;
                        return true;
                    }
                }
            }

            return false;
        }

        private static void SourcePropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateAppBarPropertyBinding(sender as FrameworkElement);
        }

        private static void SourcePropertyChanged(object sender, RoutedEventArgs e)
        {
            UpdateAppBarPropertyBinding(sender as FrameworkElement);
        }
    }
}
