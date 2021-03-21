using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestEditor.SettingsJson;
namespace TestEditor.Controllers
{
    public partial class appsettingsController : ControllerBase
    {
        partial void BeforeGet(appsettings data)
        {
            string s = data.ToString();
        }
        partial void BeforeSave(appsettings data, appsettings original)
        {

        }

    }
}
