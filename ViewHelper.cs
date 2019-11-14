using DSM.Core.Ops;
using DSM.Core.Ops.ConsoleTheming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DSM.GatewayEngine
{
    public static class ViewHelper
    {
        public static List<ApiModel> GetApiGuides()
        {
            XConsole.SetDefaultColorSet(ConsoleColorSetBlueW.Instance);
            Type methods = typeof(WebOperations.WebMethod);
            FieldInfo[] fields = methods.GetFields();
            List<ApiModel> apiModels = new List<ApiModel>();
            string baseUrl = $"http://{DSM.Core.Ops.Extensions.GetLocalIPAddress()}:90/";
            foreach (FieldInfo field in fields)
            {
                string value = field.GetValue(null) as string;
                if (value.Last() == '/' && value.Length > 1)
                {
                    value = value + "{" + field.Name.Split('_').Last().ToLower() + "}";
                }

                string fieldValue = string.Join(baseUrl, value);
                apiModels.Add(new ApiModel { Command = fieldValue, Description = field.Name });
            }
            return apiModels;
        }
    }
}
