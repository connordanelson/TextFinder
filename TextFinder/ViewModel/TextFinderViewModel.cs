using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Data;
using TextFinder.Model;
using TextFinder.Settings;

namespace TextFinder.ViewModel
{
	public class TextFinderViewModel : ViewModelBase, IDataErrorInfo
	{
		private BackgroundWorker _backgroundWorker = null;
		private string _sortColumn;
		private ListSortDirection _sortDirection;

		public TextFinderViewModel()
		{
			FoundFiles = new ObservableCollection<FoundFile>();
			FoundTextLines = new ObservableCollection<FoundTextLine>();
			SearchCommand = new DelegateCommand(_Search);
			SortColumnCommand = new DelegateCommand<string>(_SortColumn);
			CancelCommand = new DelegateCommand(_Cancel);
			UpdateFoundTextLinesCommand = new DelegateCommand<FoundFile>(_UpdateFoundTextLines);
			SuggestedSearchPaths = new ObservableCollection<string>();
			OpenSelectedFileCommand = new DelegateCommand<FoundFile>(_OpenSelectedFile);
			CopyToClipboardCommand = new DelegateCommand<FoundTextLine>(_CopyToClipboard);
			OpenContainingFolderCommand = new DelegateCommand<FoundFile>(_OpenContainingFolder);

			_SetupBackGroundWorker();
			_SetDefaultFromSettings();

			FoundTextLinesMessage = "No items to display";
		}

