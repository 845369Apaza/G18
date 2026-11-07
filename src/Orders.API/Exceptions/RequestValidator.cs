using System.ComponentModel.DataAnnotations;

namespace MiniApi.Exceptions;

public static class RequestValidator
{
    // Corre los Data Annotations del request y junta los problemas separados por ';'.
    public static void Validate(object request)
    {
        var errors = new List<ValidationResult>();
        if (Validator.TryValidateObject(request, new ValidationContext(request), errors, validateAllProperties: true))
            return;

        throw new ValidationException(
            ErrorCodes.InvalidData,
            string.Join("; ", errors.Select(e => e.ErrorMessage)));
    }
}
