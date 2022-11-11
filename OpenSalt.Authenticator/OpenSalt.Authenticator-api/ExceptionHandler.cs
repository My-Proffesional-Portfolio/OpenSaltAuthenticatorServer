using OpenSalt.Authenticator_infrastructure.Exceptions;

namespace OpenSalt.Authenticator_api
{
    public class ExceptionHandler
    {
        public static object ExecuteAndCatching<T>(Func<T> method)
        {

            try
            {
                return method.Invoke();
            }
            catch (KeyStoreException ex)
            {
                return Results.Problem(ex.Message, statusCode: StatusCodes.Status401Unauthorized);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}