		public DelegateCommand CancelCommand
		{
			get;
			private set;
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

		public DelegateCommand<FoundTextLine> CopyToClipboardCommand
		{
			get;
			private set;
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

		public string Error => throw new NotImplementedException();

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

		private ObservableCollection<FoundFile> _foundFiles;
		public ObservableCollection<FoundFile> FoundFiles
		{
			get
			{
				return _foundFiles;
			}
			set
			{
				_foundFiles = value;
				_foundFilesViewSource = new CollectionViewSource();
				_foundFilesViewSource.Source = _foundFiles;
				OnPropertyChanged("FoundFiles");
			}
		}

		private CollectionViewSource _foundFilesViewSource;
		public CollectionViewSource FoundFilesViewSource
		{
			get
			{
				return _foundFilesViewSource;
			}
			set
			{
				_foundFilesViewSource = value;
				OnPropertyChanged("FoundFilesViewSource");
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

		private string _foundTextLinesMessage;
		public string FoundTextLinesMessage
		{
			get
			{
				return _foundTextLinesMessage;
			}
			set
			{
				if (_foundTextLinesMessage != value)
				{
					_foundTextLinesMessage = value;
					OnPropertyChanged("FoundTextLinesMessage");
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

		public DelegateCommand<FoundFile> OpenContainingFolderCommand
		{
			get;
			private set;
		}

		public DelegateCommand<FoundFile> OpenSelectedFileCommand
		{
			get;
			private set;
		}

		public DelegateCommand SearchCommand
		{
			get;
			private set;
		}

		public DelegateCommand<string> SortColumnCommand
		{
			get;
			private set;
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

		private int _searchProgress;
		public int SearchProgress
		{
			get
			{
				return _searchProgress;
			}
			set
			{
				if (_searchProgress != value)
				{
					_searchProgress = value;
					OnPropertyChanged("SearchProgress");
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

		private FoundFile _selectedFoundFile;
		public FoundFile SelectedFoundFile
		{
			get
			{
				return _selectedFoundFile;
			}
			set
			{
				if (_selectedFoundFile != value)
				{
					_selectedFoundFile = value;
					OnPropertyChanged("SelectedFoundFile");
				}
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

		public DelegateCommand<FoundFile> UpdateFoundTextLinesCommand
		{
			get;
			private set;
		}

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

		private void _Cancel()
		{
			_backgroundWorker.CancelAsync();
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

		private void _OpenSelectedFile(FoundFile selectedFile)
		{
			if (selectedFile != null)
			{
				Process.Start(selectedFile.FilePath);
			}
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

		private void _Search()
		{
			FoundFiles.Clear();
			FoundTextLines.Clear();
			FoundTextLinesMessage = "Searching";
			_backgroundWorker.RunWorkerAsync();
		}

		private void _SortColumn(string column)
		{
			FoundFilesViewSource.SortDescriptions.Clear();

			if (column == _sortColumn)
			{
				_sortDirection = _sortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
			}
			else
			{
				_sortDirection = ListSortDirection.Ascending;
				_sortColumn = column;
			}

			FoundFilesViewSource.SortDescriptions.Add(new SortDescription
			{
				Direction = _sortDirection,
				PropertyName = _sortColumn
			});
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

		private void _SetupBackGroundWorker()
		{
			_backgroundWorker = new BackgroundWorker();
			_backgroundWorker.WorkerSupportsCancellation = true;
			_backgroundWorker.WorkerReportsProgress = true;
			_backgroundWorker.DoWork += worker_Search;
			_backgroundWorker.ProgressChanged += worker_ProgressChanged;
			_backgroundWorker.RunWorkerCompleted += worker_SearchCompleted;
		}
		private void _UpdateFoundTextLines(FoundFile selectedFile)
		{
			if (selectedFile != null)
			{
				FoundTextLines = new ObservableCollection<FoundTextLine>(selectedFile.FoundTextLines);
			}
		}

		private void _UpdateSuggestedSearchPaths()
		{
			var suggestedFilePaths = FileSearcher.SuggestFilePaths(SearchPath);

			SuggestedSearchPaths = new ObservableCollection<string>(suggestedFilePaths);
		}
		private void worker_ProgressChanged(object sender, ProgressChangedEventArgs eventArgs)
		{
			SearchProgress = eventArgs.ProgressPercentage;
			if (eventArgs.UserState != null)
			{
				var foundFile = (FoundFile)eventArgs.UserState;
				FoundFiles.Add(foundFile);
			}
		}

		private void worker_Search(object sender, DoWorkEventArgs e)
		{
			var datefilterCriteria = new DateFilterCriteria
			{
				CreatedAfterDate = CreatedAfterDate,
				CreatedBeforeDate = CreatedBeforeDate,
				LastAccessedAfterDate = LastAccessedAfterDate,
				LastAccessedBeforeDate = LastAccessedBeforeDate,
				ModifiedAfterDate = ModifiedAfterDate,
				ModifiedBeforeDate = ModifiedBeforeDate
			};

			// Have file check to get rid of error if user tries to use fileName as a directory or an invalid directory name
			if (!File.Exists(SearchPath) && Directory.Exists(SearchPath))
			{
				var searchOption = SearchSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
				var filePaths = Directory.GetFiles(SearchPath, "*", searchOption);

				var fileCounter = 0;
				var totalFilesCount = filePaths.Length;

				foreach (var filePath in filePaths)
				{
					if (_backgroundWorker.CancellationPending == true)
					{
						e.Cancel = true;
						return;
					}

					var foundFile = FileSearcher.FindText(SearchText,
						filePath,
						IncludeFiles,
						ExcludeFiles,
						MatchSearchTextCase,
						CheckNumberOfLinesBetweenSearchTextEntries,
						LinesBetweenSearchText,
						datefilterCriteria);

					if (foundFile != null)
					{
						var progressPercentage = Convert.ToInt32(((double)fileCounter / totalFilesCount) * 100);
						(sender as BackgroundWorker).ReportProgress(progressPercentage, foundFile);
					}
					else if (fileCounter % 50 == 0)
					{
						var progressPercentage = Convert.ToInt32(((double)fileCounter / totalFilesCount) * 100);
						(sender as BackgroundWorker).ReportProgress(progressPercentage);
					}
					fileCounter++;
				}
			}
			else
			{
				MessageBox.Show("An invalid file path was entered. Please enter a new path", "Invalid file path", MessageBoxButton.OK);
			}
		}
		private void worker_SearchCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Cancelled)
			{
				SearchProgress = 0;
				FoundTextLinesMessage = "Search cancelled";
			}
			else
			{
				SearchProgress = 100;
				FoundTextLinesMessage = "Search complete";
				_SaveSettings();
			}
		}
	}
}