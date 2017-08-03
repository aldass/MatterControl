﻿/*
Copyright (c) 2017, John Lewin
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

The views and conclusions contained in the software and documentation are those
of the authors and should not be interpreted as representing official policies,
either expressed or implied, of the FreeBSD Project.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.MatterControl.DataStorage;
using MatterHackers.MatterControl.PrintHistory;

namespace MatterHackers.MatterControl.Library
{
	public class HistoryRowItem : ILibraryContentStream
	{
		public HistoryRowItem(PrintTask printTask)
		{
			this.PrintTask = printTask;
		}

		public PrintTask PrintTask { get; }

		public long FileSize => 0;

		public string ContentType => "";

		public string FileName => "";

		public string AssetPath => "";

		public string ID { get; } = Guid.NewGuid().ToString();

		public string Name => this.PrintTask.PrintName;

		public bool IsProtected => true;

		public bool IsVisible => true;

		public Task<StreamAndLength> GetContentStream(Action<double, string> reportProgress)
		{
			throw new NotImplementedException();
		}
	}

	public class HistoryContainer : LibraryContainer
	{
		public HistoryContainer()
		{
			this.ChildContainers = new List<ILibraryContainerLink>();
			this.Items = new List<ILibraryItem>();
			this.Name = "Print History".Localize();
			this.DefaultView = new HistoryListView();

			//PrintHistoryData.Instance.ItemAdded.RegisterEvent((sender, e) => OnDataReloaded(null), ref unregisterEvent);
			
			this.ReloadContainer();
		}

		private void ReloadContainer()
		{
			Task.Run(() =>
			{
				var printHistory = PrintHistoryData.Instance.GetHistoryItems(25);

				// PrintItems projected onto FileSystemFileItem
				Items = printHistory.Select(f => new HistoryRowItem(f)).ToList<ILibraryItem>();

				UiThread.RunOnIdle(this.OnReloaded);
			});
		}
	}
}