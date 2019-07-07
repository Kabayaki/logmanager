using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogManager
{
	public class CollectionChangeLog<T> : ILog
	{
		public string Name => throw new NotImplementedException();

		ObservableCollection<T> List;

		NotifyCollectionChangedEventArgs Args;

		public CollectionChangeLog(ObservableCollection<T> list, NotifyCollectionChangedEventArgs args)
		{
			List = list;
			Args = args;
		}

		public void Redo()
		{
			switch (Args.Action)
			{
			case NotifyCollectionChangedAction.Add:
				Add(Args.NewItems, Args.NewStartingIndex);
				break;
			case NotifyCollectionChangedAction.Remove:
				Remove(Args.OldStartingIndex, Args.OldItems.Count);
				break;
			case NotifyCollectionChangedAction.Move:
				Move(Args.OldStartingIndex, Args.NewStartingIndex);
				break;
			default:
				throw new NotImplementedException();
			}
		}

		public void Undo()
		{
			switch (Args.Action)
			{
			case NotifyCollectionChangedAction.Add:
				Remove(Args.NewStartingIndex, Args.NewItems.Count);
				break;
			case NotifyCollectionChangedAction.Remove:
				Add(Args.OldItems, Args.OldStartingIndex);
				break;
			case NotifyCollectionChangedAction.Move:
				Move(Args.NewStartingIndex, Args.OldStartingIndex);
				break;
			default:
				throw new NotImplementedException();
			}
		}

		void Add(IList newList, int newIndex)
		{
			for (var i = 0; i < newList.Count; i++)
			{
				List.Insert(newIndex + i, (T)newList[i]);
			}
		}

		void Remove(int oldIndex, int count)
		{
			for (var i = oldIndex; i < oldIndex + count; i++)
			{
				List.RemoveAt(i);
			}
		}

		void Move(int oldIndex, int newIndex)
		{
			List.Move(oldIndex, newIndex);
		}
	}
}
