using System;

namespace DesignPatterns.Traditional.Builder
{
    /*******************************************************************************
     * Builder
     *******************************************************************************
     * What is it?
     * Original GoF pattern tries to simplify the creation of complex objects.
     * While similar to Factory pattern, Builder is not limited to single method
     * allowing the user to use a process to build the object.
     * 
     * Design Principles:
     *  - Object properties are usually gradually defined (ex: Fluent API)
     *  - Private object is usually built with each call until final object returned
     *  - Business rules can be managed at each component of the build
     * 
     * Real-world Examples:
     *  - Moq (.Setup method, .Object) and FluentAssertions in unit testing (.Should, .And, ...)
     *  - Anything using Fluent API to construct objects (ex: Elasticsearch)
     * 
     * Demo
     *  - Create an address with Fluent API
     * 
     *******************************************************************************/

    // Define the simple address
    public class Address { public string Street { get; set; } public string State { get; set; } }

    // Optional (if multiple builders), abstract class to share logic preventing multiple builds
    public abstract class BuilderBase
    {
        private bool _built = false; // Track whether built, could even track by component with dictionary to prevent dupes
        protected bool ValidateBuild() => (_built == false) ? true : throw new ApplicationException("Already built");
        protected void CompleteBuild() => _built = true;
    }

    // Create the builder
    public class AddressBuilder : BuilderBase
    {
        protected Address _address = new Address(); // Always private/protected

        // Fluent options
        public AddressBuilder WithStreet(string street)
        {
            this.ValidateBuild(); // Could add logic to base 
            _address.Street = street;
            return this; // Self-reference for fluency
        }
        public AddressBuilder WithState(string state)
        {
            this.ValidateBuild();
            _address.State = state; // Any business rules, lookup with db cross-walk?
            return this; // Self-reference for fluency
        }
        public Address Build()
        {
            this.CompleteBuild(); // Any final steps/initialization or validation of missing components?
            return _address;
        }
    }

    // Use builder to fluently create the address
    public class BuilderDemo
    {
        public static void Run()
        {
            Address address = new AddressBuilder()
                .WithStreet("123 Main Street")
                .WithState("GA")
                .Build();
        }
    }
}
