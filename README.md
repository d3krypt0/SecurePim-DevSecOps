# SecurePim DevSecOps Lab

SecurePim is an AI-assisted DevSecOps reference project for a synthetic Product Information Management platform. It is a learning environment and makes no claims about Inriver's internal architecture.

## Milestone 1: Secure Build Baseline

**What:** a minimal ASP.NET Core service with a health endpoint, security response headers, automated tests, compiler analysis, dependency auditing, and Azure DevOps validation.

**Why:** every later security control needs a repeatable build and a small known-good application baseline. Failing vulnerable NuGet restores prevents known-risk dependencies from silently entering the build.

**How:** requests enter Kestrel and pass through HTTPS redirection and response-header middleware before reaching `/health`. Azure Pipelines restores packages with NuGet audit enabled, builds with .NET analyzers, and runs the tests. No secret or cloud connection is needed.

```powershell
dotnet restore SecurePim.slnx
dotnet build SecurePim.slnx --no-restore
dotnet test SecurePim.slnx --no-build
dotnet run --project src/SecurePim.Api
```

Browse to the HTTPS URL shown by `dotnet run`, followed by `/health`. Success is HTTP 200 with body `Healthy`. The current milestone costs $0 because it runs locally or on an Azure DevOps Microsoft-hosted allowance, where available.

## Milestone 2: Synthetic PIM API and STRIDE

**What:** a small product catalog supports listing, lookup, and creation of synthetic products. It validates input, normalizes SKUs, rejects duplicate SKUs atomically, filters storage by trusted tenant context, and omits tenant identifiers from responses.

**Why:** product data is the central PIM asset. Validation protects its integrity, atomic duplicate checks prevent inconsistent state during concurrent requests, and token-derived tenant context prevents clients from selecting another tenant. Milestone 3 now protects these routes in every environment.

**How:** authorization policies run before endpoint handlers, which validate and normalize the request before calling the singleton in-memory catalog. The catalog applies tenant filters to reads and performs duplicate checking and insertion within one lock. Response contracts expose product fields without the internal tenant identifier.

Run locally and use the HTTPS address printed by ASP.NET Core:

```powershell
dotnet run --project src/SecurePim.Api
curl.exe -k -i https://localhost:<port>/api/products/
```

Review [the STRIDE threat model](docs/threat-model.md), [the roadmap](docs/roadmap.md), and [the target architecture](docs/architecture.md). All current functionality remains local and costs $0.

## Milestone 3: OAuth/OIDC Authorization

**What:** product routes now require validated JWT access tokens. Reads require `products:read`, writes require `products:write`, and every product operation requires subject and tenant claims. Product creation emits a structured audit event tied to the authenticated subject.

**Why:** signature, issuer, audience, and lifetime validation establish whether the API can trust a token. Scope checks limit allowed operations, while token-derived tenant context prevents clients from selecting another tenant. These controls allow product routes to remain mapped in Production without anonymous access.

**How:** ASP.NET Core's JWT bearer handler retrieves Auth0 signing metadata over HTTPS and validates incoming access tokens. Authorization policies run before endpoint handlers. Validated claims supply the tenant boundary used by the catalog. Local integration tests substitute a test signing key, so all negative cases run without an Auth0 account.

See [OAuth/OIDC and Auth0 integration](docs/authentication.md) for the token flow, required claims, Auth0 activation steps, and current test coverage.

## Milestone 4: SonarQube SAST

**What:** GitHub Actions runs SonarScanner for .NET on pushes to `main`, manual runs, and trusted pull requests. It uploads analysis, test results, and coverage, then waits for the SonarQube quality gate.

**Why:** automated SAST gives developers feedback while a change is still reviewable. Requiring the quality-gate check on `main` prevents code that violates the agreed gate from merging.

**How:** the scanner begins before compilation, observes the analyzer-enabled build and tests, and uploads results after they finish. GitHub stores the analysis token as a secret; project identifiers are variables. See [SonarQube SAST on GitHub](docs/sast.md) for setup and expected results.
