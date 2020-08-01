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
using System.Net;
using Newtonsoft.Json.Linq;
using Telegram.Bot;
using System.Threading;
using Telegram.Bot.Types;

namespace Homework_09
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		TelegramBotEngine engine;

		public MainWindow()
		{
			InitializeComponent();
			// Прикручиваем иконки "скрепка" для отправки файлов и "галку" для отправки сообщения
			// Папка Images\ должна быть в папке исполняемого файла - bin\Debug\
			string sendFileIconFullPath = System.IO.Path.GetFullPath(@"Images\SendFilePaperClip.png");
			string sendMessageIcon = System.IO.Path.GetFullPath(@"Images\SendMessage.png");
			SendFileIcon.Source	   = new BitmapImage(new Uri(sendFileIconFullPath, UriKind.Absolute));
			SendMessageIcon.Source = new BitmapImage(new Uri(sendMessageIcon, UriKind.Absolute));

			engine = new TelegramBotEngine(this);

			MessagesRoll.ItemsSource = engine.messagesRoll;

		}


		private void SendMessageButton_Click(object sender, RoutedEventArgs e)
		{
			if (String.IsNullOrEmpty(ContactID.Text)) return;
			engine.SendMessage(ContactID.Text, FocusContact.Text, InputMessaageField.Text);

			// Прокручиваем список, чтобы был виден его последний элемент
			// Пример нашёл в инете 
			var border = (Border)VisualTreeHelper.GetChild(MessagesRoll, 0);
			var scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
			scrollViewer.ScrollToBottom();
			// Очищаем поле ввода
			InputMessaageField.Text = "";
		}

		private void InputMessaageField_KeyDown(object sender, KeyEventArgs e)
		{
			if (String.IsNullOrEmpty(ContactID.Text)) return;
			if (e.Key == Key.Enter)
			{
				if (String.IsNullOrEmpty(InputMessaageField.Text)) return;
				engine.SendMessage(ContactID.Text, FocusContact.Text, InputMessaageField.Text);

				// Прокручиваем список, чтобы был виден его последний элемент
				// Пример нашёл в инете 
				var border = (Border)VisualTreeHelper.GetChild(MessagesRoll, 0);
				var scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
				scrollViewer.ScrollToBottom();
				// Очищаем поле ввода
				InputMessaageField.Text = "";
			}
		}
	}
}
