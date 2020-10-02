namespace DesignPatterns.Extended.Guard
{
    /*******************************************************************************
     * Guard Pattern
     *******************************************************************************
     * What is it?
     * Not GoF, but very common to keep logic simple for methods (SOLID)
     * 
     * Real-world Examples:
     *  - Checking pre-conditions for a method (timer is stopped before starting)
     *  - Checking MVC model state
     * 
     * Demo
     * Model state validation for MVC controller action
     * 
     *******************************************************************************/

    // Mock API controller
    public interface IHttpAction { }
    public class ModelState { public bool IsValid { get; set; } }
    public abstract class ApiController
    {
        public ModelState ModelState { get; set; }
        public IHttpAction NotFound() => null;
        public IHttpAction BadRequest() => null;
        public IHttpAction Ok() => null;
    }

    // Model (might have data annotation attributes for validation)
    public class Book { }

    public class DemoController : ApiController
    {
        public IHttpAction CreateBook(Book model)
        {
            if (model == null) return NotFound(); // Guard
            if (ModelState.IsValid == false) return BadRequest(); // Guard
            // Save changes from user
            return Ok();
        }

        public IHttpAction CreateBook_Bad(Book model)
        {
            if (model != null)
            {
                if (ModelState.IsValid == true)
                {
                    // Save changes from user
                    return Ok();
                }
                else
                {
                    return BadRequest(); // Model state not valid
                }
            }
            else
            {
                return NotFound(); // Model not found/bound
            }
        }
    }
}
