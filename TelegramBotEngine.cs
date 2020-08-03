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
using Telegram.Bot.Types.InputFiles;

namespace Homework_09
{
	class TelegramBotEngine
	{
		private MainWindow w;
		static TelegramBotClient bot;
		public ObservableCollection<Message> messagesRoll { get; set; }
		public ObservableCollection<long> contactList { get; set; }
		private string token;

		public TelegramBotEngine(MainWindow W)
		{
			this.messagesRoll = new ObservableCollection<Message>();
			this.w = W;
			this.contactList = new ObservableCollection<long>();
			token = System.IO.File.ReadAllText("token");
			bot = new TelegramBotClient(token);

			bot.OnMessage += MessageListener;

			bot.StartReceiving();
		}
		
		private bool NotInContactList (long chat_id)
		{
			return !contactList.Contains(chat_id);
		}

		private void MessageListener(object sender, Telegram.Bot.Args.MessageEventArgs e)
		{
			WebClient	wc		 = new WebClient();
			string		baseUrl  = $@"https://api.telegram.org/bot{token}/getFile?file_id=";
			string		fileName = "";
			string		fileExt  = "";
			JObject		fileInfo;
			InputOnlineFile iof;

			// собираем адресную книгу. Это для 10 домашнего задания
			if (NotInContactList(e.Message.Chat.Id)) contactList.Add(e.Message.Chat.Id);

			if (e.Message.Type == MessageType.Text)
			{
				TextMessageProcessor(sender, e);
				return;
			}
			else
			{
				switch (e.Message.Type)
				{
					// Всяческие файлы, включая отдельные фото(!), которые пересылают, как файлы
					case MessageType.Document:

						if (e.Message.Document.FileSize > 20_971_520)
						{
							// Выдаём в чат сообщение о невозможности скачать файл
							e.Message.Text = $"К сожалению, сейчас Телеграм бот не может передавать файлы " +
								   "размером больше 20 Мб.";

						}
						else
						{
							DownloadFile(e.Message.Document.FileId, e.Message.Document.FileName);
							e.Message.Text = $"{e.Message.Document.FileName}\n" +
								   $"Размер: {e.Message.Document.FileSize:##,# bytes}";

							fileName = e.Message.Document.FileName;
							// Отсылаем файл назад
							iof = new InputOnlineFile(e.Message.Document.FileId);
							bot.SendDocumentAsync(e.Message.Chat.Id, iof);
						}
						break;

					case MessageType.Photo:

						PhotoSize photo = e.Message.Photo[2];
						if (photo.FileSize > 20_971_520)
						{
							// Выдаём в чат сообщение о невозможности скачать файл
							e.Message.Text = $"К сожалению, сейчас Телеграм бот не может передавать файлы " +
									"размером больше 20 Мб.";
						}
						else
						{
							fileInfo = JObject.Parse(wc.DownloadString(baseUrl + photo.FileId));

							// Создаём уникальное имя файла
							fileExt = System.IO.Path.GetExtension((string)fileInfo["result"]["file_path"]);
							fileName = photo.FileUniqueId + fileExt;

							// скачиваем файл на диск
							DownloadFile(photo.FileId, fileName);

							// Выдаём в чат сообщение о полученном файле
							e.Message.Text = $"Фото сохранено в файле {fileName}\n" +
									$"Размер: {photo.FileSize:##,# байт}";

							// Отсылаем файл назад
							iof = new InputOnlineFile(photo.FileId);
							bot.SendPhotoAsync(e.Message.Chat.Id, iof);
						}
						break;
					
					case MessageType.Audio:

						if (e.Message.Audio.FileSize > 20_971_520)
						{
							// Выдаём в чат сообщение о невозможности скачать файл
							e.Message.Text = $"К сожалению, сейчас Телеграм бот не может передавать файлы " +
								   "размером больше 20 Мб.";

						}
						else
						{
							fileInfo = JObject.Parse(wc.DownloadString(baseUrl + e.Message.Audio.FileId));

							// Пытаемся создать нормальное и в то же время уникальное имя файла
							fileExt = System.IO.Path.GetExtension((string)fileInfo["result"]["file_path"]);
							fileName = e.Message.Audio.Title + "_" +
									   e.Message.Audio.Performer + "_" +
									   e.Message.Audio.FileUniqueId +
									   fileExt;

							// скачиваем файл на диск
							DownloadFile(e.Message.Audio.FileId, fileName);

							// Выдаём в чат сообщение о полученном файле
							e.Message.Text = $"Аудио файл {fileName}\n" +
								   $"Размер: {e.Message.Audio.FileSize:##,# байт}";

							// Отсылаем файл назад
							iof = new InputOnlineFile(e.Message.Audio.FileId);
							bot.SendAudioAsync(e.Message.Chat.Id, iof);
						}
						break;

					case MessageType.Voice:

						if (e.Message.Voice.FileSize > 20_971_520)
						{
							// Выдаём в чат сообщение о невозможности скачать файл
							e.Message.Text = $"К сожалению, сейчас Телеграм бот не может передавать файлы " +
								   "размером больше 20 Мб.";
						}
						else
						{
							fileInfo = JObject.Parse(wc.DownloadString(baseUrl + e.Message.Voice.FileId));

							// Создаём уникальное имя файла
							fileExt = System.IO.Path.GetExtension((string)fileInfo["result"]["file_path"]);
							fileName = e.Message.Voice.FileUniqueId + fileExt;

							// скачиваем файл на диск
							DownloadFile(e.Message.Voice.FileId, fileName);

							// Выдаём в чат сообщение о полученном файле
							e.Message.Text = $"Голосовое сообщение сохранено в файле {fileName}\n" +
								   $"Размер: {e.Message.Voice.FileSize:##,# байт}";

							// Отсылаем файл назад
							iof = new InputOnlineFile(e.Message.Voice.FileId);
							bot.SendVoiceAsync(e.Message.Chat.Id, iof);
						}
						break;

					case MessageType.Video:

						if (e.Message.Video.FileSize > 20_971_520)
						{
							// Выдаём в чат сообщение о невозможности скачать файл
							e.Message.Text = $"К сожалению, сейчас Телеграм бот не может передавать файлы " +
								   "размером больше 20 Мб.";
						}
						else
						{
							fileInfo = JObject.Parse(wc.DownloadString(baseUrl + e.Message.Video.FileId));

							// Создаём уникальное имя файла
							fileExt = System.IO.Path.GetExtension((string)fileInfo["result"]["file_path"]);
							fileName = e.Message.Video.FileUniqueId + fileExt;

							// скачиваем файл на диск
							DownloadFile(e.Message.Video.FileId, fileName);

							// Выдаём в чат сообщение о полученном файле
							e.Message.Text = $"Видео файл {fileName}\n" +
								   $"Размер: {e.Message.Video.FileSize:##,# байт}";

							// Отсылаем файл назад
							iof = new InputOnlineFile(e.Message.Video.FileId);
							bot.SendVideoAsync(e.Message.Chat.Id, iof);
						}
						break;

					case MessageType.VideoNote:

						if (e.Message.VideoNote.FileSize > 20_971_520)
						{
							// Выдаём в чат сообщение о невозможности скачать файл
							e.Message.Text = $"К сожалению, сейчас Телеграм бот не может передавать файлы " +
								   "размером больше 20 Мб.";
						}
						else
						{
							fileInfo = JObject.Parse(wc.DownloadString(baseUrl + e.Message.VideoNote.FileId));

							// Создаём уникальное имя файла
							fileExt = System.IO.Path.GetExtension((string)fileInfo["result"]["file_path"]);
							fileName = e.Message.VideoNote.FileUniqueId + fileExt;

							// скачиваем файл на диск
							DownloadFile(e.Message.VideoNote.FileId, fileName);

							// Выдаём в чат сообщение о полученном файле
							e.Message.Text = $"Видео сообщение сохранено в файл {fileName}\n" +
								   $"Размер: {e.Message.VideoNote.FileSize:##,# байт}";

							// Отсылаем файл назад
							InputTelegramFile itf = new InputTelegramFile(e.Message.VideoNote.FileId);
							bot.SendVideoNoteAsync(e.Message.Chat.Id, itf);
						}
						break;

					case MessageType.Sticker:

						if (e.Message.Sticker.FileSize > 20_971_520)
						{
							// Выдаём в чат сообщение о невозможности скачать файл
							e.Message.Text = $"К сожалению, сейчас Телеграм бот не может передавать файлы " +
								   "размером больше 20 Мб.";

						}
						else
						{
							fileInfo = JObject.Parse(wc.DownloadString(baseUrl + e.Message.Sticker.FileId));

							// Пытаемся создать нормальное и в то же время уникальное имя файла
							fileExt = System.IO.Path.GetExtension((string)fileInfo["result"]["file_path"]);
							fileName = e.Message.Sticker.Emoji + "_" +
									   e.Message.Sticker.SetName + "_" +
									   e.Message.Sticker.FileUniqueId +
									   fileExt;

							// скачиваем файл на диск
							DownloadFile(e.Message.Sticker.FileId, fileName);

							// Выдаём в чат сообщение о полученном файле
							e.Message.Text = $"Стикер сохранён в файл {fileName}\n" +
								   $"Размер: {e.Message.Sticker.FileSize:##,# байт}";

							// Отсылаем файл назад
							iof = new InputOnlineFile(e.Message.Sticker.FileId);
							bot.SendStickerAsync(e.Message.Chat.Id, iof);
						}
						break;

						// Cообщения других типов игнорируем
					default:
						return;
				}
			}
			TextMessageProcessor(sender, e);
			w.Dispatcher.Invoke(() =>
			{
				messagesRoll.Add(new Message()
				{
					ChatID = e.Message.Chat.Id,
					FromTo = "Bot answer to " + e.Message.From.Username,
					UserName = e.Message.From.Username,
					Type = e.Message.Type.ToString(),
					Text = "Возвращаем файл\n" + fileName,
					MessageDT = DateTime.Now
				});

				// Прокручиваем список, чтобы был виден его последний элемент
				// Пример нашёл в инете 
				var border = (Border)VisualTreeHelper.GetChild(w.MessagesRoll, 0);
				var scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
				scrollViewer.ScrollToBottom();
			});
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
					LastName = e.Message.Chat.LastName,
					Type = e.Message.Type.ToString(),
					Text = e.Message.Text,
					MessageDT = DateTime.Now
				});
				// Прокручиваем список, чтобы был виден его последний элемент
				// Пример нашёл в инете 
				var border = (Border)VisualTreeHelper.GetChild(w.MessagesRoll, 0);
				var scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
				scrollViewer.ScrollToBottom();
			}
			);
		}

		/// <summary>
		/// Получает и сохраняет на диске файл документ
		/// </summary>
		/// <param name="fileId"></param>
		/// <param name="path"></param>
		static async void DownloadFile(string fileId, string path)
		{
			var file = await bot.GetFileAsync(fileId);
			FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
			await bot.DownloadFileAsync(file.FilePath, fs);
			fs.Close();

			fs.Dispose();
		}

		/// <summary>
		/// Для отправки текстовых сообщений, набранных вручную
		/// </summary>
		/// <param name="ChatId"></param>
		/// <param name="toUserName"></param>
		/// <param name="Text"></param>
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

			// Прокручиваем список, чтобы был виден его последний элемент
			// Пример нашёл в инете 
			var border = (Border)VisualTreeHelper.GetChild(w.MessagesRoll, 0);
			var scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
			scrollViewer.ScrollToBottom();
		}

		public async void SendFile(object destination, string fullFileName)
		{
			Message d = destination as Message; 
			string shortfn = System.IO.Path.GetFileName(fullFileName);
			using (Stream fs = new FileStream(fullFileName, FileMode.Open, FileAccess.Read))
			{
				await bot.SendDocumentAsync(d.ChatID, new InputOnlineFile(fs, shortfn));
			}
			w.Dispatcher.Invoke(() =>
			{
				messagesRoll.Add(new Message()
				{
					ID = ++messagesRoll.Last().ID,
					ChatID = d.ChatID,
					FromTo = "Bot answer to " + d.FromTo,
					UserName = d.UserName,
					FirstName = d.FirstName,
					LastName = d.LastName,
					Type = "SendingFile",
					Text = "Посылаем " + shortfn,
					MessageDT = DateTime.Now
				});
				// Прокручиваем список, чтобы был виден его последний элемент
				// Пример нашёл в инете 
				var border = (Border)VisualTreeHelper.GetChild(w.MessagesRoll, 0);
				var scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
				scrollViewer.ScrollToBottom();
			}
			);
		}
	}
}
