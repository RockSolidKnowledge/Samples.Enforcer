using Rsk.Enforcer.Services.DataMasking;

namespace Masking
{
    internal class Response
    {
        [ConstantValueMaskingCategory("secret",MaskedValue = "********")]
        public string Message { get; set; }
        
        [EmailMaskingCategory("secret")]
        public string From { get; set; }
        public string To { get; set; }

        public override string ToString()
        {
            return $"{nameof(Message)}: {Message}, {nameof(From)}: {From}, {nameof(To)}: {To}";
        }
    }
}