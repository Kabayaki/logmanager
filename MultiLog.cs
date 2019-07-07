using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogManager
{
	public class MultiLog : ILog
	{
		public string Name { get; private set; }

		IList<ILog> Logs;

		public int Count
		{
			get => Logs.Count;
		}

		public MultiLog(string name)
		{
			Name = name;
			Logs = new List<ILog>();
		}

		public void Add(ILog action)
		{
			Logs.Add(action);
		}

		public void Redo()
		{
			foreach (var action in Logs)
			{
				action.Redo();
			}
		}

		public void Undo()
		{
			for (var i = Logs.Count - 1; i >= 0; i--)
			{
				Logs[i].Undo();
			}
		}
	}
}
