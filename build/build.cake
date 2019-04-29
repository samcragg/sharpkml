var configuration = Argument("configuration", "Release");
bool isLocalBuild = BuildSystem.IsLocalBuild;
var target = Argument("target", "Default");

void UploadTestResults(FilePath result)
{
    if (isLocalBuild)
    {
        return;
    }

    using (var client = new System.Net.WebClient())
    {
        string jobId = EnvironmentVariable("APPVEYOR_JOB_ID");
        try
        {
            client.UploadFile(
                "https://ci.appveyor.com/api/testresults/mstest/" + jobId,
                result.FullPath);
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
}

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
    try
    {
        DotNetCoreTest("../UnitTests/UnitTests.csproj", new DotNetCoreTestSettings
        {
            Configuration = configuration,
            Logger = "trx;LogFileName=UnitTests.trx",
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