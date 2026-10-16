# Building

## Requirements

- .NET 10 SDK (the `global.json` pins `10.0.401` with `latestFeature` roll-forward)
- Windows (the build references the Steam Among Us interop package)

## Steps

```bash
dotnet restore
dotnet build -c Release
```

Output: `src/MiraUnleashed/bin/Release/net6.0/MiraUnleashed.dll`

## Auto-copy to Among Us

Create `Directory.Build.local.props` at the repo root (it is git-ignored):

```xml
<Project>
  <PropertyGroup>
    <AmongUs>C:\Program Files (x86)\Steam\steamapps\common\Among Us</AmongUs>
  </PropertyGroup>
</Project>
```

Every build then copies `MiraUnleashed.dll` to `<AmongUs>\BepInEx\plugins\`.

## CI

`build.yml` compiles with `-p:ContinuousIntegrationBuild=true`, which enables warnings-as-errors.

## Building the experimental branch

The `experimental` branch references TOU-Mira from source instead of NuGet. Clone it next to this repository with its submodules:

```bash
cd ..
git clone https://github.com/AU-Avengers/TOU-Mira --branch experimental --recurse-submodules
```

so the layout is:

```text
repos/
  MiraUnleashed/
  TOU-Mira/
```

Then build as usual. If TOU-Mira lives elsewhere, point `TouMiraSourcePath` at it — either as an environment variable or in `Directory.Build.local.props`:

```xml
<Project>
  <PropertyGroup>
    <TouMiraSourcePath>D:\source\TOU-Mira\</TouMiraSourcePath>
  </PropertyGroup>
</Project>
```

### Runtime set

The game must run the `TownOfUsMira.dll`, `MiraAPI.dll` and `AchievementsAPI.dll` produced by the same TOU-Mira experimental build (found in `TOU-Mira\TownOfUs\bin\Release\` after building `TownOfUsMira.csproj`), plus the Reactor version pinned in TOU-Mira's `AmongUs.props`. Mixing CI artifacts from different pipelines or branches causes `TypeLoadException`s and crashes.
