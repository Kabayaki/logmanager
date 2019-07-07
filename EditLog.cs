using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LogManager
{
	public class EditLog<T> : ILog where T : class
	{
		public string Name { get; set; }

		public event EventHandler OnChange;

		readonly T Source;

		readonly object OldValue;

		readonly object NewValue;

		System.Reflection.PropertyInfo Property;

		public EditLog(T source, object oldValue, object newValue, string name, string propertyName)
		{
			Source = source;
			OldValue = oldValue;
			NewValue = newValue;
			Property = source.GetType().GetProperty(propertyName);

			Name = name;
		}

		public void Redo()
		{
			Property.SetValue(Source, NewValue);
			OnChange?.Invoke(this, EventArgs.Empty);
		}

		public void Undo()
		{
			Property.SetValue(Source, OldValue);
			OnChange?.Invoke(this, EventArgs.Empty);
		}
	}
}
