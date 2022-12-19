using FluentValidation;

namespace DevoceanExample.WebApi.Tests.Util
{
    public class ValidatorUtil<T1, T2> where T1 : AbstractValidator<T2>
    {
        readonly T1 _validate;

        public ValidatorUtil(T1 validate)
        {
            _validate = validate;
        }

        public void ValidateOk(T2 dados)
        {
            var validationResult = _validate.Validate(dados);
            Assert.True(validationResult.IsValid);
            Assert.False(validationResult.Errors.Any());
        }

        public void ValidateErro(string message, T2 dados)
        {
            var validationResult = _validate.Validate(dados);
            Assert.Equal(message, validationResult.ToString());
            Assert.True(validationResult.Errors.Any());
            Assert.False(validationResult.IsValid);
        }
    }
}
