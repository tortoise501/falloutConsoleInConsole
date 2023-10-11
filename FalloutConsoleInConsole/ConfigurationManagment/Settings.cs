public class Settings
{
  public Settings()
  {
    if (HintAmount % ColumnAmount != 0)
    {
      HintAmount = (HintAmount / ColumnAmount) * ColumnAmount;
    }
    if (WordAmount % ColumnAmount != 0)
    {
      WordAmount = (WordAmount / ColumnAmount) * ColumnAmount;
    }
  }
  public int ColumnWidth = 12;
  public int ColumnHeight = 16;

  public int AddressWidth = 6;
  public int AddressHeight = 16;

  public int WordAmount = 16;
  public int WordLength = 6;

  public int LoggerHeight = 16;

  public int MaxAttempts = 4;

  public int ColumnAmount = 2;
  public int HintAmount = 8;

  public ConsoleColor notSelectedBackground = ConsoleColor.Black;
  public ConsoleColor notSelectedChar = ConsoleColor.Green;

  public ConsoleColor selectedAsElementBackground = ConsoleColor.Green;
  public ConsoleColor selectedAsElementChar = ConsoleColor.Black;

  public ConsoleColor selectedAsCharBackground = ConsoleColor.Blue;
  public ConsoleColor selectedAsCharChar = ConsoleColor.Black;
}