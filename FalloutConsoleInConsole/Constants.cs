using System;
using System.Linq;
using System.Collections.Generic;

static class Constants
{
  static public readonly string[][] WordsPull = new string[5][]
  {
    new string[]
    {
        "book", "desk", "lamp", "moon", "star", "tree", "bird", "fish",
        "frog", "ball", "door", "rock", "bell", "fire", "food", "kite",
        "lake", "leaf", "milk", "nest"
    },
    new string[]
    {
        "apple", "chair", "horse", "mouse", "snake", "table", "house", "train",
        "beach", "clock", "glove", "knife", "cloud", "shoes", "socks", "pearl",
        "flute", "ghost", "leafy", "light"
    },
    new string[]
    {
        "pencil", "turtle", "rabbit", "rocket", "candle", "guitar", "switch", "flower",
        "pillow", "butter", "laptop", "window", "orange", "banana", "muffin", "router",
        "spider", "apples", "school", "lemons"
    },
    new string[]
    {
        "friends", "helmets", "tunnels", "blocker", "cottage", "diamond", "fortune", "trailer",
        "kitchen", "library", "monster", "package", "rolling", "rainbow", "whistle", "vampire",
        "whisper", "mailbox", "through", "manager"
    },
    new string[]
    {
        "capture", "dinosaur", "football", "generator", "microscope", "photograph", "revolution", "television",
        "university", "volunteer", "watermelon", "xylophone", "celebrate", "fascinate", "happiness", "integrate",
        "juxtapose", "legitimate", "overwhelm", "provocative"
    }
  };
  static public readonly char[] symbols = {
    // '{',
    '}',
    // '[',
    ']',
    // '(',
    ')',
    //'F',//'F' is a false parenthesis     'S' is start 'E' is end 
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

  static public Dictionary<CharacterState, ConsoleColor> CharStateToCharColor = new Dictionary<CharacterState, ConsoleColor>()
  {
    {CharacterState.notSelected,ConsoleColor.Green},
    {CharacterState.selectedAsElement,ConsoleColor.Black},
    {CharacterState.selectedAsChar,ConsoleColor.Black}
  };
  static public Dictionary<CharacterState, ConsoleColor> CharStateToBackgroundColor = new Dictionary<CharacterState, ConsoleColor>()
  {
    {CharacterState.notSelected,ConsoleColor.Black},
    {CharacterState.selectedAsElement,ConsoleColor.Green},
    {CharacterState.selectedAsChar,ConsoleColor.Blue}
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
