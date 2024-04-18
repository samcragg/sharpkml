var configuration = Argument("configuration", "Release");
bool isLocalBuild = BuildSystem.IsLocalBuild;
var target = Argument("target", "Default");

void UploadTestResults(FilePath result)
{
    if (isLocalBuild)
    {
        return;
    }

    string jobId = EnvironmentVariable("APPVEYOR_JOB_ID");
    try
    {
        UploadFile($"https://ci.appveyor.com/api/testresults/mstest/{jobId}", result);
    }
    catch (Exception ex)
    {
        Warning("Unable to upload test results");
        while (ex != null)
        {
            Warning("    " + ex.Message);
            ex = ex.InnerException;
        }
    }
}

Task("Build")
    .Does(() =>
{
    DotNetBuild("../SharpKml.sln", new DotNetBuildSettings
    {
        Configuration = configuration,
    });
});

Task("Pack")
    .Does(() =>
{
    DotNetPack("../SharpKml/SharpKml.Core.csproj", new DotNetPackSettings
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
    try
    {
        DotNetTest("../UnitTests/UnitTests.csproj", new DotNetTestSettings
        {
            Configuration = configuration,
            Loggers = ["trx;LogFileName=UnitTests.trx"],
            NoBuild = true,
            NoRestore = true,
            ResultsDirectory = "./TestResults/",
        });
    }
    finally
    {
        UploadTestResults("./TestResults/UnitTests.trx");
    }
});

Task("Default")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .IsDependentOn("Pack");

RunTarget(target);