var configuration = Argument("configuration", "Release");
var target = Argument("target", "Default");

Task("Build")
    .Does(() =>
{
    DotNetCoreBuild("../SharpKml.sln", new DotNetCoreBuildSettings
    {
        Configuration = configuration,
    });
});

Task("Pack")
    .Does(() =>
{
    DotNetCorePack("../SharpKml/SharpKml.Core.csproj", new DotNetCorePackSettings
    {
        Configuration = configuration,
        NoBuild = true,
        NoRestore = true,
        OutputDirectory = "./artifacts/",
    });
});

Task("Test")
    .Does(() =>
{
    DotNetCoreTest("../UnitTests/UnitTests.csproj", new DotNetCoreTestSettings
    {
        Configuration = configuration,
        NoBuild = true,
        NoRestore = true,
    });
});

Task("Default")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .IsDependentOn("Pack");

RunTarget(target);