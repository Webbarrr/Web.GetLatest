using System;
using System.Collections.Generic;
using System.Text;

namespace Web.GetLatest.Domain;

public sealed class Repository
{
    public string Owner { get; private set; }
    public string RepoName { get; private set; }

    public Repository(string owner, string repoName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(owner);
        ArgumentException.ThrowIfNullOrWhiteSpace(repoName);

        if (Uri.TryCreate(repoName, UriKind.Absolute, out var uri))
        {
            var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length >= 2)
            {
                owner = segments[0];
                repoName = segments[1].Replace(".git", "", StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                throw new ArgumentException("Invalid GitHub repository URL format.", nameof(repoName));
            }
        }
        else
        {
            var parts = repoName.Split('/');
            if (parts.Length == 2)
            {
                owner = parts[0];
                repoName = parts[1].Replace(".git", "", StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                throw new ArgumentException("Path must be a valid GitHub URL or 'owner/repo' format.", nameof(repoName));
            }
        }
    }
}
