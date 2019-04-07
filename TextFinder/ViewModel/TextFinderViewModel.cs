using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using TextFinder.Model;
using TextFinder.Settings;

namespace TextFinder.ViewModel
{
	public class TextFinderViewModel : ViewModelBase, IDataErrorInfo
	{
		private BackgroundWorker _backgroundWorker = null;

		public TextFinderViewModel()
		{
			FoundFiles = new ObservableCollection<FoundFile>();
			SearchCommand = new DelegateCommand(_Search);
			CancelCommand = new DelegateCommand(_Cancel);
			UpdateFoundTextLinesCommand = new DelegateCommand<FoundFile>(_UpdateFoundTextLines);
			SuggestedSearchPaths = new ObservableCollection<string>();
			OpenSelectedFileCommand = new DelegateCommand<FoundFile>(_OpenSelectedFile);
			CopyToClipboardCommand = new DelegateCommand<FoundTextLine>(_CopyToClipboard);
			OpenContainingFolderCommand = new DelegateCommand<FoundFile>(_OpenContainingFolder);

			_backgroundWorker = new BackgroundWorker();
			_backgroundWorker.WorkerSupportsCancellation = true;
			_backgroundWorker.WorkerReportsProgress = true;
			_backgroundWorker.DoWork += worker_Search;
			_backgroundWorker.ProgressChanged += worker_ProgressChanged;
			_backgroundWorker.RunWorkerCompleted += worker_SearchCompleted;

			_SetDefaultFromSettings();
		}

		private string _searchText;

		public string SearchText
		{
			get
			{
				return _searchText;
			}
			set
			{
				if (_searchText != value)
				{
					_searchText = value;
					OnPropertyChanged("SearchText");
				}
			}
		}

		private bool _isSearching;

		public bool IsSearching
		{
			get
			{
				return _isSearching;
			}
			set
			{
				if (_isSearching != value)
				{
					_isSearching = value;
					OnPropertyChanged("IsSearching");
				}
			}
		}

		private string _searchPath;

		public string SearchPath
		{
			get
			{
				return _searchPath;
			}
			set
			{
				if (_searchPath != value)
				{
					_searchPath = value;
					OnPropertyChanged("SearchPath");

					_UpdateSuggestedSearchPaths();
				}
			}
		}

		private bool _matchSearchTextCase;

		public bool MatchSearchTextCase
		{
			get
			{
				return _matchSearchTextCase;
			}
			set
			{
				if (_matchSearchTextCase != value)
				{
					_matchSearchTextCase = value;
					OnPropertyChanged("MatchSearchTextCase");
				}
			}
		}

		private int? _linesBetweenSearchText;

		public int? LinesBetweenSearchText
		{
			get
			{
				return _linesBetweenSearchText;
			}
			set
			{
				if (_linesBetweenSearchText != value)
				{
					_linesBetweenSearchText = value;
					OnPropertyChanged("LinesBetweenSearchText");
				}
			}
		}

		private bool _searchSubdirectories;

		public bool SearchSubdirectories
		{
			get
			{
				return _searchSubdirectories;
			}
			set
			{
				if (_searchSubdirectories != value)
				{
					_searchSubdirectories = value;
					OnPropertyChanged("SearchSubdirectories");
				}
			}
		}

		private bool _checkNumberOfLinesBetweenSearchTextEntries;

		public bool CheckNumberOfLinesBetweenSearchTextEntries
		{
			get
			{
				return _checkNumberOfLinesBetweenSearchTextEntries;
			}
			set
			{
				if (_checkNumberOfLinesBetweenSearchTextEntries != value)
				{
					_checkNumberOfLinesBetweenSearchTextEntries = value;
					OnPropertyChanged("CheckNumberOfLinesBetweenSearchTextEntries");
					OnPropertyChanged("LinesBetweenSearchText");
				}
			}
		}

		private string _excludeFiles;

		public string ExcludeFiles
		{
			get
			{
				return _excludeFiles;
			}
			set
			{
				if (_excludeFiles != value)
				{
					_excludeFiles = value;
					OnPropertyChanged("ExcludeFiles");
				}
			}
		}

		private string _includeFiles;

		public string IncludeFiles
		{
			get
			{
				return _includeFiles;
			}
			set
			{
				if (_includeFiles != value)
				{
					_includeFiles = value;
					OnPropertyChanged("IncludeFiles");
				}
			}
		}

		private DateTime? _modifiedBeforeDate;

		public DateTime? ModifiedBeforeDate
		{
			get
			{
				return _modifiedBeforeDate;
			}
			set
			{
				if (_modifiedBeforeDate != value)
				{
					_modifiedBeforeDate = value;
					OnPropertyChanged("ModifiedBeforeDate");
				}
			}
		}

		private DateTime? _modifiedAfterDate;

		public DateTime? ModifiedAfterDate
		{
			get
			{
				return _modifiedAfterDate;
			}
			set
			{
				if (_modifiedAfterDate != value)
				{
					_modifiedAfterDate = value;
					OnPropertyChanged("ModifiedAfterDate");
				}
			}
		}

