# Contributing

Thanks for your interest in contributing to Mira Unleashed!

## Branch model

- `main` – release commits only, reached via release PRs.
- `dev` – open pull requests here. All normal development targets `dev`.
- `experimental` – tracks TOU-Mira's experimental branch and is rebased on `dev`.

Never force-push to `main`.

## Setup

1. Install the .NET 10 SDK (see `global.json`).
2. `dotnet restore` then `dotnet build -c Release`.
3. Optionally add `Directory.Build.local.props` with an `<AmongUs>` path to auto-copy the DLL into `BepInEx/plugins` on build.

## Coding conventions

- File-scoped namespaces, 4-space indentation; match the style of the file you touch.
- Namespace mirrors folder: `MiraUnleashed.Roles.<Team>`, `MiraUnleashed.Buttons.<Team>`, `MiraUnleashed.Events.<Team>`, `MiraUnleashed.Modules`, `MiraUnleashed.Modifiers`, `MiraUnleashed.Options.Roles.<Team>`, `MiraUnleashed.Options.Modifiers`, `MiraUnleashed.Patches.<Area>`.
- Roles implement `IMiraUnleashedRole` (and `IWikiDiscoverable` etc.). Do not override `RoleName`/description locale properties — MiraAPI resolves them from the ID prefix/part.
- Locale keys: `MiraUnleashed.Role.<IdPart>` (+ `IntroBlurb`/`TabDescription`/`WikiDescription` suffixes), abilities `MiraUnleashed.Role.<IdPart><Ability>`, options `MiraUnleashed.Options.<IdPart>.<Option>`, modifiers `MiraUnleashed.Modifier.<Name>`, misc `MiraUnleashed.<Area>.<Key>`. Add every new key to `Resources/Locale/en_US.xml` — duplicate `name` attributes are not allowed.
- Option attribute titles and enum value labels take raw locale keys.
- RPCs go in `Networking/MiraUnleashedRpc.cs` and use `[MethodRpc((uint)MiraUnleashedRpc.X)]`.
- Button instances are accessed via `CustomButtonSingleton<T>.Instance`, never static instance properties.
- No swallow-all `try { } catch { }` blocks. Catch only where a call genuinely throws in normal operation (e.g. reflection over another mod's internals) and log with `Warning(...)`.
- No `required` members or other features unsupported by net6.0.
- Commit messages are plain imperative sentences with no trailers.

## Adding a role: checklist

1. Role class under `Roles/<Team>/`, button(s) under `Buttons/<Team>/`, events under `Events/<Team>/`, options under `Options/Roles/<Team>/`.
2. All strings into `Resources/Locale/en_US.xml` using the key scheme above.
3. Any sprites/audio already embedded under `Resources/` referenced via `MiraUnleashedAssets`/`MiraUnleashedAudio`.
4. New RPC ids appended to `MiraUnleashedRpc`.
5. `dotnet build -c Release` with 0 warnings and 0 errors.
6. Docs page in `docs/` and a `CHANGELOG.md` entry.

## Wiki

`docs/` is mirrored to the GitHub Wiki by `wiki-sync.yml`. The repository's Wiki must be created once in repo settings before the first sync.
