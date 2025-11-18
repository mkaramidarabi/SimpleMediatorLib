# Publishing SimpleMediator to NuGet

## Prerequisites

1. A NuGet account (sign up at https://www.nuget.org/)
2. An API key from your NuGet account (create at https://www.nuget.org/account/apikeys)

## Steps to Publish

### 1. Update Package Metadata (if needed)

Edit `SimpleMediator.csproj` and update:
- `<Authors>` - Your name or organization
- `<Company>` - Your company name
- `<RepositoryUrl>` - Your GitHub repository URL
- `<Version>` - Increment version number for new releases

### 2. Build the Package

```bash
dotnet build --configuration Release
```

This will create the `.nupkg` file in `bin/Release/` directory.

### 3. Pack the Package (Alternative)

If you want to pack without building:

```bash
dotnet pack --configuration Release
```

### 4. Publish to NuGet.org

#### Option A: Using dotnet CLI (Recommended)

```bash
dotnet nuget push bin/Release/SimpleMediator.1.0.0.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
```

Replace `YOUR_API_KEY` with your NuGet API key and `1.0.0` with your actual version number.

#### Option B: Using NuGet CLI

```bash
nuget push bin/Release/SimpleMediator.1.0.0.nupkg YOUR_API_KEY -Source https://api.nuget.org/v3/index.json
```

### 5. Verify Publication

After publishing, wait a few minutes and check:
- https://www.nuget.org/packages/SimpleMediator

## Versioning

Follow [Semantic Versioning](https://semver.org/):
- **MAJOR** version for incompatible API changes
- **MINOR** version for backwards-compatible functionality additions
- **PATCH** version for backwards-compatible bug fixes

Update the `<Version>` in `SimpleMediator.csproj` before each release.

## Notes

- The package will be publicly available after publication
- It may take a few minutes for the package to appear in search results
- You can unlist a package but cannot delete it once published

