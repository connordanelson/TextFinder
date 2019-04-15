using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TextFinder.CustomExtensions;
using TextFinder.Model;

namespace TextFinder
{
	public static class FileSearcher
	{
		public static FoundFile FindText(string searchText,
			string filePath,
			string includeFiles,
			string excludeFiles,
			bool matchSearchTextCase,
			bool checkNumberOfLinesBetweenSearchTextEntries,
			int? linesBetweenSearchText,
			DateFilterCriteria dateFilterCriteria)
		{
			FoundFile foundFile = null;

			var fileLines = File.ReadAllLines(filePath);
			FileInfo fileInfo = new FileInfo(filePath);

			var searchLines = true;

			if (!string.IsNullOrEmpty(includeFiles))
			{
				searchLines = _DetermineIfMatchesIncludeFilesFilter(includeFiles, fileInfo);
			}
			if (searchLines && !string.IsNullOrEmpty(excludeFiles))
			{
				searchLines = _DetermineIfMatchesExcludeFilesFilter(excludeFiles, fileInfo);
			}
			if (searchLines)
			{
				searchLines = _DetermineIfMatchesDateFilter(fileInfo, dateFilterCriteria);
			}

			List<FoundTextLine> foundTextLines = new List<FoundTextLine>();

			if (checkNumberOfLinesBetweenSearchTextEntries && linesBetweenSearchText.HasValue)
			{
				foundTextLines = _FindTextLinesWithLinesBetweenSearchText(fileLines, searchText, searchLines, matchSearchTextCase, linesBetweenSearchText.Value);
			}
			else
			{
				foundTextLines = _FindTextLines(fileLines, searchText, searchLines, matchSearchTextCase);
			}

			if (foundTextLines.Count > 0)
			{
				foundFile = new FoundFile
				{
					FileName = fileInfo.Name,
					FilePath = filePath,
					FileType = fileInfo.Extension,
					CreatedOnDate = fileInfo.CreationTime,
					LastAccessedDate = fileInfo.LastAccessTime,
					LastModifiedDate = fileInfo.LastWriteTime,
					FoundTextLines = foundTextLines.ToArray()
				};
			}

			return foundFile;
		}

		public static List<string> SuggestFilePaths(string filePath)
		{
			string directoryName = Path.GetDirectoryName(filePath);

			var suggestedFilePaths = new List<string>();
			// Have file check to get rid of error if user tries to use fileName as a directory
			if (File.Exists(directoryName))
			{
				suggestedFilePaths.Add(directoryName);
			}
			else if (Directory.Exists(Path.GetDirectoryName(directoryName)))
			{
				string[] files = Directory.GetFiles(directoryName, "*.*", SearchOption.TopDirectoryOnly);
				string[] directories = Directory.GetDirectories(directoryName, "*.*", SearchOption.TopDirectoryOnly);

				var paths = directories.Concat(files).ToArray();

				foreach (var path in paths)
				{
					if (path.StartsWith(directoryName, StringComparison.CurrentCultureIgnoreCase))
					{
						suggestedFilePaths.Add(path);
					}
				}
			}

			return suggestedFilePaths;
		}
		private static bool _DetermineIfMatchesCreatedDate(FileInfo fileInfo, DateTime? createdBeforeDate, DateTime? createdAfterDate)
		{
			bool matchesCreatedDateFilter = true;

			if (createdBeforeDate.HasValue)
			{
				if (createdBeforeDate.Value < fileInfo.CreationTime.Date)
				{
					matchesCreatedDateFilter = false;
				}
			}
			if (matchesCreatedDateFilter && createdAfterDate.HasValue)
			{
				if (createdAfterDate.Value > fileInfo.CreationTime.Date)
				{
					matchesCreatedDateFilter = false;
				}
			}

			return matchesCreatedDateFilter;
		}

		private static bool _DetermineIfMatchesDateFilter(FileInfo fileInfo, DateFilterCriteria dateFilterCriteria)
		{
			bool matchesDateFilter;

			matchesDateFilter = _DetermineIfMatchesCreatedDate(fileInfo, dateFilterCriteria.CreatedBeforeDate, dateFilterCriteria.CreatedAfterDate);

			if (matchesDateFilter)
			{
				matchesDateFilter = _DetermineIfMatchesLastAccessedDate(fileInfo, dateFilterCriteria.LastAccessedBeforeDate, dateFilterCriteria.LastAccessedAfterDate);
			}
			if (matchesDateFilter)
			{
				matchesDateFilter = _DetermineIfMatchesModifiedDate(fileInfo, dateFilterCriteria.ModifiedBeforeDate, dateFilterCriteria.ModifiedAfterDate);
			}

			return matchesDateFilter;
		}

		private static bool _DetermineIfMatchesExcludeFilesFilter(string excludeFiles, FileInfo fileInfo)
		{
			var searchLines = true;
			var excludeFilesDelimiters = excludeFiles.Split(';');
			foreach (var delimiter in excludeFilesDelimiters)
			{
				if (fileInfo.Name.Contains(delimiter, StringComparison.InvariantCultureIgnoreCase))
				{
					searchLines = false;
					break;
				}
			}

			return searchLines;
		}

