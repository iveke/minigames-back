using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using MiniGame.Models;

public class CurrentUserModelBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        if (context.Metadata.ModelType == typeof(User) &&
            context.BindingInfo.BindingSource == BindingSource.Custom)
        {
            return new BinderTypeModelBinder(typeof(CurrentUserModelBinder));
        }

        return null;
    }
}