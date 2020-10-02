namespace DesignPatterns.Traditional.Facade
{
    /*******************************************************************************
     * Facade Pattern
     *******************************************************************************
     * What is it?
     * One of original GoF patterns to consolidate sub-systems to simplify access.
     * Commonly confused with Adapter, but not adapting to existing code.
     * 
     * Real-world Examples:
     *  - IOC with Dependency Injection to simplify constructors (limit 3-4 params)
     *  - Basis for Unit of Work enterprise pattern with Repositories
     * 
     * Demo
     * Simplify construction of controller with multiple dependencies
     * 
     *******************************************************************************/

    // Assume we have 3 interfaces used in authentication and authorization
    public interface IUserService { }
    public interface IAuthService { }
    public interface IProfileService { }
    
    // Consolidate these "sub-systems" with a facade
    public interface ISecurityFacade
    {
        IUserService UserService { get; }
        IAuthService AuthService { get; }
        IProfileService ProfileService { get; }
    }
    public class SecurityFacade
    {
        public IUserService UserService { get; }
        public IAuthService AuthService { get; }
        public IProfileService ProfileService { get; }
        public SecurityFacade(IUserService userService, IAuthService authService, IProfileService profilService)
        {
            this.UserService = userService;
            this.AuthService = authService;
            this.ProfileService = profilService;
        }
    }

    // Now controller only needs 1 IOC constructor injection instead of 3 for this security control
    public class AuthController
    {
        public ISecurityFacade Security { get; }
        public AuthController(ISecurityFacade security)
        {
            this.Security = security;
        }
    }
}
