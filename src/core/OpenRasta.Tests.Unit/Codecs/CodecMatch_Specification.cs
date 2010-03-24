using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OpenRasta.Codecs;
using OpenRasta.Testing;
using OpenRasta.TypeSystem;
using OpenRasta.Web;

namespace CodecMatch_Specification
{
    public class when_sorting_codec_matches : context
    {
        [Test]
        public void the_highest_score_wins()
        {
            var codec1 = CreateCodecMatch("application/xml", 0, 1);
            var codec2 = CreateCodecMatch("application/xml", 1, 1);

            List<CodecMatch> list = GetSoretedList(codec1, codec2);

            list[0].ShouldBe(codec2);
        }
        [Test]
        public void when_score_is_the_same_the_highest_matching_parameters_is_selected()
        {
            var codec1 = CreateCodecMatch("application/xml;q=0.9", 0, 1);
            var codec2 = CreateCodecMatch("application/xhtml+xml;q=1", 0, 2);

            List<CodecMatch> list = GetSoretedList(codec1, codec2);

            list[0].ShouldBe(codec2);
        }
        [Test]
        public void when_score_and_param_matching_are_the_same_the_highest_quality_is_selected()
        {
            var codec1 = CreateCodecMatch("application/xml;q=0.9", 0, 1);
            var codec2 = CreateCodecMatch("application/xhtml+xml;q=1", 0, 1);

            List<CodecMatch> list = GetSoretedList(codec1, codec2);

            list[0].ShouldBe(codec2);
            
        }
        [Test]
        public void a_null_value_is_never_at_the_top_of_the_list()
        {
            var codec = CreateCodecMatch("application/xml;q=0.9", 0, 1);

            var list = GetSoretedList(codec, null);

            list[0].ShouldBe(codec);

        }
        private List<CodecMatch> GetSoretedList(params CodecMatch[] codecs)
        {
            var list = new List<CodecMatch>();
            list.AddRange(codecs);
            list.Sort();
            list.Reverse();//we want from higher to smaller
            return list;
        }

        private CodecMatch CreateCodecMatch(string mediaType, float score, int matchingParameters)
        {
            return
                new CodecMatch(
                    new CodecRegistration(
                        typeof (string), 
                        TypeSystems.Default.FromClr(typeof (object)), new MediaType(mediaType)), score, matchingParameters);
        }
    }
}
