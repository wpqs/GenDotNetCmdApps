namespace KLineEdCmdApp.Utils
{
    public class CursorPosition
    {
        public CursorPosition(int rowIndex, int colIndex)
        {
            RowIndex = rowIndex;
            ColIndex = colIndex;
        }
        public int RowIndex { get; set; }
        public int ColIndex { get; set; }
    }
}
