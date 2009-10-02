using System;

namespace OpenRasta.Codecs
{
    /// <summary>
    /// Represents the result of matching a codec to method parameters.
    /// </summary>
    public class CodecMatch : IComparable<CodecMatch>
    {
        public CodecMatch(CodecRegistration codecRegistration, float score, int matchingParameters)
        {
            if (codecRegistration == null)
                throw new ArgumentNullException("codecRegistration");

            CodecRegistration = codecRegistration;
            Score = score;
            MatchingParameterCount = matchingParameters;
        }

        public CodecRegistration CodecRegistration { get; private set; }
        public int MatchingParameterCount { get; private set; }
        public float Score { get; private set; }

        public int CompareTo(CodecMatch other)
        {
            if (other == null || other.CodecRegistration == null)
                return 1;
            if (this == other)
                return 0;
            float weightedScore = Score * CodecRegistration.MediaType.Quality;
            float otherWeightedScore = other.Score * other.CodecRegistration.MediaType.Quality;
            if (weightedScore == otherWeightedScore)
            {
                return MatchingParameterCount == other.MatchingParameterCount
                           ? CodecRegistration.MediaType.CompareTo(other.CodecRegistration.MediaType)
                           : MatchingParameterCount.CompareTo(other.MatchingParameterCount);
            }

// highest score is better, so we revert the comparison
            return weightedScore.CompareTo(otherWeightedScore);
        }
    }
}