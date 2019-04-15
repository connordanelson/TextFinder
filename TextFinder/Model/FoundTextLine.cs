namespace TextFinder.Model
{
	public class FoundTextLine
	{
		public string Line
		{
			get;
			set;
		}

		public int LineNumber
		{
			get;
			set;
		}

		public override bool Equals(object obj)
		{
			FoundTextLine foundTextLine = obj as FoundTextLine;
			return foundTextLine != null && foundTextLine.LineNumber == this.LineNumber && foundTextLine.Line == this.Line;
		}

		public override int GetHashCode()
		{
			return this.LineNumber.GetHashCode() ^ this.Line.GetHashCode();
		}
	}
}