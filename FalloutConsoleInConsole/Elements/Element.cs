public abstract class Element
{
  public ElementType elementType { get; set; }
  public char value { get; set; } = ' ';
  public Coordinates coordinates;
  public Element(char value, Coordinates coordinates)
  {
    this.coordinates = coordinates;
    this.value = value;
  }
  public override string ToString()
  {
    return value.ToString();
  }
}