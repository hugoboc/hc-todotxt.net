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

        private const string context_pattern = @"(^|\s)(?<context>\@[^\s]+)";

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
                Match priorityMatch = new Regex(@"^(?<priority>\([A-Z]\)\s)").Match(s);
                Match projectMatch = new Regex(@"(?<proj>(?<=^|\s)\+[^\s]+)").Match(s);
                Match contextMatch = new Regex(@"(^|\s)(?<context>\@[^\s]+)").Match(s);
                Match task_titleMatch = new Regex(@"\[[^\][]*]").Match(s);
                Match created_dateMatch = new Regex(@"(?<date>(\d{4})-(\d{2})-(\d{2}))").Match(s);
                Match due_dateMatch = new Regex(@"due:(?<date>(\d{4})-(\d{2})-(\d{2}))").Match(s);
                Match mailMatch = new Regex(@"(E2T)(?!.*E2T)\S+").Match(s);
                Match update_dateMatch = new Regex(@"(?<date>(\d{2})\D(\d{2})\D(\d{4}))").Match(s);


                if (s.Equals("(A)"))
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
                else if (task_titleMatch.Success)
                {
                    textBlock.Inlines.Add(new Run(s) { Foreground = Brushes.Gold });
                }
                else if (contextMatch.Success)
                {
                    textBlock.Inlines.Add(new Run(s) { Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(User.Default.ContextBrushColor)) });
                }
                else if (projectMatch.Success)
                {
                    textBlock.Inlines.Add(new Run(s) { Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(User.Default.ProjectBrushColor)) });
                }
                else if (due_dateMatch.Success)
                {
                    textBlock.Inlines.Add(new Run(s) { Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(User.Default.DateBrushColor)) });
                }
                else if (mailMatch.Success)
                {
                    textBlock.Inlines.Add(new Run(s) { Foreground = Brushes.MediumTurquoise });
                }
                else if (update_dateMatch.Success)
                {
                    textBlock.Inlines.Add(new Run(s) { Foreground = Brushes.SkyBlue });
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