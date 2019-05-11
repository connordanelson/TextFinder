using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TextFinder.Model;

/*
 * Logic for DoubleClickBehavior class obtained from: https://stackoverflow.com/questions/4629862/attached-behavior-to-execute-command-for-listviewitem
 */
namespace TextFinder
{
    public class DoubleClickBehavior
    {
		public static DependencyProperty DoubleClickCommandProperty = DependencyProperty.RegisterAttached("DoubleClick",
			  typeof(ICommand),
			  typeof(DoubleClickBehavior),
			  new FrameworkPropertyMetadata(null, new PropertyChangedCallback(DoubleClickBehavior.DoubleClickChanged)));

		public static void SetDoubleClick(DependencyObject target, ICommand value)
		{
			target.SetValue(DoubleClickBehavior.DoubleClickCommandProperty, value);
		}

		public static ICommand GetDoubleClick(DependencyObject target)
		{
			return (ICommand)target.GetValue(DoubleClickCommandProperty);
		}

		private static void DoubleClickChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
		{
			ListViewItem element = target as ListViewItem;
			if (element != null)
			{
				if ((e.NewValue != null) && (e.OldValue == null))
				{
					element.MouseDoubleClick += element_MouseDoubleClick;
				}
				else if ((e.NewValue == null) && (e.OldValue != null))
				{
					element.MouseDoubleClick -= element_MouseDoubleClick;
				}
			}
		}

		static void element_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			UIElement element = (UIElement)sender;
			ICommand command = (ICommand)element.GetValue(DoubleClickBehavior.DoubleClickCommandProperty);
			if (sender is ListViewItem listViewItem)
			{
				if (listViewItem.DataContext is FoundFile foundFile)
				{
					command.Execute(foundFile);
				}
				else
				{
					command.Execute(null);
				}
			}
			else
			{
				command.Execute(null);
			}
		}
	}
}
