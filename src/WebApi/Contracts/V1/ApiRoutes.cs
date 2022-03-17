namespace WebApi.Contracts.V1
{
    public static class ApiRoutes
    {
        public const string Root = "";

        public const string Version = "V1";

        public const string Base = Root + "/" + Version;

        public static class Auth
        {
            public const string Register = Base + "/Register";

            public const string Login = Base + "/Login";

            public const string Logout = Base + "/Logout";
        }
    }
}
