using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Homework_09
{
	/// <summary>
	/// One message sent to or received by bot
	/// </summary>
	class Message
	{
		public long ID { get; set; }
		public string UserName { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Type { get; set; }
		public string Text { get; set; }
		public DateTime MessageDT { get; set; }
		public Message () { }
	}
}
