using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace IntegrativeMidterm.userControl.HelperClasses
{
    public static class ScrollViewerHelper
    {
        public static readonly DependencyProperty ScrollToEndCommandProperty =
            DependencyProperty.RegisterAttached(
                "ScrollToEndCommand",
                typeof(ICommand),
                typeof(ScrollViewerHelper),
                new PropertyMetadata(null, OnScrollToEndCommandChanged));

        public static ICommand GetScrollToEndCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(ScrollToEndCommandProperty);
        }

        public static void SetScrollToEndCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(ScrollToEndCommandProperty, value);
        }

        private static void OnScrollToEndCommandChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is ScrollViewer scrollViewer)
            {
                scrollViewer.ScrollChanged += (sender, args) =>
                {
                    if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
                    {
                        ICommand command = GetScrollToEndCommand(scrollViewer);
                        command?.Execute(scrollViewer);
                    }
                };
                scrollViewer.ScrollToVerticalOffset(scrollViewer.ScrollableHeight / 2);
            }
        }
    }
}
