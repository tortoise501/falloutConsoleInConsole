//TODO: "address" generation is completely random now, it shouldn't be random, implement more fallout like address generation 
public class Addresses : IRenderable
{
  List<List<RenderData>> addressesColumnData = new List<List<RenderData>>();
  public Addresses(int height, int width, int x = 0, int y = 0)
  {
    ((IRenderable)this).x = x;
    ((IRenderable)this).y = y;
    addressesColumnData = GenerateAddressesColumn(height, width);
  }

  List<List<RenderData>> GenerateAddressesColumn(int height, int width)
  {
    List<List<RenderData>> res = new List<List<RenderData>>();
    for (int i = 0; i < height; i++)
    {
      res.Add(new List<RenderData>());
      res.Last().Add(new RenderData('0'));
      res.Last().Add(new RenderData('x'));
      Random rnd = new Random();
      for (int j = 2; j < width; j++)
      {
        res.Last().Add(new RenderData(rnd.Next(0, 16).ToString("X")[0]));
      }
    }

    return res;
  }

  List<List<RenderData>> IRenderable.GetRenderData()
  {
    return addressesColumnData;
  }
  int IRenderable.x { get; set; }
  int IRenderable.y { get; set; }
}