
using System.Reflection.PortableExecutable;
using System.Xml.Serialization;
public class XMLManager
{
  static WordsPull wordsPull = new WordsPull();
  static string pathToWordsPull = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wordspull.xml");

  static Settings settings = new Settings();
  static string pathToSettings = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.xml");


  public XMLManager()
  {
    if (!File.Exists(pathToWordsPull))
    {
      CreateWordsPullXml();
    }
    else
    {
      ReadWordsPullXml();
    }
    Constants.LoadWordsPull(wordsPull);


    if (!File.Exists(pathToSettings))
    {
      CreateSettingXml();
    }
    else
    {
      ReadSettingXml();
    }
    Constants.LoadSettings(settings);
  }

  private static void CreateWordsPullXml()
  {
    var xmlSerializer = new XmlSerializer(typeof(WordsPull));
    using (var writer = new StreamWriter(pathToWordsPull))
    {
      xmlSerializer.Serialize(writer, wordsPull);
    }
  }
  private static void ReadWordsPullXml()
  {
    var xmlSerializer = new XmlSerializer(typeof(WordsPull));
    using (StreamReader reader = new StreamReader(pathToWordsPull))
    {
      wordsPull = (WordsPull)xmlSerializer.Deserialize(reader);
    }
  }


  private static void CreateSettingXml()
  {
    var xmlSerializer = new XmlSerializer(typeof(Settings));
    using (var writer = new StreamWriter(pathToSettings))
    {
      xmlSerializer.Serialize(writer, settings);
    }
  }
  private static void ReadSettingXml()
  {
    var xmlSerializer = new XmlSerializer(typeof(Settings));
    using (StreamReader reader = new StreamReader(pathToSettings))
    {
      settings = (Settings)xmlSerializer.Deserialize(reader);
    }
  }
}