﻿using ObservableDictionarySample;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Buttons.Style
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableStyleDictionary _observableButtonStyles = null;

        public MainWindow()
        {
            InitializeComponent();
            _observableButtonStyles = FindResource("ButtonStyles") as ObservableStyleDictionary;
            ReloadDictionary(null, null);
        }

        private void ReloadDictionary(object sender, RoutedEventArgs e)
        {
            // cache the selected index
            int index = StyleSelector.SelectedIndex;

            ResourceDictionary buttonStyles = Application.LoadComponent(
                new Uri("ButtonStyles.xaml", UriKind.Relative)) as ResourceDictionary;
            _observableButtonStyles.Clear();
            foreach (DictionaryEntry de in buttonStyles)
            {
                if (!(de.Value is System.Windows.Style)) continue;

                if (de.Key == typeof(Button))
                {
                    _observableButtonStyles["Default Style"] = de.Value as System.Windows.Style;
                }
                else if (de.Key is string)
                {
                    _observableButtonStyles[de.Key as string] = de.Value as System.Windows.Style;
                }
            }

            // restore the selected index, if still applicable; otherwise select the first index
            if (index > -1 && StyleSelector.Items.Count > index)
            {
                StyleSelector.SelectedIndex = index;
            }
            else
            {
                StyleSelector.SelectedIndex = 0;
            }
        }

        private void AddOrRemoveButton(object sender, RoutedEventArgs e)
        {
            if (!(e.OriginalSource is Button)) return;

            System.Windows.Style selectedStyle = (e.OriginalSource as Button).Style;
            DependencyObject parentLVI = VisualTreeHelper.GetParent(e.OriginalSource as DependencyObject);
            while (parentLVI != null && !(parentLVI is ListViewItem))
                parentLVI = VisualTreeHelper.GetParent(parentLVI);
            if (parentLVI != null)
            {
                int index = StyleListView.ItemContainerGenerator.IndexFromContainer(parentLVI);
                if (index % 2 == 0)
                {
                    // if an even index, remove the style from the dictionary
                    _observableButtonStyles.Remove(((KeyValuePair<string, System.Windows.Style>)StyleListView.Items[index]).Key as string);
                }
                else
                {
                    // if an odd index, duplicate the style in the dictionary
                    string newKey = "New Style ";
                    int i = 1;
                    while (_observableButtonStyles.ContainsKey(newKey + i.ToString())) i++;
                    _observableButtonStyles[newKey + i.ToString()] = selectedStyle;
                }
            }
        }
    }
}