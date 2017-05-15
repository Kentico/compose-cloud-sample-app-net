#addin "Cake.MsDeploy"

var target = Argument<string>("target", "Default");
var username = Argument<string>("username", "");
var password = Argument<string>("password", "");
var computer = Argument<string>("computer", "");

Task("Deploy")
  .Does(() =>
  {
	CopyDirectory("../bin/roslyn", "../../publish/DancingGoat/_PublishedWebsites/DancingGoat/bin/roslyn");
	Zip("../../publish/DancingGoat/_PublishedWebsites/DancingGoat", "../../publish/DancingGoat.zip");

    MsDeploy(new MsDeploySettings
    {
        Verb = Operation.Sync,
        RetryAttempts = 5,
        RetryInterval = 5000,
        Source = new PackageProvider
        {
            Direction = Direction.source,
            Path = MakeAbsolute(File("../../publish/DancingGoat.zip")).FullPath
        },
        Destination = new AutoProvider
        {
            Direction = Direction.dest,
            IncludeAcls = false,
            AuthenticationType = AuthenticationScheme.Basic,
            Username=username,
            Password=password,
            ComputerName = computer,            
        },
        EnableRules = new[]{"AppOffline"}
    });
  });

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

Task("Default")
  .IsDependentOn("Deploy");

RunTarget(target);