		private DateTime? _lastAccessedBeforeDate;

		public DateTime? LastAccessedBeforeDate
		{
			get
			{
				return _lastAccessedBeforeDate;
			}
			set
			{
				if (_lastAccessedBeforeDate != value)
				{
					_lastAccessedBeforeDate = value;
					OnPropertyChanged("LastAccessedBeforeDate");
				}
			}
		}

		private DateTime? _lastAccessedAfterDate;

		public DateTime? LastAccessedAfterDate
		{
			get
			{
				return _lastAccessedAfterDate;
			}

			set
			{
				if (_lastAccessedAfterDate != value)
				{
					_lastAccessedAfterDate = value;
					OnPropertyChanged("LastAccessedAfterDate");
				}
			}
		}

		private DateTime? _createdBeforeDate;

		public DateTime? CreatedBeforeDate
		{
			get
			{
				return _createdBeforeDate;
			}
			set
			{
				if (_createdBeforeDate != value)
				{
					_createdBeforeDate = value;
					OnPropertyChanged("CreatedBeforeDate");
				}
			}
		}

		private DateTime? _createdAfterDate;

		public DateTime? CreatedAfterDate
		{
			get
			{
				return _createdAfterDate;
			}

			set
			{
				if (_createdAfterDate != value)
				{
					_createdAfterDate = value;
					OnPropertyChanged("CreatedAfterDate");
				}
			}
		}

		public DelegateCommand SearchCommand
		{
			get;
			private set;
		}

		public DelegateCommand CancelCommand
		{
			get;
			private set;
		}

		public DelegateCommand<FoundFile> UpdateFoundTextLinesCommand
		{
			get;
			private set;
		}

		public DelegateCommand<FoundFile> OpenSelectedFileCommand
		{
			get;
			private set;
		}

		public DelegateCommand<FoundTextLine> CopyToClipboardCommand
		{
			get;
			private set;
		}

		public DelegateCommand<FoundFile> OpenContainingFolderCommand
		{
			get;
			private set;
		}

		private void _SetDefaultFromSettings()
		{
			SearchPath = TextFinderSettings.Default.FilePath;
			SearchText = TextFinderSettings.Default.SearchText;
			IncludeFiles = TextFinderSettings.Default.IncludeFiles;
			ExcludeFiles = TextFinderSettings.Default.ExcludeFiles;
			MatchSearchTextCase = TextFinderSettings.Default.MatchSearchTextCase;
			SearchSubdirectories = TextFinderSettings.Default.SearchSubdirectories;
			CheckNumberOfLinesBetweenSearchTextEntries = TextFinderSettings.Default.CheckNumberOfLinesBetweenSearchTextEntries;
			LinesBetweenSearchText = CheckNumberOfLinesBetweenSearchTextEntries
				? (int?)TextFinderSettings.Default.LinesBetweenSearchText
				: null;

			ModifiedBeforeDate = TextFinderSettings.Default.ModifiedBeforeDate != DateTime.MinValue
				? (DateTime?)TextFinderSettings.Default.ModifiedBeforeDate
				: null;
			ModifiedAfterDate = TextFinderSettings.Default.ModifiedAfterDate != DateTime.MinValue
				? (DateTime?)TextFinderSettings.Default.ModifiedAfterDate
				: null;

			CreatedBeforeDate = TextFinderSettings.Default.CreatedBeforeDate != DateTime.MinValue
				? (DateTime?)TextFinderSettings.Default.CreatedBeforeDate
				: null;
			CreatedAfterDate = TextFinderSettings.Default.CreatedAfterDate != DateTime.MinValue
				? (DateTime?)TextFinderSettings.Default.CreatedAfterDate
				: null;

			LastAccessedBeforeDate = TextFinderSettings.Default.LastAccessedBeforeDate != DateTime.MinValue
				? (DateTime?)TextFinderSettings.Default.LastAccessedBeforeDate
				: null;
			LastAccessedAfterDate = TextFinderSettings.Default.LastAccessedAfterDate != DateTime.MinValue
				? (DateTime?)TextFinderSettings.Default.LastAccessedAfterDate
				: null;
		}

		private void _UpdateSuggestedSearchPaths()
		{
			var suggestedFilePaths = FileSearcher.SuggestFilePaths(SearchPath);

			SuggestedSearchPaths = new ObservableCollection<string>(suggestedFilePaths);
		}

		private ObservableCollection<FoundFile> mFoundFiles;

		public ObservableCollection<FoundFile> FoundFiles
		{
			get
			{
				return mFoundFiles;
			}
			set
			{
				mFoundFiles = value;
				OnPropertyChanged("FoundFiles");
			}
		}

		private ObservableCollection<string> _suggestedSearchPaths;

		public ObservableCollection<string> SuggestedSearchPaths
		{
			get
			{
				return _suggestedSearchPaths;
			}

			set
			{
				if (_suggestedSearchPaths != value)
				{
					_suggestedSearchPaths = value;
					OnPropertyChanged("SuggestedSearchPaths");
				}
			}
		}

