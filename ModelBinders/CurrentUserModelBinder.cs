using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;
using MiniGame.Services;
using MiniGame.Helpers;

public class CurrentUserModelBinder : IModelBinder
{
    private readonly JwtHelper _jwtHelper;
    private readonly UserService _userService;

    public CurrentUserModelBinder(JwtHelper jwtHelper, UserService userService)
    {
        _jwtHelper = jwtHelper;
        _userService = userService;
    }

    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var authHeader = bindingContext.HttpContext.Request.Headers["Authorization"].ToString();
        var token = authHeader.StartsWith("Bearer ") ? authHeader.Substring("Bearer ".Length) : authHeader;

        var payload = _jwtHelper.GetUserInfoFromToken(token);
        if (payload == null)
        {
            bindingContext.Result = ModelBindingResult.Success(null);
            return;
        }

        var user = await _userService.GetUserByIdAsync(payload.id);
        bindingContext.Result = ModelBindingResult.Success(user);
    }
}
