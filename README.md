# More Voiceover Options for Disco Elysium

Allows changing voiceover mode per-character. Want classic voiceover, but fully
voiced Kim? This mod lets you do it.

Also adds a "reverse psychological" voiceover mode, which enables full skills
VO, but not full NPC VO. In other words, Reverse Psychological VO = Classic VO
+ (Full VO - Psychological VO).

## Installation

* Install BepInEx.
* Download a release from the releases page and dump it into BepInEx/plugins.
* No automated installation method, I'm lazy. Pull requests welcome!

## Configuration

* Run the game with the mod once.
* Open file BepInEx/config/DEMoreVoiceoverOptions.cfg and edit it according to
  instructions.

## Limitations

* "Character" is loosely defined by the game. If there's e.g. a narrator line
  in overridden character's dialogue, voiceover for that line will be
  overridden as well. I don't know if it can be improved.
* This is an early mod version and very few characters are added for now.
  Configuration file describes how to add more overrides. Feel free to help me
  fill the list, or figure out a way to extract actor IDs automatically!
