public class Word : MasterElement
{
  public string word { get; private set; }
  public Word(char value, int index, string word, List<Element>? slaveElements = null) : base(value, index, slaveElements)
  {
    this.word = word;
    elementType = ElementType.Word;
  }
}