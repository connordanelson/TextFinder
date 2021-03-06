# TextFinder

Application to find files in a folder and containing subfolders if the user chooses, which contains text the user is searching for. 

To use, set the file path you wish to search into the "Search In" field and the text you are looking for into the "Search For" field. If 
you wish to search for your text in subdirectories check the "Subdirectories" checkbox. If you wish to search for multiple words or phrases then separate each word or phrase you are looking for with a semicolon, such as "word1;word2".

The are a few options to filter the search results. One is to filter by the file's creation date, modified date, and last accessed date in 
the "Dates" tab. Another is the "Include" and "Exclude" options in the "Filename" tab. There you can filter by text that must be in the
file's name, such as exluding .dll files or having it so the file name must include .cs. The search text can also be filtered by having
it match the case that you selected, or if you are searching for multiple words or phrases the number of lines between words or phrases
you are searching for. So if you are searching for "word1;word2" and only want results where they are on the same line you can set the
"# of lines between search entries" to 0 to accomplish this.

Once files containing the searched for text and matching any additional criteria are found, you can filter the results by selecting the column headers to switch between sorting the results in ascending and descending order. Once a search is completed, the criteria that was used will be saved for the next time the application is opened. If a search is cancelled, the saved settings will not be updated with the cancelled search criteria.
