using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;

static class Constants
{
  static public WordsPull wordsPull = new WordsPull();
  static public void LoadWordsPull(WordsPull pull)
  {
    wordsPull = pull;
  }


  static public Settings settings = new Settings();
  static public void LoadSettings(Settings settings)
  {
    Constants.settings = settings;
    UpdateColors();
  }
  static private void UpdateColors()
  {
    // CharStateToBackgroundColor[CharacterState.notSelected] = settings.notSelectedBackground;
    // CharStateToBackgroundColor[CharacterState.selectedAsElement] = settings.selectedAsElementBackground;
    // CharStateToBackgroundColor[CharacterState.selectedAsChar] = settings.selectedAsCharBackground;

    // CharStateToCharColor[CharacterState.notSelected] = settings.notSelectedChar;
    // CharStateToCharColor[CharacterState.selectedAsElement] = settings.selectedAsElementChar;
    // CharStateToCharColor[CharacterState.selectedAsChar] = settings.selectedAsCharChar;
    StateToBrush[CharacterState.notSelected] = new Brush(settings.notSelectedBackground, settings.notSelectedChar);//TODO: better settings integration
    StateToBrush[CharacterState.selectedAsElement] = new Brush(settings.selectedAsElementBackground, settings.selectedAsElementChar);
    StateToBrush[CharacterState.selectedAsChar] = new Brush(settings.selectedAsCharBackground, settings.selectedAsCharChar);
  }


  static public readonly char[] symbols = {
    '}',
    ']',
    ')',
    '#',
    '%',
    ',',
    '.',
    '/',
    '\\',
    '*'
  };


  static public readonly char[] Parentheses = new char[4]{
    '}',
    ')',
    '>',
    ']'
  };


  static public readonly Dictionary<char, char> GetOppositeParentheses = new Dictionary<char, char>()
  {
    {')','('},
    {'}','{'},
    {']','['},
    {'>','<'}
  };


  static public bool IsParentheses(char c)
  {
    if (c == '[' || c == ']' || c == '{' || c == '}' || c == '(' || c == ')' || c == '<' || c == '>')
      return true;
    else
      return false;
  }

  // static public Dictionary<CharacterState, ConsoleColor> CharStateToCharColor = new Dictionary<CharacterState, ConsoleColor>()
  // {
  //   {CharacterState.notSelected,ConsoleColor.Green},
  //   {CharacterState.selectedAsElement,ConsoleColor.Black},
  //   {CharacterState.selectedAsChar,ConsoleColor.Black}
  // };
  // static public Dictionary<CharacterState, ConsoleColor> CharStateToBackgroundColor = new Dictionary<CharacterState, ConsoleColor>()
  // {
  //   {CharacterState.notSelected,ConsoleColor.Black},
  //   {CharacterState.selectedAsElement,ConsoleColor.Green},
  //   {CharacterState.selectedAsChar,ConsoleColor.Blue}
  // };
  static public Dictionary<CharacterState, Brush> StateToBrush = new Dictionary<CharacterState, Brush>()
  {
    {CharacterState.notSelected,new Brush(ConsoleColor.Black,ConsoleColor.Green)},
    {CharacterState.selectedAsElement,new Brush(ConsoleColor.Green,ConsoleColor.Black)},
    {CharacterState.selectedAsChar,new Brush(ConsoleColor.Blue,ConsoleColor.Black)}
  };
}
public enum CharacterState
{
  notSelected,
  selectedAsElement,
  selectedAsChar
}
public enum ExecutionCode
{
  WrongInput,
  Mistake,
  CorrectWord,
  HintLife,
  HintDuds
}
public enum GameState
{
  InProgress,
  Lost,
  Won
}
public enum HintType
{
  Dud,
  Attempt
}
public enum ElementType
{
  Symbol,
  Word,
  Hint
}