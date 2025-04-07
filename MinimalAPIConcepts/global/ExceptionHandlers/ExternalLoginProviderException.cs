namespace NEXT.GEN.ExceptionHandlers
{
    public class ExternalLoginProviderException(string provider, string message) :
        IOException($"External login provider : {provider} error occured : {message}");
}