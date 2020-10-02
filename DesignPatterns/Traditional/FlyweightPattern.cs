using System;

namespace DesignPatterns.Traditional.Flyweight
{
    /*******************************************************************************
     * Flyweight Pattern (+ Lazy?)
     *******************************************************************************
     * What is it?
     * One of original GoF patterns to persist high memory/expensive objects to share 
     * with similar objects
     * 
     * Real-world Examples:
     *  - Binary object persisted in memory (image, or file)
     *  - Caching (object/memory cache in .net)
     * 
     * Demo:
     * Persist implementations of an expensive object to share with users of the objects
     * 
     *******************************************************************************/

    public class ReferenceData
    {
        public string[] Provinces => _provinces.Value; // Class member to reference lazily loaded static value
        private static Lazy<string[]> _provinces = new Lazy<string[]>(() => new string[] { }); // Load lazily into static member
        // Alternately utilize object cache to force refresh after period of fixed time
    }
}
