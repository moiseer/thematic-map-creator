using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;

namespace Core.Exceptions
{
    public static class TmcError
    {
        private static readonly Dictionary<string, HttpStatusCode> httpStatusCodes = GetHttpStatusCodes();

        private static Dictionary<string, HttpStatusCode> GetHttpStatusCodes()
        {
            return typeof(TmcError)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(field => field.IsLiteral && !field.IsInitOnly)
                .ToDictionary(
                    field => (string)field.GetRawConstantValue(),
                    field => field.GetCustomAttribute<HttpStatusCodeAttribute>()?.StatusCode ?? HttpStatusCode.InternalServerError);
        }

        public static HttpStatusCode GetHttpStatusCode(string errorCode)
        {
            return httpStatusCodes.GetValueOrDefault(errorCode, HttpStatusCode.InternalServerError);
        }

        public static class Map
        {
            [HttpStatusCode(HttpStatusCode.UnprocessableEntity)]
            public const string LongDescription = "Error.Map.LongDescription";

            [HttpStatusCode(HttpStatusCode.UnprocessableEntity)]
            public const string LongName = "Error.Map.LongName";

            [HttpStatusCode(HttpStatusCode.UnprocessableEntity)]
            public const string NameRequired = "Error.Map.NameRequired";

            [HttpStatusCode(HttpStatusCode.UnprocessableEntity)]
            public const string NotUniqueName = "Error.Map.NotUniqueName";

            [HttpStatusCode(HttpStatusCode.UnprocessableEntity)]
            public const string ShortName = "Error.Map.ShortName";

            [HttpStatusCode(HttpStatusCode.UnprocessableEntity)]
            public const string UserRequired = "Error.Map.UserRequired";
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

            [HttpStatusCode(HttpStatusCode.UnprocessableEntity)]
            public const string NotUniqueName = "Error.Layer.NotUniqueName";

            [HttpStatusCode(HttpStatusCode.UnprocessableEntity)]
            public const string ShortName = "Error.Layer.ShortName";
        }
    }
}
