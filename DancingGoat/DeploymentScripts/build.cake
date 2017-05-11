#addin "Cake.Compression"

var target = Argument<string>("target", "Default");
var config = Argument<string>("config", "Debug");

TaskSetup(context =>
{
    if (TeamCity.IsRunningOnTeamCity) 
        TeamCity.WriteStartBlock(context.Task.Name);
});

TaskTeardown(context =>
{
  if (TeamCity.IsRunningOnTeamCity) 
    TeamCity.WriteEndBlock(context.Task.Name);
});

Task("Publish")
  .Does(() => {
    CleanDirectory("../publish/");
    CreateDirectory("../publish/");
    
    MSBuild("../DancingGoat", new MSBuildSettings
    {
      Configuration = config,
      OutDir = "../publish/DancingGoat"
    });
    Zip("../publish/DancingGoat", "../publish/DancingGoat.zip");

    if (TeamCity.IsRunningOnTeamCity) 
        TeamCity.PublishArtifacts(MakeAbsolute(File("../publish/DancingGoat.zip")).FullPath);
  });

Task("Build")
  .Does(() => {
    NuGetRestore("../DancingGoat.sln");
    
    MSBuild("../DancingGoat.sln", new MSBuildSettings
    {
      Verbosity = Verbosity.Minimal,
      ToolVersion = MSBuildToolVersion.VS2015,
      Configuration = config,
      PlatformTarget = PlatformTarget.MSIL
    });
  });


Task("Default")
  .IsDependentOn("Build")
  .IsDependentOn("Publish");

RunTarget(target);