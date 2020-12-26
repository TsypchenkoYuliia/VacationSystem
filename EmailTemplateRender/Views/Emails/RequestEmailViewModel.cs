using EmailModels.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmailTemplateRender.Views.Emails
{
    public class RequestEmailViewModel
    {
        public RequestEmailViewModel(RequestDataForEmail data, string approveUrl = null, string rejectUrl = null)
        {
            Data = data;
            ApproveUrl = approveUrl;
            RejecteUrl = rejectUrl;
        }

        public string ApproveUrl { get; set; }
        public string RejecteUrl { get; set; }
        public RequestDataForEmail Data { get; set; }
    }
}
