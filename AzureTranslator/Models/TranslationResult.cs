using System.Collections.Generic;

namespace AzureTranslator.Models
{
    public class TranslationResult
    {
        public DetectedLanguage? DetectedLanguage { get; set; }

        public TextResult? SourceText { get; set; }

        public IEnumerable<Translation>? Translations { get; set; }
    }

    public class DetectedLanguage
    {
        public string? Language { get; set; }

        public float Score { get; set; }
    }

    public class TextResult
    {
        public string? Text { get; set; }

        public string? Script { get; set; }
    }

    public class Translation
    {
        public string? Text { get; set; }

        public TextResult? Transliteration { get; set; }

        public string? To { get; set; }

        public Alignment? Alignment { get; set; }

        public SentenceLength? SentLen { get; set; }
    }

    public class Alignment
    {
        public string? Proj { get; set; }
    }

    public class SentenceLength
    {
        public IEnumerable<int>? SrcSentLen { get; set; }

        public IEnumerable<int>? TransSentLen { get; set; }
    }
}