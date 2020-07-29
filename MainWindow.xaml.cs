using System;
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
using System.IO;

namespace Homework_09
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			string sendFileIconFullPath = System.IO.Path.GetFullPath(@"Images\SendFilePaperClip.png");
			string sendMessageIcon = System.IO.Path.GetFullPath(@"Images\SendMessage.png");
			SendFileIcon.Source	   = new BitmapImage(new Uri(sendFileIconFullPath, UriKind.Absolute));
			SendMessageIcon.Source = new BitmapImage(new Uri(sendMessageIcon, UriKind.Absolute));
		}

		private void MessagesRoll_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{

		}
	}
}
