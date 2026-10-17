# Web.GetLatest

Web.GetLatest is a small command-line tool that fetches the latest GitHub release information for a repository and displays it in a tidy console panel. The CLI is implemented using Spectre.Console and is intended to be used as a lightweight helper for quickly checking release metadata.

CLI name: `ghver`

Key features
- Resolve repository and credentials from: command-line options, local user config (~/.ghver/config.json), or environment variables
- Fetch latest release metadata from the GitHub Releases API
- Nicely formatted console output using Spectre.Console

## Supported platforms
- .NET 10

## Quick start

```sh
# build
dotnet build

# run
dotnet run --project src/Web.GetLatest.Cli -- [options]
```

## Usage examples
```sh
# Fetch the latest release for a specific repo
dotnet run --project src/Web.GetLatest.Cli -- -o owner -r repo

# Provide GitHub credentials (PAT) for private repos or higher rate limits
dotnet run --project src/Web.GetLatest.Cli -- -o owner -r repo -p <personal-access-token>
```

## Short flags and tips:
  - -o|--owner <OWNER>  : Repository owner
  - -r|--repo <REPO>    : Repository name
  - -p|--pat <PAT>      : Personal Access Token (optional)
  - -v|--verbose        : Verbose output (stack traces on error)

## Configuration sources and defaults
- Local user config file (if present): %USERPROFILE%/.ghver/config.json on Windows or ~/.ghver/config.json on Linux/macOS. The file should contain JSON matching the expected shape (Owner, Repo).
- Environment variables (take precedence over local config when command-line option is not provided):
  - GHVER_OWNER
  - GHVER_REPO
  - GHVER_PAT
- If owner/repo are not provided anywhere, defaults are used: Owner = Webbarrr, Repo = Web.GetLatest

## Development

- The solution contains three main projects:
  - src/Web.GetLatest.Cli - the CLI program and command wiring
  - src/Web.GetLatest.Application - application services and configuration providers
  - src/Web.GetLatest.Infrastructure - GitHub HTTP client and local config repository

Notes
- The HTTP client is pre-configured with GitHub API headers (User-Agent and API version).
