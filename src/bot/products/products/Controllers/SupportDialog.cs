namespace products.Controllers
{
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Builder.Luis.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;

    [LuisModel("229c49a2-d6ce-4e33-9bd1-e0e5a942dd6e", "83df26914f4f4499be8b48456a9d1ed5")]
    public class SupportDialog : LuisDialog<object>
    {
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            var message = "Please call us: 1-877-631-2845";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("Get Support")]
        public async Task SearchForProduct(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("We know you want support but we are unable to help you at this time?");

            context.Wait(MessageReceived);
        }
    }
}