# appSettingsEditor
app settings editor - powered by Roslyn


 ** C# 9.0 ONLY **

      Autogenerates controller API for appsettings.json
      Add 2 NUGET  references:
      appSettingEditorAPI
      appSettingsEditor

      If you want to see a GUI for settings , please do
      app.UseEndpoints(endpoints =>
      {
      endpoints.MapControllers(); // add next line
      endpoints.MapSettingsView &lt; appsettings &gt;(Configuration);
      });

      If you want to handle security, add

      public partial class appsettingsController : ControllerBase
      {
      //private partial void BeforeGet(appsettings data)
      //{

      //}
      //private partial void BeforeSave(appsettings data, appsettings original)
      //{

      //}

      }
      
