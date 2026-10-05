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
