# AliLib
**AliLib** Is a [Blade & Sorcery](https://store.steampowered.com/app/629730/Blade_and_Sorcery/) Modding Library designed to promote easier, more modular scripted mods.

## Cloning the Repo
Due to the fact AliLib uses Blade & Sorcery assemblies the best way to work with the project is to clone it into `Bade & Sorcery/BladeAndSorcery_Data/StreamingAssets`.

# Features
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
public ModEvent<object> MyEvent {  get; set; } = new ModEvent<object>();

/// <inheritdoc />
public override void ScriptUpdate()
{
  base.ScriptUpdate();

  MyEvent.Invoke(null);
  MyEvent.Cancelled = true;

  if (MyClip != null && Platform.IsPCVR)
  {
    Debug.Log($"[AliLib] {MyClip.length}");
  }
}
```
