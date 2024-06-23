using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
    public static class TopicModeling
    {
        public static List<string> ExtractTopics(IEnumerable<string> titles)
        {
            var topics = new List<string>();

            foreach (var title in titles)
            {
                var words = title.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var word in words)
                {
                    topics.Add(word.ToLower());
                }
            }

            return topics.Distinct().ToList();
        }
    }
}
