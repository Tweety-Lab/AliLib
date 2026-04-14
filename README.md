# AliLib
![Badge](https://img.shields.io/badge/Version-v0.3-orange)

**AliLib** Is a [Blade & Sorcery](https://store.steampowered.com/app/629730/Blade_and_Sorcery/) Modding Library designed to promote easier, more modular scripted mods. (specifically spells)

Want to use AliLib? Check out the [Wiki!](https://github.com/Tweety-Lab/AliLib/wiki/)

## AliLib.Analyzer
A Roslyn Analyzer that handles semantic checks aswell as provides some functionality for AliLib build events.

## AliLib.Core
The core library for AliLib, all of the non-build related content lives here.

## Cloning the Repo
Due to the fact AliLib uses Blade & Sorcery assemblies the best way to work with the project is to clone it into `Blade & Sorcery/BladeAndSorcery_Data/StreamingAssets`.

# Features
For a full list of features, check the [Wiki](https://github.com/Tweety-Lab/AliLib/wiki/Features).

## MSBuild Integration
```xml
<!-- Custom AliLib Properties -->
<PropertyGroup>
  <ModPath>$(ProjectDir)..\..\Mods\AliLibTest</ModPath>
</PropertyGroup>
```

## Easier Addressables
```csharp
[Addressable("ProjectionSorcery.GlassSpawn")]
public static AudioClip? MyClip { get; set; }
```

## Exported Strings
```csharp
[ExportedString("Test/MyText.txt")]
public const string MyAsset = "Hello, World!";
```

## Mod Events
```csharp
public ModEvent MyEvent {  get; set; } = new ModEvent();

/// <inheritdoc />
public override void ScriptUpdate()
{
  base.ScriptUpdate();

  MyEvent.Invoke();
  MyEvent.Cancelled = true;

  if (MyClip != null && Platform.IsPCVR)
  {
    Debug.Log($"[AliLib] {MyClip.length}");
  }
}
```
