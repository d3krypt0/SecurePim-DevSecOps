# SonarQube SAST on GitHub

## What Was Built

`.github/workflows/sonarqube.yml` runs SonarQube analysis around the normal .NET build and test commands. It uploads C# static-analysis results, test results, and OpenCover coverage to SonarQube Cloud. The scanner waits for the quality gate and fails the GitHub Actions job when that gate fails.

The scanner is pinned in `.config/dotnet-tools.json`, so local and CI runs use the same version. The workflow receives read-only repository permission and does not run with repository secrets for pull requests from forks.

## Why It Works This Way

SonarScanner for .NET must start before MSBuild and finish after the build and tests. The begin step injects analyzers and records build context. The end step uploads the resulting analysis. Full Git history is checked out so SonarQube can assign new code and blame information correctly.

The token is an encrypted GitHub Actions secret. Project and organization identifiers are repository variables because they are identifiers rather than credentials.

## Connect the GitHub Repository

1. Sign in to [SonarQube Cloud](https://sonarcloud.io) with GitHub and import the repository.
2. Record the project key and organization key shown by SonarQube Cloud.
3. Generate a scoped analysis token for the project.
4. In GitHub, open **Settings > Secrets and variables > Actions**.
5. Create the repository secret `SONAR_TOKEN` containing the analysis token.
6. Create repository variables `SONAR_PROJECT_KEY` and `SONAR_ORGANIZATION`.
7. Open **Actions > SonarQube SAST > Run workflow**, or push to `main`.

Never place the token in YAML, source files, workflow output, pull-request text, or a committed `.env` file.

## Expected Result

The GitHub Actions job restores dependencies, starts analysis, builds, runs tests with coverage, uploads results, and waits for the quality gate. The SonarQube project then displays security issues, reliability and maintainability findings, security hotspots, duplications, and coverage.

After the first successful run, require the **SonarQube SAST / Analyze .NET solution** check in the `main` branch protection rules. Review security hotspots manually; SonarQube does not classify a hotspot as a confirmed vulnerability without review.

## Current Boundary

The workflow targets SonarQube Cloud at `https://sonarcloud.io`. A self-hosted SonarQube server requires a reachable server URL, a different secret configuration, and either network exposure or a self-hosted GitHub runner.
