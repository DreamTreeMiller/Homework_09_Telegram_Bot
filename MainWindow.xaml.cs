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
using System.Collections.ObjectModel;

namespace Homework_09
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		ObservableCollection<Message> messagesRoll = new ObservableCollection<Message>();
		int globalID = 0; // only for testing purpose
		public MainWindow()
		{
			InitializeComponent();
			// Прикручиваем иконки "скрепка" для отправки файлов и "галку" для отправки сообщения
			// Папка Images\ должна быть в папке исполняемого файла - bin\Debug\
			string sendFileIconFullPath = System.IO.Path.GetFullPath(@"Images\SendFilePaperClip.png");
			string sendMessageIcon = System.IO.Path.GetFullPath(@"Images\SendMessage.png");
			SendFileIcon.Source	   = new BitmapImage(new Uri(sendFileIconFullPath, UriKind.Absolute));
			SendMessageIcon.Source = new BitmapImage(new Uri(sendMessageIcon, UriKind.Absolute));

			MessagesRoll.ItemsSource = messagesRoll;

		}


		private void SendMessageButton_Click(object sender, RoutedEventArgs e)
		{
			if (String.IsNullOrEmpty(InputMessaageField.Text)) return;
			Message newMsg = new Message()
			{
				ID = ++globalID,
				Text = InputMessaageField.Text,
				MessageDT = DateTime.Now
			};
			messagesRoll.Add(newMsg);
			var border = (Border)VisualTreeHelper.GetChild(MessagesRoll, 0);
			var scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
			scrollViewer.ScrollToBottom();
			InputMessaageField.Text = "";
		}

		private void InputMessaageField_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				if (String.IsNullOrEmpty(InputMessaageField.Text)) return;
				Message newMsg = new Message()
				{
					ID = ++globalID,
					Text = InputMessaageField.Text,
					MessageDT = DateTime.Now
				};
				messagesRoll.Add(newMsg);
				var border = (Border)VisualTreeHelper.GetChild(MessagesRoll, 0);
				var scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
				scrollViewer.ScrollToBottom();
				InputMessaageField.Text = "";
			}
		}
	}
}
