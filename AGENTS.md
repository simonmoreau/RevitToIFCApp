# RevitToIFCApp - Agent Instructions

## Architecture

- **Clean Architecture** monolith: `Domain` → `Application` (MediatR CQRS) → `Infrastructure` (EF Core) → `WebApp` (ASP.NET 8 API) + `WebClient` (Blazor WASM, MudBlazor)
- All projects target `net8.0` except `RevitToIFCBundle` (per-Revit-version: `net10.0-windows`/`net8.0-windows`/`net48`)
- **RevitToIFCBundle** (`src/RevitToIFCBundle/`): IExternalDBApplication plugin for Autodesk Design Automation. Build with `-c 2027` / `2026` / etc. to target a Revit version. PostBuild.ps1 copies DLL to Revit addin folder and zips the bundle.
- **Infrastructure DB**: `ConfigureServices.cs:12` uses **InMemory** (`UseInMemoryDatabase`) always. `DependencyInjection.cs` registers SQL Server but is not wired into the DI pipeline — don't add migrations unless you switch to SQL Server.
- **Auth**: Azure AD B2C + MSAL for Blazor WASM. WebApp validates JWTs via `MicrosoftIdentityWebApiAuthentication`. WebClient uses `MsalAuthentication` with redirect login mode.
- **Forge/APS**: Two SDK versions — `Autodesk.Forge.DesignAutomation` v6.0.1 (WebApp) and v5.1.2 (Application). Uses OSS (Object Storage Service) + Design Automation APIs.
- **Payments**: Stripe. Price tiers in `StripeSettings.Products` (from config). Checkout sessions stored in Azure Table Storage.

## Build & Test

```powershell
dotnet build -c Release           # full solution (except RevitToIFCBundle which needs -c <version>)
dotnet build src/RevitToIFCBundle -c 2027    # build bundle for Revit 2027
dotnet test test/ApplicationTest   # xUnit tests (InMemory DB, collection fixture "QueryCollection")
dotnet test test/WebAppClientTest  # bUnit + xUnit for Blazor components
```

No lint/format/typecheck scripts configured. CI pushes to master deploy via GitHub Actions.

## Key Conventions

- **No `var`** — always explicit type
- **No `#region`**, no local functions
- **`is null` / `is not null`** — never `== null` / `!= null`
- **Newline before `{`** on all control flow blocks
- **`_camelCase`** for private fields
- **`nameof`** instead of string literals
- **Early returns** over nested conditionals
- **XML doc comments** on all public APIs
- **DI & MediatR** for all application logic
- Controllers inherit `BaseController` which resolves `IMediator` from `HttpContext.RequestServices`

## Deployment

- Bicep template (`deploy.bicep`) provisions: App Service (Linux, Free tier, .NET 8), Static Web App (Free), Key Vault, Storage Account (RBAC + managed identity), Application Insights, budget alert.
- `deploy.ps1` creates resource group and deploys. **`deploy.parameters.json` is gitignored** — must be created locally.
- CI/CD: GitHub Actions `master_revittoifcappapi.yml` builds + deploys API to `app-bim42-prod-fr-rvt2ifc` and static site from `src/WebClient`.
- Secrets from Key Vault (dev uses `DefaultAzureCredential`, prod uses user-assigned managed identity).

## Testing Quirks

- `ApplicationTest` uses `AppContextFactory` (InMemory DB, seeded with sample `Objet` entities). Tests use `QueryTestFixture` (collection fixture `QueryCollection`) or `CommandTestBase` (per-test setup/teardown).
- No integration or E2E tests.
- RevitToIFCBundle is untestable outside Design Automation runtime.

## Startup

- `WebApp/Program.cs`: dev → `AddDevAzureServices` (Table Storage via `DefaultAzureCredential`), prod → `AddAzureServices` (Key Vault + managed identity). Dev also seeds the InMemory DB via `DbContextInitialiser`.
- `WebClient/Program.cs`: In dev, API calls go to same origin; in prod, uses `APIAdress` config key.
