public class Word : MasterElement
{
  public string word { get; private set; }
  public Word(char value, Coordinates coordinates, string word, List<Element>? slaveElements = null) : base(value, coordinates, slaveElements)
  {
    this.word = word;
    elementType = ElementType.Word;
  }

}