# Web.GetLatest

Web.GetLatest is a small command-line tool that fetches the latest GitHub release information for a repository and displays it in a tidy console panel. The CLI is implemented using Spectre.Console and is intended to be used as a lightweight helper for quickly checking release metadata.

CLI name: `ghver`

Key features
- Resolve repository and credentials from: command-line options, local user config (~/.ghver/config.json), or environment variables
- Fetch latest release metadata from the GitHub Releases API
- Filter releases by version prefix (for example: 2.5 -> latest v2.5.* release)
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

## Installing as a dotnet global tool

```sh
# Install latest published version
dotnet tool install -g ghver

# Install a specific version
dotnet tool install -g ghver --version 1.2.3

# Update
dotnet tool update -g ghver

# Uninstall
dotnet tool uninstall -g ghver
```


## Usage examples
```sh
# If installed as a global tool
ghver -o owner -r repo

# From local
# Fetch the latest release for a specific repo
dotnet run --project src/Web.GetLatest.Cli -- -o owner -r repo

# Provide GitHub credentials (PAT) for private repos or higher rate limits.
# Set GHVER_PAT through your shell's environment/secret-management facilities;
# do not replace the placeholder in a command that will be saved in shell history.
$env:GHVER_PAT = '<personal-access-token>'
dotnet run --project src/Web.GetLatest.Cli -- -o owner -r repo

# Fetch the latest release that matches v2.5.*
dotnet run --project src/Web.GetLatest.Cli -- -o owner -r repo -v 2.5

# Long option variant
dotnet run --project src/Web.GetLatest.Cli -- --owner owner --repo repo --version 2.5
```

## Short flags and tips:
  - -o|--owner <OWNER>  : Repository owner
  - -r|--repo <REPO>    : Repository name
  - -p|--pat <PAT>      : Personal Access Token (optional; may be exposed in process listings/history)
  - -v|--version <VER>  : Version prefix filter (for example: 2.5 -> v2.5.*)
  - -t|--trace          : Trace output (full exception stack traces and diagnostic logging)

## Configuration sources and defaults
- Local user config file (if present): the CLI searches for a .ghver/config.json file in the following locations (in order):
  1. %APPDATA%/.ghver/config.json (Windows Application Data)
  2. %LOCALAPPDATA%/.ghver/config.json (Windows Local Application Data)
  3. %USERPROFILE%/.ghver/config.json (legacy fallback)
  On Unix-like systems this typically maps to ~/.ghver/config.json. The file should contain JSON with fields such as Owner, Repo and optionally Username.
- For credentials, prefer the `GHVER_PAT` environment variable or an environment-backed secret-management tool. The CLI does not echo the token; normal output only indicates whether one is configured.
- Configuration precedence is command-line option, environment variable, local config, then built-in default:
  - GHVER_OWNER
  - GHVER_REPO
  - GHVER_PAT
- If owner/repo are not provided anywhere, defaults are used: Owner = Webbarrr, Repo = Web.GetLatest
- The `-p|--pat` option remains available as a convenience, but command-line arguments can be visible in shell history, process listings, terminal recordings, or CI diagnostics. Avoid it when possible.

## Development

- The solution contains three main projects:
  - src/Web.GetLatest.Cli - the CLI program and command wiring
  - src/Web.GetLatest.Application - application services and configuration providers
  - src/Web.GetLatest.Infrastructure - GitHub HTTP client and local config repository

Notes
- The HTTP client is pre-configured with GitHub API headers (User-Agent and API version).
