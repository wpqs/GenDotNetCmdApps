namespace KLineEdCmdApp.Utils
{
    public class CursorPosition
    {
        public CursorPosition( CursorPosition cursor) : this(cursor?.RowIndex ?? Program.PosIntegerNotSet, cursor?.ColIndex ?? Program.PosIntegerNotSet) { }

        public CursorPosition(int rowIndex, int colIndex)
        {
            RowIndex = rowIndex;
            ColIndex = colIndex;
        }
        public int RowIndex { get; set; }
        public int ColIndex { get; set; }

        public bool IsSame(CursorPosition cursor)
        {
            return ((cursor != null) && (cursor.RowIndex == RowIndex) && (cursor.ColIndex == ColIndex));
        }

        public bool IsSame(int checkRowIndex, int checkColIndex)
        {
            return ((checkRowIndex == RowIndex) && (checkColIndex == ColIndex));
        }

        public bool IsValid(int lineCount, int maxColIndex)
        {
            return ((RowIndex >= 0) && (((lineCount > 0) && (RowIndex < lineCount)) || ((lineCount == 0) && (RowIndex == 0))) && (ColIndex >= 0) && (ColIndex <= maxColIndex));
        }

        public bool IsValid(int lineCount)
        {
            return ((RowIndex >= 0) && (((lineCount > 0) && (RowIndex < lineCount)) || ((lineCount == 0) && (RowIndex == 0))) && (ColIndex >= 0));
        }

        public CursorPosition Copy()
        {
            return (CursorPosition) this.MemberwiseClone();
        }

        public override string ToString()
        {
            return $"rowIndex={RowIndex}; colIndex={ColIndex}";
        }
    }
}
