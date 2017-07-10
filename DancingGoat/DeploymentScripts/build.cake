var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var projectId = Argument("projectId", "");
var previewToken = Argument("previewToken", "");

TaskSetup(context =>
{
	if (TeamCity.IsRunningOnTeamCity) TeamCity.WriteStartBlock(context.Task.Name);
});

TaskTeardown(context =>
{
	if (TeamCity.IsRunningOnTeamCity) TeamCity.WriteEndBlock(context.Task.Name);
});

private void Build(string config)
{
    var solutionFile = "../../DancingGoat.sln";
    NuGetRestore(solutionFile);
    MSBuild(solutionFile, settings =>
        settings.SetConfiguration(config)
            .UseToolVersion(MSBuildToolVersion.VS2015)
            .SetVerbosity(Cake.Core.Diagnostics.Verbosity.Minimal));
}

private void Publish()
{
    var projectFile = "../DancingGoat.csproj";
    var projectName = System.IO.Path.GetFileNameWithoutExtension(projectFile);
    var publishDirectory = "../publish";
    var tempPublishDirectory = MakeAbsolute(Directory(publishDirectory + "/" + projectName));
    var publishArchive = publishDirectory + "/" + projectName + ".zip";

    CleanDirectory(tempPublishDirectory);
    CreateDirectory(tempPublishDirectory);
    if (FileExists(publishArchive))
    {
        DeleteFile(publishArchive);
    }

    MSBuild(projectFile, settings =>
        settings.SetConfiguration(configuration)
            .SetVerbosity(Cake.Core.Diagnostics.Verbosity.Minimal)
            .UseToolVersion(MSBuildToolVersion.VS2015)
            .WithTarget("Build;WebPublish")
            .WithProperty("WebPublishMethod","FileSystem")
            .WithProperty("publishUrl", tempPublishDirectory.FullPath)
            .WithProperty("ProfileTransformWebConfigEnabled", "false")
        );
        
    EditWebConfig(tempPublishDirectory.FullPath);

	Zip(tempPublishDirectory, publishArchive);
    DeleteDirectory(tempPublishDirectory, recursive:true);

    if (TeamCity.IsRunningOnTeamCity)
    {
        TeamCity.PublishArtifacts(MakeAbsolute(File(publishArchive)).FullPath);
    }
}

private void EditWebConfig(string directoryPath) {    
    var webConfig = File(directoryPath + "/Web.config");
    XmlPoke(webConfig, "/configuration/appSettings/add[@key = 'ProjectId']/@value", projectId);
    XmlPoke(webConfig, "/configuration/appSettings/add[@key = 'PreviewToken']/@value", previewToken);
}

Task("Build")
    .Does(() =>
{
    Build(configuration);
});

Task("Publish")
    .IsDependentOn("Build")
    .Does(() =>
{
    Publish();    
});

Task("Default")
    .IsDependentOn("Build")
    .Does(() =>
{
});

RunTarget(target);