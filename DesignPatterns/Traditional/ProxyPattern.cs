using System;
using System.Collections.Generic;

namespace DesignPatterns.Traditional.Proxy
{
    /*******************************************************************************
     * Proxy Pattern (+ Flyweight and Strategy)
     *******************************************************************************
     * What is it?
     * One of original GoF patterns to perform additional logic when accessing
     * an underlying object (or subject)
     * 
     * Real-world Examples:
     *  - Security must be checked before accessing a method
     *  - Combine with "Strategy" (below) to auto-select implementation by rules
     *  - MDM Search
     * 
     * Demo:
     * Utilize a search strategy based on the request with a combination of strategy
     * pattern with the proxy pattern to auto-select appropriate search. ALso, use
     * a flyweight pattern to avoid initializing many instances in high volumes.
     * 
     *******************************************************************************/

    public interface ISearchStrategy { string[] Search(string text, string type); }
    public class FullTextSearch : ISearchStrategy { public string[] Search(string text, string type) => new string[] { }; }
    public class FuzzySearch : ISearchStrategy { public string[] Search(string text, string type) => new string[] { }; }
    public class ContainsSearch : ISearchStrategy { public string[] Search(string text, string type) => new string[] { }; }
    public class SearchProxy : ISearchStrategy
    {
        public static void ExampleConroller()
        {
            var searcher = new SearchProxy(); // Or with IOC
            var results = searcher.Search("text", "fuzzy");
        }

        private Dictionary<string, ISearchStrategy> _strategies { get; } = new Dictionary<string, ISearchStrategy>
        { // Flyweight pattern
            { "fullText", new FullTextSearch() },
            { "fuzzy", new FuzzySearch() },
            { "contains", new ContainsSearch() }
        };

        public string[] Search(string text, string type)
        {
            if (_strategies.TryGetValue(type, out ISearchStrategy strategy) == false)
                throw new ApplicationException("Invalid strategy selected"); // Guard pattern
            return strategy.Search(text, type);
        }
    }
}
