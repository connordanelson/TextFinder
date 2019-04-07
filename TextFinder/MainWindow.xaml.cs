using System.Windows;
using TextFinder.ViewModel;

namespace TextFinder
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			this.DataContext = new TextFinderViewModel();
		}

		//private void TextFinderViewControl_Loaded(object sender, RoutedEventArgs e)
		//{
		//	TextFinderViewControl.DataContext = new TextFinderViewModel();
		//}
	}
}