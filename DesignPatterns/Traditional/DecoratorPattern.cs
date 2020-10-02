namespace DesignPatterns.Traditional.Decorator
{
    /*******************************************************************************
     * Decorator Pattern
     *******************************************************************************
     * What is it?
     * One of original GoF patterns to add/remove functional to existing objects
     * dynamically at run time (flexibility to avoid sub-classes)
     * 
     * Real-world Examples:
     *  - GUI / user interfaces for windows and pages (like view-port)
     *  - Business calculations or rule validations based on conditions
     *  - Add functionality to third party library
     * 
     * Demo
     *  Validation of a new user request needs to add new validation rules on top
     *  of a third party validation library that cannot be modified in any way.
     * 
     *******************************************************************************/

    /// <summary>
    /// This is a simple class/interface (assume 3rd party and can't be modified)
    /// </summary>
    public class NewUserRequest { public string Username { get; set; } public string Password { get; set; } }
    public interface IValidator { bool IsValid(NewUserRequest r); }
    public class ThirdPartyValidator : IValidator { public bool IsValid(NewUserRequest r) => string.IsNullOrEmpty(r.Username) == false; }

    // Add new validators (chaining decorators)
    public class MinLengthValidator : IValidator
    {
        protected IValidator _toDecorate;
        public MinLengthValidator(IValidator toDecorate) => _toDecorate = toDecorate;
        public bool IsValid(NewUserRequest r) => _toDecorate.IsValid(r) && r.Username.Length > 5;
    }

    public class PasswordValidator : IValidator
    {
        protected IValidator _toDecorate;
        public PasswordValidator(IValidator toDecorate) => _toDecorate = toDecorate;
        public bool IsValid(NewUserRequest r) => _toDecorate.IsValid(r) && (r.Password?.Length ?? 0) >= 6;
    }

    public class DemoValidation
    {
        public void Run()
        {
            // Configure available features to enhance base class
            IValidator validator = new PasswordValidator(new MinLengthValidator(new ThirdPartyValidator()));

            // Use just like base class
            var r = new NewUserRequest() { Username = "test", Password = "abc123" };
            var isValid = validator.IsValid(r);
        }
    }

}
