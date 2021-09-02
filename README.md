# appSettingsEditor
app settings editor - powered by Roslyn

## What it does

Autogenerates controller API for appsettings.json

Optional, it has also an endpoint for a HTML GUI editor

This is how swagger looks :

<img src='https://github.com/ignatandrei/appSettingsEditor/blob/main/docs/images/swagger.png?raw=true' />

And you can browse to /settingsUI to have this editor ( powered by https://github.com/json-editor/json-editor )
	
<img src='https://github.com/ignatandrei/appSettingsEditor/blob/main/docs/images/settingsui.png?raw=true' />
	


## How to use 

  Add 2 NUGET  references:

  appSettingEditorAPI
  appSettingsEditor

For your convenience, please add to the csproj :
	
	
```xml
	
<ItemGroup>
    <PackageReference Include="appSettingsEditor" Version="2021.3.21.2300" />
    <PackageReference Include="appSettingsEditorAPI" Version="2021.3.21.2300" />
  	<CompilerVisibleItemMetadata Include="AdditionalFiles" MetadataName="generateAPI" />
    <AdditionalFiles Include="appsettings.json" generateAPI="true" >
    		<CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
	</AdditionalFiles>
</ItemGroup>
<PropertyGroup>
	<EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
	<CompilerGeneratedFilesOutputPath>$(BaseIntermediateOutputPath)Generated</CompilerGeneratedFilesOutputPath>
</PropertyGroup>

```

If you want to see a GUI for settings ( /settingsUI) , please do
	
```csharp
app.UseEndpoints(endpoints =>
{
endpoints.MapControllers(); // add next line
endpoints.MapSettingsView <SettingsJson.appsettings>(Configuration);
});
```

If you want to handle security, add

```csharp
public partial class appsettingsController : ControllerBase
{
//partial void BeforeGet(appsettings data)
//{

//}
//partial void BeforeSave(appsettings data, appsettings original)
//{

//}

}
```
	  
## Known problems and solving

### I have a versioning API .      

Add this to Startup.cs , 
```csharp
public void ConfigureServices(IServiceCollection services)
services.AddApiVersioning(
it =>
{
    it.AssumeDefaultVersionWhenUnspecified = true;
    it.DefaultApiVersion = new ApiVersion(1,0);
}
);
```


### I have another file name app_custom_settings.json, not appsettings.json

Make the modifications below:
In the csproj
```xml
<ItemGroup>
    <PackageReference Include="appSettingsEditor" Version="2021.3.21.2300" />
    <PackageReference Include="appSettingsEditorAPI" Version="2021.3.21.2300" />
    <CompilerVisibleItemMetadata Include="AdditionalFiles" MetadataName="generateAPI" />
    <AdditionalFiles Include="app_custom_setting.json" generateAPI="true">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </AdditionalFiles>
  </ItemGroup>
  <PropertyGroup>
    <EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
    <CompilerGeneratedFilesOutputPath>$(BaseIntermediateOutputPath)Generated</CompilerGeneratedFilesOutputPath>
  </PropertyGroup>
```

In the startup.cs
```csharp
appSettingsEditor.Extensions.MapSettingsView<app_custom_setting>(endpoints, Configuration,"app_custom_setting.json","/api/app_custom_setting");
```



### I have multiple appsettings.json

This is not supported yet for the GUI. Please make an issue and describe your problem
