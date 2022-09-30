using System.ComponentModel;

namespace QRCodeGenerator.Model
{
    public enum LanguageEnum
    {
        [Description("pt-BR")]
        PTBR,

        [Description("en-US")]
        ENUS
    }

    public class LanguageModel
    {
        public string Name { get; set; }

        public string ImageSource { get; set; }

        public LanguageEnum LanguageEnum { get; set; }
    }
}