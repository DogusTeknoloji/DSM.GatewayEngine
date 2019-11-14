namespace DSM.GatewayEngine
{
    public static class Extensions
    {
        public static string WithParamSiteId(this string value)
        {
            return string.Concat(value, "/{SiteId}");
        }

        public static string WithParamMachineName(this string value)
        {
            return string.Concat(value, "/{MachineName}");
        }

        public static string WithParamId(this string value)
        {
            return string.Concat(value, "/{Id}");
        }

    }
}
