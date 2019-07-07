using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LogManager
{
	public class LogManager : BindableBase
	{
		#region Commands

		public Wpf.RelayCommand _undoCommand;
		public ICommand UndoCommand
		{
			get
			{
				if (_undoCommand == null)
				{
					_undoCommand = new Wpf.RelayCommand(p => Undo(), p => CanUndo);
				}
				return _undoCommand;
			}
		}

		public Wpf.RelayCommand _redoCommand;
		public ICommand RedoCommand
		{
			get
			{
				if (_redoCommand == null)
				{
					_redoCommand = new Wpf.RelayCommand(p => Redo(), p => CanRedo);
				}
				return _redoCommand;
			}
		}

		#endregion

		int Max;

		IList<ILog> UndoBuffer;

		IList<ILog> RedoBuffer;

		bool Executing;

		public event EventHandler OnChange;

		MultiLog MultiLog;

		#region Property

		/// <summary>
		/// 元に戻す処理可
		/// </summary>
		public bool CanUndo { get => UndoBuffer.Count > 0; }

		/// <summary>
		/// やり直し処理可
		/// </summary>
		public bool CanRedo { get => RedoBuffer.Count > 0; }

		/// <summary>
		/// 保存した位置
		/// </summary>
		int SavePoint { get; set; }

		/// <summary>
		/// 実行中
		/// </summary>
		public bool IsEdited { get => UndoBuffer.Count != SavePoint; }

		#endregion

		ObservableCollection<ILog> _actionLog;
		public ObservableCollection<ILog> ActionLog
		{
			get => _actionLog;
			set => SetProperty(ref _actionLog, value);
		}

		public LogManager(int max = 100)
		{
			ActionLog = new ObservableCollection<ILog>();

			UndoBuffer = new List<ILog>();

			RedoBuffer = new List<ILog>();

			Max = max;

			Executing = false;

			SavePoint = 0;
		}

		#region Undo or Redo

		public void Undo()
		{
			var undoAction = UndoBuffer.Last();
			Executing = true;
			UndoBuffer.Remove(undoAction);
			undoAction.Undo();
			RedoBuffer.Add(undoAction);
			Executing = false;

			OnChange?.Invoke(this, EventArgs.Empty);

			//ビューへの通知用
			RaisePropertyChanged("IsEdited");
		}

		public void Redo()
		{
			var undoAction = RedoBuffer.Last();
			Executing = true;
			RedoBuffer.Remove(undoAction);
			undoAction.Redo();
			UndoBuffer.Add(undoAction);
			Executing = false;

			OnChange?.Invoke(this, EventArgs.Empty);

			//ビューへの通知用
			RaisePropertyChanged("IsEdited");
		}

		#endregion

		/// <summary>
		/// 操作履歴をためる
		/// </summary>
		public void BeginRegister(string name)
		{
			MultiLog = new MultiLog(name);
		}

		/// <summary>
		/// 溜まっている履歴を登録する
		/// </summary>
		public void EndRegister()
		{
			var log = MultiLog;
			MultiLog = null;

			if (log.Count > 0)
			{
				Register(log);
			}
		}

		public void Cancel()
		{
			MultiLog = null;
		}

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="log"></param>
		public void Register(ILog log)
		{
			if (MultiLog != null)
			{
				MultiLog.Add(log);
			}
			else if (!Executing)
			{
				RedoBuffer.Clear();
				UndoBuffer.Add(log);

				//上限を超えたら最も古い履歴を消してインデクスを調整する
				if (UndoBuffer.Count() > Max)
				{
					UndoBuffer.RemoveAt(0);
					if (SavePoint >= 0)
					{
						SavePoint--;
					}
				}

				//保存後に元に戻して実行にて戻ってきた際は
				//保存した状態ではなくなっているのでずっと一致しないように不正値で設定する
				if (UndoBuffer.Count <= SavePoint)
				{
					SavePoint = -1;
				}

				//操作ログを再構築する
				ActionLog = new ObservableCollection<ILog>();
				ActionLog.AddRange(UndoBuffer);
				ActionLog.AddRange(RedoBuffer);

				OnChange?.Invoke(this, EventArgs.Empty);

				//ビューへの通知用
				RaisePropertyChanged("IsEdited");
			}
		}

		/// <summary>
		/// 保存位置を現在の位置に更新する
		/// </summary>
		public void UpdateSavePoint()
		{
			SavePoint = UndoBuffer.Count;

			//ビューへの通知用
			RaisePropertyChanged("IsEdited");
		}
	}
}
