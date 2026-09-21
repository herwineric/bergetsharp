# AGENTS.md

Unofficial .NET client for the Berget.AI inference API. Two projects in the solution: `BergetSharp` (client library) and `BergetSharp.Test` (TUnit).

## Commands

- Build: `dotnet build BergetSharp.sln`
- All tests: `dotnet test BergetSharp.sln`
- Single test class: `dotnet test BergetSharp.sln -- --treenode-filter "/*/*/RerankTest/*"` (MTP runner via `global.json`; plain `--filter` matches nothing)
- Targets `net10.0`; requires a .NET 10 SDK (older SDKs fail restore).
- CI: `.github/workflows/test.yml` (build + test on push/PR to main, needs `BERGET_API_KEY` secret), `.github/workflows/publish.yml` (packs and pushes to nuget.org via Trusted Publishing on GitHub Release), `.github/dependabot.yml` (weekly NuGet + GitHub Actions updates). Verification = build + test.

## Packaging

- `BergetSharp` packs to NuGet (`PackageId: BergetSharp`); bump `VersionPrefix` in `BergetSharp.csproj` and create a release tagged `v<version>` to publish.
- `GeneratePackageOnBuild` is set, so `dotnet pack` skips its implicit build — always `dotnet build` first (CI does; see NU5026 note in `publish.yml`).

## Tests hit the live API

`BergetSharp.Test` is an integration suite: tests construct a real `BergetClient` against `https://api.berget.ai/v1` (currently with an empty `ApiKeyCredential`, so they fail with 401/`ClientResultException` without valid service access). There is no mocking framework or test double. Don't assume `dotnet test` can pass offline.

## Architecture conventions

- `BergetClient` extends the official `OpenAIClient` (OpenAI 2.x package), so chat/embeddings/etc. come for free; the default endpoint is overridden to `https://api.berget.ai/v1`.
- Endpoint-specific sub-clients (e.g. `Rerank/RerankClient` for `/v1/rerank`) are built on `System.ClientModel.Primitives`: `ClientPipeline`, `PipelineMessage`, `ClientResult<T>`, and protocol-method overloads taking `BinaryContent`/`RequestOptions`. Follow the existing client's shape when adding a new endpoint, and share the parent pipeline via the internal constructor (see `BergetClient.GetRerankClient`).
- Auth is an `Authorization: Bearer` header via `ApiKeyAuthenticationPolicy`; message classification accepts only 200 (`PipelineMessageClassifier200`).
- Internal transport plumbing (response serialization, exception extraction, URI building) lives in `BergetSharp/Internal` — reuse `JsonUtilities`/`ClientPipelineExtensions` instead of duplicating.

## Serialization & models

- Model classes (`Model/Rerank/*`) are plain POCOs serialized with `System.Text.Json` (`JsonSerializerDefaults.Web`, `JsonIgnoreCondition.WhenWritingNull`).
- API payloads use snake_case (Cohere-compatible rerank format): every mapped property needs an explicit `[JsonPropertyName("snake_case")]`; add `[JsonExtensionData] Dictionary<string, JsonElement>? ExtensionData` to capture unknown fields. This is why the data differs from C# naming defaults — don't "fix" it.

## Workflow

- Commit messages use Conventional Commits (`feat:`, `refactor:`), one feature per commit.
