public struct Coordinates
{
  public int x;
  public int y;
  public Coordinates(int x, int y)
  {
    this.x = x;
    this.y = y;
  }
  public static bool operator ==(Coordinates c1, Coordinates c2)
  {
    return c1.x == c2.x && c1.y == c2.y;
  }

  // Overload the != operator
  public static bool operator !=(Coordinates c1, Coordinates c2)
  {
    return !(c1 == c2);
  }
}