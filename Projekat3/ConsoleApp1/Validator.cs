using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
    public static class Validator
    {
        public static bool IsValidKeyword(string keyword)
        {
            return !string.IsNullOrEmpty(keyword);
        }

        public static bool IsValidCategory(string category)
        {
            List<string> validCategories = new List<string> {
                "business", "entertainment", "general", "health", "science", "sports", "technology"
            };

            return validCategories.Contains(category.ToLower());
        }
    }
}
