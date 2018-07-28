var configuration = Argument("configuration", "Release");
bool isLocalBuild = BuildSystem.IsLocalBuild;
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
        Logger = "trx;LogFileName=UnitTests.trx",
        NoBuild = true,
        NoRestore = true,
        ResultsDirectory = "./TestResults/",
    });
});

Task("UploadTestResults")
    .WithCriteria(!isLocalBuild)
    .IsDependentOn("Test")
    .Does(() =>
{
    using (var client = new System.Net.WebClient())
    {
        string jobId = EnvironmentVariable("APPVEYOR_JOB_ID");
        client.UploadFile(
            "https://ci.appveyor.com/api/testresults/nunit3/" + jobId,
            "./TestResults/UnitTests.trx");
    }
});


// System.Net.WebClient'
//    $wc.UploadFile("https://ci.appveyor.com/api/testresults/nunit3/$($env:APPVEYOR_JOB_ID)", (Resolve-Path ./Adaptive.Agrona.Tests/TestResults/AgronaResults.trx))



Task("Default")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .IsDependentOn("Pack")
    .IsDependentOn("UploadTestResults");

RunTarget(target);