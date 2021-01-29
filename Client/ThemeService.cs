using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Client
{
    public static class ThemeService
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached(
        "Text",
        typeof(string),
        typeof(ThemeService),
        new PropertyMetadata(null, OnTextChanged)
    );

        public static string GetText(DependencyObject d)
        { return d.GetValue(TextProperty) as string; }

        public static void SetText(DependencyObject d, string value)
        { d.SetValue(TextProperty, value); }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBlock = d as TextBlock;
            if (textBlock == null)
                return;

            textBlock.Inlines.Clear();

            var newText = (string)e.NewValue;
            if (string.IsNullOrEmpty(newText))
                return;

            String[] strings = newText.Split(null);

            foreach (String s in strings)
            {
                Match priorityMatch = new Regex(@"\([E-Z]\)").Match(s);
                Match TaskTitle = new Regex(@"\[[^\][]*]").Match(s);

                if (TaskTitle.Success)
                {
                    textBlock.Inlines.Add(new Run(s) { Foreground = Brushes.Yellow});
                }
                else if (s.Equals("(A)"))
                {
                    textBlock.Inlines.Add(new Run(s) { Foreground = Brushes.Red, FontWeight = FontWeights.Bold });
                }
                else if (s.Equals("(B)"))
                {
                    textBlock.Inlines.Add(new Run(s) { Foreground = Brushes.Orange, FontWeight = FontWeights.Bold });
                }
                else if (s.Equals("(C)"))
                {
                    textBlock.Inlines.Add(new Run(s) { Foreground = Brushes.LightGreen, FontWeight = FontWeights.Bold });
                }
                else if (s.Equals("(D)"))
                {
                    textBlock.Inlines.Add(new Run(s) { Foreground = Brushes.LightBlue, FontWeight = FontWeights.Bold });
                }
                else if (priorityMatch.Success)
                {
                    textBlock.Inlines.Add(new Run(s) { Foreground = Brushes.White, FontWeight = FontWeights.Bold });
                }
                else if (s.StartsWith("@") && s.Length > 1)
                {
                    textBlock.Inlines.Add(new Run(s) { Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(User.Default.ContextBrushColor)) });
                }
                else if (s.StartsWith("+") && s.Length > 1)
                {
                    textBlock.Inlines.Add(new Run(s) { Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(User.Default.ProjectBrushColor)) });
                }
                else if (s.StartsWith("t:") || s.StartsWith("due:"))
                {
                    textBlock.Inlines.Add(new Run(s) { Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(User.Default.DateBrushColor)) });
                }
                else if (s.StartsWith("http://") || s.StartsWith("www.") || s.StartsWith("https://"))
                {

                    // If it starts with "www." add "http://" to make it a valid Uri
                    String uri = s.StartsWith("www.") ? String.Format("https://{0}", s) : s;

                    if (!Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                        continue;

                    var link = new Hyperlink(new Run(s))
                    {
                        NavigateUri = new Uri(uri),
                        ToolTip = uri
                    };

                    link.Click += OnUrlClick;

                    textBlock.Inlines.Add(link);
                }
                else
                {
                    textBlock.Inlines.Add(new Run(s));
                }

                textBlock.Inlines.Add(" ");
            }
        }

        private static void OnUrlClick(object sender, RoutedEventArgs e)
        {
            var link = (Hyperlink)sender;
            // Do something with link.NavigateUri like:
            Process.Start(link.NavigateUri.ToString());
        }
    }

}