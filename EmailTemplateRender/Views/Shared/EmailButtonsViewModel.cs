namespace EmailTemplateRender.Views.Shared
{
    public class EmailButtonsViewModel
    {
        public EmailButtonsViewModel(string approveUrl)
        {
            ApproveUrl = approveUrl;
            
        }

        public string ApproveUrl { get; set; }
    }
}
