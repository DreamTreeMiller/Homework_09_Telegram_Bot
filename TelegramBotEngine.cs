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
	class TelegramBotEngine
	{
		private MainWindow w;
		private TelegramBotClient bot;
		public ObservableCollection<Message> messagesRoll { get; set; }

		public TelegramBotEngine(MainWindow W, string PathToken = "token")
		{
			this.messagesRoll = new ObservableCollection<Message>();
			this.w = W;

			bot = new TelegramBotClient(File.ReadAllText(PathToken));

			bot.OnMessage += MessageListener;

			bot.StartReceiving();
		}

		private void MessageListener(object sender, Telegram.Bot.Args.MessageEventArgs e)
		{
			if (e.Message.Text == null) e.Message.Text = "";
			w.Dispatcher.Invoke(() =>
								messagesRoll.Add(new Message()
								{
									ID = e.Message.MessageId,
									UserName = e.Message.From.Username,
									FirstName = e.Message.From.FirstName,
									LastName = e.Message.From.LastName,
									Type = e.Message.Type.ToString(),
									Text = e.Message.Text,
									MessageDT = e.Message.Date
								})
			);
		}
		public void SendMessage(string Text, string ChatId)
		{
			long chatid = Convert.ToInt64(ChatId);
			bot.SendTextMessageAsync(chatid, Text);
		}
	}
}
