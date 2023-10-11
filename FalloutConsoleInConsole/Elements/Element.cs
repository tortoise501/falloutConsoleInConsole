public abstract class Element
{
  public ElementType elementType { get; set; }
  public char value { get; set; }
  public int index { get; protected set; }
  // public Coordinates coordinates;
  public Element(char value, int index)
  {
    // this.coordinates = coordinates;
    this.value = value;
    this.index = index;
  }
  public override string ToString()
  {
    return value.ToString();
  }
}