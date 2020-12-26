using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EmailTemplateRender.Services
{
    public interface IRazorViewToStringRenderer
    {
        Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model);
    }
}
