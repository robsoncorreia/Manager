﻿using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FC.Domain._Base
{
    public static class ComboBoxItemsSourceDecorator
    {
        public static IEnumerable GetItemsSource(UIElement element)
        {
            return (IEnumerable)element.GetValue(ItemsSourceProperty);
        }

        public static void SetItemsSource(UIElement element, bool value)
        {
            element.SetValue(ItemsSourceProperty, value);
        }

        private static void ItemsSourcePropertyChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            ComboBox target = element as ComboBox;
            if (element == null)
            {
                return;
            }

            // Save original binding
            Binding originalBinding = BindingOperations.GetBinding(target, ComboBox.TextProperty);

            BindingOperations.ClearBinding(target, ComboBox.TextProperty);
            try
            {
                target.ItemsSource = e.NewValue as IEnumerable;
            }
            finally
            {
                if (originalBinding != null)
                {
                    _ = BindingOperations.SetBinding(target, ComboBox.TextProperty, originalBinding);
                }
            }
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.RegisterAttached(
                                    "ItemsSource",
            typeof(IEnumerable),
            typeof(ComboBoxItemsSourceDecorator),
            new PropertyMetadata(null, ItemsSourcePropertyChanged));
    }
}