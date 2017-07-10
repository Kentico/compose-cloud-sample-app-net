#addin "Cake.MsDeploy"

var target = Argument<string>("target", "Default");
var publishSettings = Argument("publishSettings", "");
var zipPath = "../publish/DancingGoat.zip";

Task("Deploy")
  .WithCriteria(!String.IsNullOrEmpty(publishSettings) && FileExists(publishSettings))
  .WithCriteria(!String.IsNullOrEmpty(zipPath) && FileExists(zipPath))
  .Does(() =>
  {
    var publishProfile = GetPublishProfile(publishSettings);
    var webDestinationPath = "./";
    
    Information("Deploying to: " + publishProfile.MsDeployURL);
    CleanDeployedPath(webDestinationPath, publishProfile.UserName, publishProfile.Password, publishProfile.MsDeployURL);
    DeployArchive(zipPath, webDestinationPath, publishProfile.UserName, publishProfile.Password, publishProfile.MsDeployURL);
  });

private void CleanDeployedPath(string destinationPath, string userName, string password, string msdeployURL) 
{
    try
    {
        MsDeploy(new MsDeploySettings
        {
            Verb = Operation.Delete,
            Destination = new ContentPathProvider
            {
                Direction = Direction.dest,
                
                AuthenticationType = AuthenticationScheme.Basic,
                Username = userName,
                Password = password,
                ComputerName = msdeployURL,
                
                AppendQuotesToPath = true,
                Path = destinationPath
            },
        });
    }
    catch (CakeException ex) {
      if (ex.HResult != -2146233088)      // -2146233088 = FileOrFolderNotFound exception
      {
          throw ex;
      }
    }
}

private void DeployArchive(string zipFile, string destinationPath, string userName, string password, string msdeployURL) 
{
    MsDeploy(new MsDeploySettings
    {
        Verb = Operation.Sync,
        RetryAttempts = 5,
        RetryInterval = 5000,
        Source = new PackageProvider
        {
            Direction = Direction.source,
            Path = MakeAbsolute(File(zipFile)).FullPath
        },
        Destination = new ContentPathProvider
        {
            Direction = Direction.dest,
            
            IncludeAcls = false,
            AuthenticationType = AuthenticationScheme.Basic,
            Username = userName,
            Password = password,
            ComputerName = msdeployURL,
            
            AppendQuotesToPath = true,
            Path = destinationPath
        },
        EnableRules = new[] { "DoNotDelete", "AppOffline" }
    });
}

private class PublishProfile
{
    public string UserName { get; private set; }
    public string Password { get; private set; }
    public string MsDeployURL { get; private set; }
    
    public PublishProfile(string userName, string password, string msdeployURL)
    {
        UserName = userName;
        Password = password;
        MsDeployURL = msdeployURL;
    }
}

private string TrimStart(string source, string prefix)
{
    if (!source.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase)) return source;
    return source.Substring(prefix.Length);
}

private PublishProfile GetPublishProfile(string publishSettings)
{
    var baseProfilePath = "//publishData/publishProfile[@publishMethod='MSDeploy']/{0}";
    var publishUrl = XmlPeek(publishSettings, String.Format(baseProfilePath, "@publishUrl"));
    var userName = XmlPeek(publishSettings, String.Format(baseProfilePath, "@userName"));
    var site = XmlPeek(publishSettings, String.Format(baseProfilePath, "@msdeploySite"));
    var password = XmlPeek(publishSettings, String.Format(baseProfilePath, "@userPWD"));
    
    publishUrl = TrimStart(publishUrl, "https://");
    publishUrl = TrimStart(publishUrl, "http://");
    var msdeployURL = String.Format("https://{0}/msdeploy.axd?site={1}", publishUrl, site);    
    
    return new PublishProfile(userName, password, msdeployURL);
}

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