using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Blog.Extensions
{
    public static class ModelStateExtensions
    {
        public static List<string> GetErrors(this ModelStateDictionary modelState)
        {
            List<string> result = new List<string>();
            foreach (var item in modelState.Values)
            {
                foreach (var error in item.Errors)
                {
                    result.Add(error.ErrorMessage);
                }
            }

            return result;
        }
    }
}
