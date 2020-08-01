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
using Telegram.Bot.Types.Enums;
using System.Threading;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;

namespace Homework_09
{
	class TelegramBotEngine
	{
		private MainWindow w;
		private TelegramBotClient bot;
		public ObservableCollection<Message> messagesRoll { get; set; }

		public TelegramBotEngine(MainWindow W, string PathToken = "token")
		{
			this.messagesRoll = new ObservableCollection<Message>();
			this.w = W;

			bot = new TelegramBotClient(System.IO.File.ReadAllText(PathToken));

			bot.OnMessage += MessageListener;

			bot.StartReceiving();
		}

		private void MessageListener(object sender, Telegram.Bot.Args.MessageEventArgs e)
		{
			switch (e.Message.Type)
			{
				case MessageType.Text:
					TextMessageProcessor(sender, e);
					break;
				case MessageType.Document:
					GetDocument(sender, e);
					break;
				case MessageType.Photo:
					break;
				case MessageType.Audio:
					break;
				case MessageType.Voice:
					break;
				case MessageType.Video:
					break;
				case MessageType.VideoNote:
					break;
				case MessageType.Contact:
					break;
				case MessageType.Sticker:
					break;
				case MessageType.Location:
					break;
				case MessageType.Venue:
					break;
				case MessageType.Dice:
					break;
				case MessageType.Game:
					break;

				// Непонятные или устаревшие типы сообщения
				// Animation, Unknown, финансовые, технические сообщения

				default:
					break;
			}
		}

		/// <summary>
		/// Обработчик полученного сообщения
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TextMessageProcessor (object sender, Telegram.Bot.Args.MessageEventArgs e)
		{
			string username = "";
			if (e.Message.From.Username != null) username = e.Message.From.Username;
			if (e.Message.Chat.Title != null) username = e.Message.Chat.Title;
			if (e.Message.Chat.FirstName == null) e.Message.Chat.FirstName = "";
			if (e.Message.Chat.LastName == null) e.Message.Chat.LastName = "";
			if (e.Message.Text == null) e.Message.Text = "";
			w.Dispatcher.Invoke(() =>
			{
				messagesRoll.Add(new Message()
				{
					ID = e.Message.MessageId,
					ChatID = e.Message.Chat.Id,
					FromTo = username,
					UserName = username,
					FirstName = e.Message.Chat.FirstName,
					LastName = e.Message.From.LastName,
					Type = e.Message.Type.ToString(),
					Text = e.Message.Text,
					MessageDT = DateTime.Now
				});
				var border = (Border)VisualTreeHelper.GetChild(w.MessagesRoll, 0);
				var scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
				scrollViewer.ScrollToBottom();
			}
			);
		}

		/// <summary>
		/// Получает и сохраняет на диске файл документ
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void GetDocument(object sender, Telegram.Bot.Args.MessageEventArgs e)
		{

		}

		public void SendMessage(string ChatId, string toUserName, string Text)
		{
			long chatid = Convert.ToInt64(ChatId);
			bot.SendTextMessageAsync(chatid, Text);
			messagesRoll.Add(new Message()
			{
				ChatID = chatid,
				FromTo = "Bot answer to " + toUserName,
				UserName = toUserName,
				Type = "Text",
				Text = Text,
				MessageDT = DateTime.Now
			});
			var border = (Border)VisualTreeHelper.GetChild(w.MessagesRoll, 0);
			var scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
			scrollViewer.ScrollToBottom();
		}
	}
}
