# Release Process

This document describes how to release a new version of MapOnly to NuGet.org.

## Automated Release via GitHub Actions

The project is configured to automatically build and publish NuGet packages to NuGet.org using GitHub Actions.

### Prerequisites

1. **NuGet API Key**: A NuGet.org API key must be configured as a GitHub secret named `NUGET_API_KEY`
   - Go to Repository Settings → Secrets and variables → Actions
   - Add a new secret named `NUGET_API_KEY` with your NuGet.org API key

### Release Steps

1. **Update Version Number**
   - Edit `src/MapOnlyStandard/MapOnly.csproj`
   - Update the `<Version>`, `<AssemblyVersion>`, and `<FileVersion>` properties
   - Commit and push the changes

2. **Create and Push a Tag**
   ```bash
   git tag v1.0.0
   git push origin v1.0.0
   ```
   
   The tag must follow the pattern `v*.*.*` (e.g., `v1.0.0`, `v1.2.3`)

3. **Automated Build and Publish**
   - The GitHub Actions workflow will automatically trigger
   - It will:
     - Build the project for all target frameworks
     - Run all tests
     - Pack the NuGet package (.nupkg)
     - Pack the symbol package (.snupkg) with debugging symbols
     - Publish both packages to NuGet.org
     - Upload both packages as build artifacts for download

4. **Verify the Release**
   - Check the Actions tab in GitHub to see the workflow run
   - Once complete, verify the packages are available on NuGet.org: https://www.nuget.org/packages/MapOnly/
   - Download packages from the workflow run:
     - Go to the Actions tab and select the workflow run
     - Scroll to the bottom to find the "Artifacts" section
     - Download `nuget-package` (contains .nupkg file)
     - Download `symbol-package` (contains .snupkg file with debugging symbols)

### Manual Trigger

The workflow can also be manually triggered:
1. Go to Actions → Build and Publish NuGet Package
2. Click "Run workflow"
3. Select the branch and click "Run workflow"

### Troubleshooting

- **Authentication Failed**: Verify the `NUGET_API_KEY` secret is correctly set
- **Build Failed**: Check the workflow logs in the Actions tab
- **Package Already Exists**: The workflow uses `--skip-duplicate` to avoid errors if the version already exists

## Current Version

- **Version 1.0.0**: Initial stable release with automated CI/CD
- NuGet Package: https://www.nuget.org/packages/MapOnly/1.0.0
