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

			//MessagesRoll.ItemsSource = messagesRoll;

			//// Запускаем бота
			//token = File.ReadAllText(
			//		System.IO.Path.GetFullPath("token"), Encoding.UTF8);
			//token = token.Substring(0, 46);

			//wc = new WebClient() { Encoding = Encoding.UTF8 };
			//int update_id = 0;
			//string startUrl = $@"https://api.telegram.org/bot{token}/";

			//this.Dispatcher.Invoke(() =>
			//{
			//	//while (true)
			//	//{
			//		string url = $"{startUrl}getUpdates?offset={update_id}";
			//		var r = wc.DownloadString(url);

			//		var msgs = JObject.Parse(r)["result"].ToArray();

			//		foreach (dynamic msg in msgs)
			//		{
			//			messagesRoll.Add(new Message()
			//			{
			//				ID = msg.message.from.id,
			//				Text = msg.message.text,
			//				MessageDT = DateTime.FromBinary((long)msg.message.date)
			//			});
			//		}
			//		Thread.Sleep(100);
			//	//}
			//});
		}


		private void SendMessageButton_Click(object sender, RoutedEventArgs e)
		{
			engine.SendMessage(InputMessaageField.Text, );
			// Прокручиваем список, чтобы был виден его последний элемент
			// Пример нашёл в инете 
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
				// Прокручиваем список, чтобы был виден его последний элемент
				// Пример нашёл в инете 
				var border = (Border)VisualTreeHelper.GetChild(MessagesRoll, 0);
				var scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
				scrollViewer.ScrollToBottom();

				InputMessaageField.Text = "";
			}
		}
	}
}
