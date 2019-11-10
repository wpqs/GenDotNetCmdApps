namespace KLineEdCmdApp.Utils
{
    public class CursorPosition
    {
        public CursorPosition( CursorPosition cursor) : this(cursor.RowIndex, cursor.ColIndex) { }

        public CursorPosition(int rowIndex, int colIndex)
        {
            RowIndex = rowIndex;
            ColIndex = colIndex;
        }
        public int RowIndex { get; set; }
        public int ColIndex { get; set; }

        public bool IsSame(CursorPosition cursor)
        {
            return ((cursor.RowIndex == RowIndex) && (cursor.ColIndex == ColIndex)) ? true : false;
        }

        public CursorPosition Copy()
        {
            return (CursorPosition) this.MemberwiseClone();
        }

        public bool IsSame(int checkRowIndex, int checkColIndex)
        {
            return ((checkRowIndex == RowIndex) && (checkColIndex == ColIndex)) ? true : false;
        }
    }
}
