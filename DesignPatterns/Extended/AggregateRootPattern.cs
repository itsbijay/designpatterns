using System;
using System.Collections.Generic;
using System.Linq;

namespace DesignPatterns.Extended.AggregateRoot
{
    /*******************************************************************************
     * Aggregate Root + Iterator
     *******************************************************************************
     * What is it?
     * "Domain-Driven Design" pattern where multiple domain objects can be treated
     * as aggregate domain root object ("Composition" from GoF patterns) to align
     * business rules and consistency within the composition.
     * 
     * Design Principles:
     *  - Controls access for all members of the root entity (never directly expose)
     *  - Other parts of system do not change the children of the aggregate root
     *  - Navigation should be limited to scope/children (others should be key only)
     *  - Deletions should be safe to cascade to children (otherwise, wrong pattern)
     * 
     * Real-world Examples:
     *  - Orders with line items (products would not be loaded since not managed)
     *  - Customers with Addresses
     * 
     * Demo
     *  - Initial Request: Customer has one or more addresses
     *  - Rule 1: Customer may not have 2+ addresses of the same type
     *  - Rule 2: Addreses of customer may need to be bound to view or view model
     * 
     *******************************************************************************/

    public class Address { public string Type { get; set; } }

    public class Customer_PoorDesign
    {
        // Gives direct access (bypassing business rules entirely to add/remove)
        // c.Addresses.Add(new Address() { Type = "home" });
        // c.Addresses.Add(new Address() { Type = "home" }); // Duplicate allowed
        public List<Address> Addresses { get; } = new List<Address> { };
    }

    public class Customer
    {
        private List<Address> _addresses { get; } = new List<Address> { };
        public virtual IEnumerable<Address> Addresses => _addresses; // Read-only access
        // public virtual IList<Address> Addresses_Bad => _addresses; // Allow read/write
        public void AddAddress(Address address) // Business rules preserved
        {
            if (_addresses.Any(a => a.Type.Equals(address.Type, StringComparison.OrdinalIgnoreCase)))
                throw new ApplicationException("Duplicate type");
            _addresses.Add(address);
        }
    }
}
