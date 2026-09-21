# Repository Guidelines

## Project Purpose & Current State

This project is an AI-assisted DevSecOps learning environment for a representative Product Information Management (PIM) application using C# / ASP.NET Core, Azure, and Azure DevOps. It uses synthetic data and does not represent a real company's internal architecture.

## Project Structure & Module Organization

As components are introduced, use this layout:

- `src/`: application projects, organized by responsibility.
- `tests/`: corresponding unit and integration test projects.
- `pipelines/`: Azure DevOps pipeline templates.
- `infra/`: Azure infrastructure definitions.
- `docs/`: architecture, STRIDE threat models, learning notes, and vulnerability case studies.

Create directories only when they contain useful work. Keep test fixtures with their tests; use synthetic product and tenant data.

## Build, Test, and Development Commands

Use these commands from the repository root:

- `dotnet restore SecurePim.slnx`: restore and audit dependencies.
- `dotnet build SecurePim.slnx --no-restore`: compile with analyzers.
- `dotnet test SecurePim.slnx --no-build`: run the test suite.
- `dotnet run --project src/SecurePim.Api`: start the API locally.

## Coding Style & Naming Conventions

Follow `.editorconfig`: four-space indentation for C# and two spaces for YAML. Use PascalCase for types and public members, camelCase for parameters and local variables, and an `Async` suffix for asynchronous methods. Compiler analysis runs during builds.

## Testing Guidelines

Tests use xUnit. Name tests `Method_Scenario_ExpectedOutcome`. Verify authorization, tenant isolation, token validation, and secret handling as those features are added. Include regression tests for security fixes; keep intentionally vulnerable exercises isolated.

## Commit & Pull Request Guidelines

With no existing Git conventions, use concise imperative subjects such as `Add STRIDE model for product imports`. Keep changes focused. PRs should explain purpose, security impact, verification commands and results, and linked issues when available. Include screenshots only for UI changes.

## Security & Learning Practices

Never commit credentials, tokens, or customer data. Prefer managed identities and Key Vault for Azure workloads. Treat AI inputs and outputs as untrusted; require human review for security exceptions and production deployment. Explain what each security control does, how it works, and why it matters. Label simulated vulnerability stories as lab findings.
