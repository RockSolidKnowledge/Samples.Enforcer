using Rsk.Enforcer.Services.DataMasking;

namespace MaskingWebApp.Models
{
    public class ResponseViewModel
    {
        [ConstantValueMaskingCategory("secret",MaskedValue = "********")]
        public string Message { get; set; }
        public string From { get; set; }
        public string To { get; set; }

        public override string ToString()
        {
            return $"{nameof(Message)}: {Message}, {nameof(From)}: {From}, {nameof(To)}: {To}";
        }
    }
}