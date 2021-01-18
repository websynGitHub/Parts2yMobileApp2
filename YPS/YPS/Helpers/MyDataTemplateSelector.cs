using Xamarin.Forms;
using YPS.ViewModels;
using YPS.Views;
using YPS.Views.Menu;
using YPS.CommonClasses;
namespace YPS.Helpers
{
    class MyDataTemplateSelector : Xamarin.Forms.DataTemplateSelector
    {
        public MyDataTemplateSelector()
        {
            // Retain instances!
            this.incomingDataTemplate = new DataTemplate(typeof(IncomingViewCell));
            this.outgoingDataTemplate = new DataTemplate(typeof(OutgoingViewCell));
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var messageVm = item as ChatMessageViewModel;

            if (messageVm.Message != null)
            {
                Settings.Text = messageVm.Message;
                Settings.Image = null;
                Settings.ChatDocument = null;
            }

            if (messageVm.Image != null)
            {
                Settings.Image = messageVm.Image;
                Settings.Text = null;
                Settings.ChatDocument = null;
            }

            if(messageVm.Document != null)
            {
                Settings.ChatDocument = messageVm.Document;
                Settings.Text = null;
                Settings.Image = null;
            }

            if (messageVm == null)
            {
                return null;
            }
            return messageVm.IsMine ? this.incomingDataTemplate : this.outgoingDataTemplate;
        }

        private readonly DataTemplate incomingDataTemplate;
        private readonly DataTemplate outgoingDataTemplate;
    }
}
