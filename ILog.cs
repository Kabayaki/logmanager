﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogManager
{
	public interface ILog
	{
		string Name { get; }

		void Undo();

		void Redo();
	}
}
