using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace appSettingsEditor
{
    public interface IAppSettingsUtils
    {
        IEnumerable<string> Properties();

        object GetFromPropertyName(string propName, bool returnNull = false);

    }
    public interface IAppSettingsConfig<T> : IAppSettingsUtils
    {
        T LoadFromConfig(IConfiguration config);
    }
}