		private static bool _DetermineIfMatchesIncludeFilesFilter(string includeFiles, FileInfo fileInfo)
		{
			var searchLines = true;
			var includeFilesDelimiters = includeFiles.Split(';');
			foreach (var delimiter in includeFilesDelimiters)
			{
				if (!fileInfo.Name.Contains(delimiter, StringComparison.InvariantCultureIgnoreCase))
				{
					searchLines = false;
					break;
				}
			}

			return searchLines;
		}

		private static bool _DetermineIfMatchesLastAccessedDate(FileInfo fileInfo, DateTime? lastAccessedBeforeDate, DateTime? lastAccessedAfterDate)
		{
			bool matchesLastAccessedDateFilter = true;

			if (lastAccessedBeforeDate.HasValue)
			{
				if (lastAccessedBeforeDate.Value < fileInfo.LastAccessTime.Date)
				{
					matchesLastAccessedDateFilter = false;
				}
			}
			if (matchesLastAccessedDateFilter && lastAccessedAfterDate.HasValue)
			{
				if (lastAccessedAfterDate.Value > fileInfo.LastAccessTime.Date)
				{
					matchesLastAccessedDateFilter = false;
				}
			}

			return matchesLastAccessedDateFilter;
		}

		private static bool _DetermineIfMatchesModifiedDate(FileInfo fileInfo, DateTime? modifiedBeforeDate, DateTime? modifiedAfterDate)
		{
			bool matchesModifiedDateFilter = true;

			if (modifiedBeforeDate.HasValue)
			{
				if (modifiedBeforeDate.Value < fileInfo.LastWriteTime.Date)
				{
					matchesModifiedDateFilter = false;
				}
			}
			if (matchesModifiedDateFilter && modifiedAfterDate.HasValue)
			{
				if (modifiedAfterDate.Value > fileInfo.LastWriteTime.Date)
				{
					matchesModifiedDateFilter = false;
				}
			}

			return matchesModifiedDateFilter;
		}

		private static List<FoundTextLine> _FindTextLines(string[] fileLines,
			string searchText,
			bool searchLines,
			bool matchSearchTextCase)
		{
			var foundTextLines = new HashSet<FoundTextLine>();
			if (searchLines)
			{
				var searchTextDelimiters = searchText.Split(';');
				var stringComparison = matchSearchTextCase ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;

				//Dictionary to keep track of which search words were found
				Dictionary<string, bool> searchTextFound = searchTextDelimiters.ToDictionary(delimiter => delimiter, matchFound => false);
				for (int lineIndex = 0; lineIndex < fileLines.Count(); lineIndex++)
				{
					var line = fileLines[lineIndex];

					foreach (var delimiter in searchTextDelimiters)
					{
						if (line.Contains(delimiter, stringComparison))
						{
							searchTextFound[delimiter] = true;
							foundTextLines.Add(new FoundTextLine
							{
								Line = line,
								LineNumber = lineIndex + 1
							});
						}
					}
				}
				if (!searchTextFound.All(dict => dict.Value))
				{
					foundTextLines.Clear();
				}
			}

			return foundTextLines.ToList();
		}

		private static List<FoundTextLine> _FindTextLinesWithLinesBetweenSearchText(string[] fileLines,
																	string searchText,
			bool searchLines,
			bool matchSearchTextCase,
			int linesBetweenSearchText)
		{
			var foundTextLines = new HashSet<FoundTextLine>();
			if (searchLines)
			{
				var searchTextDelimiters = searchText.Split(';');
				var stringComparison = matchSearchTextCase ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;

				for (int lineIndex = 0; lineIndex < fileLines.Count() - linesBetweenSearchText; lineIndex++)
				{
					var line = fileLines[lineIndex];
					HashSet<FoundTextLine> foundTextLinesToAdd = new HashSet<FoundTextLine>();
					//Dictionary to keep track of which search words were found
					Dictionary<string, bool> searchTextFound = searchTextDelimiters.ToDictionary(delim => delim, matchFound => false);
					foreach (var delimiter in searchTextDelimiters)
					{
						if (line.Contains(delimiter, stringComparison))
						{
							searchTextFound[delimiter] = true;
							foundTextLinesToAdd.Add(new FoundTextLine
							{
								Line = line,
								LineNumber = lineIndex + 1
							});

							//Search for all delimiters within the user specified number of lines between search text
							for (int linesBetweenSearchTextIndex = lineIndex + 1; linesBetweenSearchTextIndex <= linesBetweenSearchText; linesBetweenSearchTextIndex++)
							{
								var linesBetweenTextLine = fileLines[lineIndex + linesBetweenSearchTextIndex];
								foreach (var linesBetweenTextDelimiter in searchTextDelimiters)
								{
									if (linesBetweenTextLine.Contains(linesBetweenTextDelimiter, stringComparison))
									{
										foundTextLinesToAdd.Add(new FoundTextLine
										{
											Line = linesBetweenTextLine,
											LineNumber = lineIndex + linesBetweenSearchTextIndex + 1
										});
										searchTextFound[linesBetweenTextDelimiter] = true;
									}
								}
							}
						}
					}
					if (searchTextFound.All(dict => dict.Value))
					{
						foundTextLines.UnionWith(foundTextLinesToAdd);
					}
				}
			}

			return foundTextLines.ToList();
		}
	}
}