		private ObservableCollection<FoundTextLine> mFoundTextLines;

		public ObservableCollection<FoundTextLine> FoundTextLines
		{
			get
			{
				return mFoundTextLines;
			}
			set
			{
				mFoundTextLines = value;
				OnPropertyChanged("FoundTextLines");
			}
		}

		public string Error => throw new NotImplementedException();

		public string this[string columnName]
		{
			get
			{
				if (columnName == "LinesBetweenSearchText")
				{
					if (CheckNumberOfLinesBetweenSearchTextEntries && LinesBetweenSearchText == null)
					{
						return "Number of lines between search text is required.";
					}
				}
				return string.Empty;
			}
		}

		void worker_Search(object sender, DoWorkEventArgs e)
		{
			for (int i = 0; i <= 100; i++)
			{
				if (_backgroundWorker.CancellationPending == true)
				{
					e.Cancel = true;
					return;
				}
				_backgroundWorker.ReportProgress(i);
				System.Threading.Thread.Sleep(250);
			}
			e.Result = 42;
		}

		void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			//lblStatus.Text = "Working... (" + e.ProgressPercentage + "%)";
			var thing = 1;
		}

		void worker_SearchCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Cancelled)
			{
				//lblStatus.Foreground = Brushes.Red;
				//lblStatus.Text = "Cancelled by user...";
				var thing = 1;
			}
			else
			{
				//lblStatus.Foreground = Brushes.Green;
				//lblStatus.Text = "Done... Calc result: " + e.Result;
				var thing = 1;
			}
		}

		private void _Search()
		{
			_backgroundWorker.RunWorkerAsync();

			IsSearching = true;

			FoundFiles.Clear();

			var datefilterCriteria = new DateFilterCriteria
			{
				CreatedAfterDate = CreatedAfterDate,
				CreatedBeforeDate = CreatedBeforeDate,
				LastAccessedAfterDate = LastAccessedAfterDate,
				LastAccessedBeforeDate = LastAccessedBeforeDate,
				ModifiedAfterDate = ModifiedAfterDate,
				ModifiedBeforeDate = ModifiedBeforeDate
			};

			FoundFiles = new ObservableCollection<FoundFile>(FileSearcher.FindText(SearchText,
				SearchPath,
				IncludeFiles,
				ExcludeFiles,
				MatchSearchTextCase,
				SearchSubdirectories,
				CheckNumberOfLinesBetweenSearchTextEntries,
				LinesBetweenSearchText,
				datefilterCriteria));

			if (FoundFiles.Count == 0)
			{
				FoundTextLines = new ObservableCollection<FoundTextLine>();
			}

			_SaveSettings();

			IsSearching = false;
		}

		private void _Cancel()
		{
			_backgroundWorker.CancelAsync();
		}

		private void _SaveSettings()
		{
			TextFinderSettings.Default.FilePath = SearchPath;
			TextFinderSettings.Default.SearchText = SearchText;
			TextFinderSettings.Default.ExcludeFiles = ExcludeFiles;
			TextFinderSettings.Default.IncludeFiles = IncludeFiles;
			TextFinderSettings.Default.MatchSearchTextCase = MatchSearchTextCase;
			TextFinderSettings.Default.SearchSubdirectories = SearchSubdirectories;
			TextFinderSettings.Default.CheckNumberOfLinesBetweenSearchTextEntries = CheckNumberOfLinesBetweenSearchTextEntries;
			TextFinderSettings.Default.LinesBetweenSearchText = LinesBetweenSearchText ?? 0;

			TextFinderSettings.Default.CreatedBeforeDate = CreatedBeforeDate ?? DateTime.MinValue;
			TextFinderSettings.Default.CreatedAfterDate = CreatedAfterDate ?? DateTime.MinValue;
			TextFinderSettings.Default.ModifiedBeforeDate = ModifiedBeforeDate ?? DateTime.MinValue;
			TextFinderSettings.Default.ModifiedAfterDate = ModifiedAfterDate ?? DateTime.MinValue;
			TextFinderSettings.Default.LastAccessedBeforeDate = LastAccessedBeforeDate ?? DateTime.MinValue;
			TextFinderSettings.Default.LastAccessedAfterDate = LastAccessedAfterDate ?? DateTime.MinValue;

			TextFinderSettings.Default.Save();
		}

		private void _UpdateFoundTextLines(FoundFile selectedFile)
		{
			if (selectedFile != null)
			{
				FoundTextLines = new ObservableCollection<FoundTextLine>(selectedFile.FoundTextLines);
			}
		}

		private void _OpenSelectedFile(FoundFile selectedFile)
		{
			if (selectedFile != null)
			{
				Process.Start(selectedFile.FilePath);
			}
		}

		private void _CopyToClipboard(FoundTextLine foundTextLine)
		{
			if (foundTextLine != null)
			{
				Clipboard.SetText(foundTextLine.Line);
			}
		}

		private void _OpenContainingFolder(FoundFile foundFile)
		{
			if (foundFile != null)
			{
				Process.Start("explorer.exe", "/select," + foundFile.FilePath);
			}
		}
	}
}