using System;

namespace TextFinder.Model
{
	public class FoundFile : ViewModelBase
	{
		private string _fileName;

		public string FileName
		{
			get
			{
				return _fileName;
			}
			set
			{
				if (_fileName != value)
				{
					_fileName = value;
					OnPropertyChanged("FileName");
				}
			}
		}

		private string _filePath;

		public string FilePath
		{
			get
			{
				return _filePath;
			}
			set
			{
				if (_filePath != value)
				{
					_filePath = value;
					OnPropertyChanged("FilePath");
				}
			}
		}

		private string _fileType;

		public string FileType
		{
			get
			{
				return _fileType;
			}
			set
			{
				if (_fileType != value)
				{
					_fileType = value;
					OnPropertyChanged("FileType");
				}
			}
		}

		private DateTime _lastAccessedDate;

		public DateTime LastAccessedDate
		{
			get
			{
				return _lastAccessedDate;
			}

			set
			{
				if (_lastAccessedDate != value)
				{
					_lastAccessedDate = value;
					OnPropertyChanged("LastAccessedDate");
				}
			}
		}

		private DateTime _lastModifiedDate;

		public DateTime LastModifiedDate
		{
			get
			{
				return _lastModifiedDate;
			}

			set
			{
				if (_lastModifiedDate != value)
				{
					_lastModifiedDate = value;
					OnPropertyChanged("LastModifiedDate");
				}
			}
		}

		private DateTime _createdOnDate;

		public DateTime CreatedOnDate
		{
			get
			{
				return _createdOnDate;
			}

			set
			{
				if (_createdOnDate != value)
				{
					_createdOnDate = value;
					OnPropertyChanged("CreatedOnDate");
				}
			}
		}

		private FoundTextLine[] _foundTextLines;

		public FoundTextLine[] FoundTextLines
		{
			get
			{
				return _foundTextLines;
			}

			set
			{
				if (_foundTextLines != value)
				{
					_foundTextLines = value;
					OnPropertyChanged("FoundTextLines");
				}
			}
		}
	}
}