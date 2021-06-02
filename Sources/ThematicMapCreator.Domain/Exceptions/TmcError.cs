using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;

namespace ThematicMapCreator.Domain.Exceptions
{
    public static class TmcError
    {
        [HttpStatusCode(HttpStatusCode.InternalServerError)]
        public const string InnerError = "Error.InnerError";

        private static readonly Dictionary<string, HttpStatusCode> httpStatusCodes = GetHttpStatusCodes();

        public static HttpStatusCode GetHttpStatusCode(string errorCode) =>
            httpStatusCodes.GetValueOrDefault(errorCode, HttpStatusCode.InternalServerError);

        private static Dictionary<string, HttpStatusCode> GetHttpStatusCodes()
        {
            return typeof(TmcError).GetNestedTypes().Append(typeof(TmcError))
                .SelectMany(nested => nested.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
                .Where(field => field.IsLiteral && !field.IsInitOnly)
                .ToDictionary(
                    field => (string)field.GetRawConstantValue(),
                    field => field.GetCustomAttribute<HttpStatusCodeAttribute>()?.StatusCode ?? HttpStatusCode.InternalServerError);
        }

        public static class Map
        {
            [HttpStatusCode(HttpStatusCode.UnprocessableEntity)]
            public const string LongDescription = "Error.Map.LongDescription";

            [HttpStatusCode(HttpStatusCode.UnprocessableEntity)]
            public const string LongName = "Error.Map.LongName";

            [HttpStatusCode(HttpStatusCode.UnprocessableEntity)]
            public const string NameRequired = "Error.Map.NameRequired";

            [HttpStatusCode(HttpStatusCode.NotFound)]
            public const string NotFound = "Error.Map.NotFound";

            [HttpStatusCode(HttpStatusCode.UnprocessableEntity)]
            public const string NotUniqueName = "Error.Map.NotUniqueName";

            [HttpStatusCode(HttpStatusCode.UnprocessableEntity)]
            public const string ShortName = "Error.Map.ShortName";

            [HttpStatusCode(HttpStatusCode.UnprocessableEntity)]
            public const string UserRequired = "Error.Map.UserRequired";
        }

        public static class User
        {
            [HttpStatusCode(HttpStatusCode.UnprocessableEntity)]
            public const string EmailRequired = "Error.User.EmailRequired";

            [HttpStatusCode(HttpStatusCode.UnprocessableEntity)]
            public const string InvalidEmail = "Error.User.InvalidEmail";

            [HttpStatusCode(HttpStatusCode.UnprocessableEntity)]
            public const string InvalidEmailOrPassword = "Error.User.InvalidEmailOrPassword";

            [HttpStatusCode(HttpStatusCode.NotFound)]
            public const string NotFound = "Error.User.NotFound";

            [HttpStatusCode(HttpStatusCode.UnprocessableEntity)]
            public const string PasswordNotConfirmed = "Error.User.PasswordNotConfirmed";

            [HttpStatusCode(HttpStatusCode.UnprocessableEntity)]
            public const string PasswordRequired = "Error.User.PasswordRequired";
        }

        public static class Layer
        {
            [HttpStatusCode(HttpStatusCode.UnprocessableEntity)]
            public const string DataRequired = "Error.Layer.DataRequired";

            [HttpStatusCode(HttpStatusCode.UnprocessableEntity)]
            public const string InvalidIndex = "Error.Layer.InvalidIndex";

            [HttpStatusCode(HttpStatusCode.UnprocessableEntity)]
            public const string InvalidStyle = "Error.Layer.InvalidStyle";

            [HttpStatusCode(HttpStatusCode.UnprocessableEntity)]
            public const string InvalidType = "Error.Layer.InvalidType";

            [HttpStatusCode(HttpStatusCode.UnprocessableEntity)]
            public const string LongDescription = "Error.Layer.LongDescription";

            [HttpStatusCode(HttpStatusCode.UnprocessableEntity)]
            public const string LongName = "Error.Layer.LongName";

            [HttpStatusCode(HttpStatusCode.UnprocessableEntity)]
            public const string NameRequired = "Error.Layer.NameRequired";

            [HttpStatusCode(HttpStatusCode.NotFound)]
            public const string NotFound = "Error.Layer.NotFound";

            [HttpStatusCode(HttpStatusCode.UnprocessableEntity)]
            public const string NotUniqueName = "Error.Layer.NotUniqueName";

            [HttpStatusCode(HttpStatusCode.UnprocessableEntity)]
            public const string ShortName = "Error.Layer.ShortName";
        }
    }
}
