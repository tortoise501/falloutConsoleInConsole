public class Word : MasterElement
{
  public string word { get; private set; }
  public Word(char value, int index, int startPos, string word, List<Element> slaveElements) : base(value, index, startPos, slaveElements)
  {
    this.word = word;
    elementType = ElementType.Hint;
  }
}