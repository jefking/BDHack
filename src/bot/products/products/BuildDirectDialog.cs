﻿namespace products
{
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Builder.Luis.Models;
    using System.Linq;
    using System.Threading.Tasks;

    [LuisModel("229c49a2-d6ce-4e33-9bd1-e0e5a942dd6e", "83df26914f4f4499be8b48456a9d1ed5")]
    public class BuildDirectDialog : LuisDialog<object>
    {
        #region None
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry I did not understand: " + string.Join(", ", result.Intents.Select(i => i.Intent));
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }
        #endregion

        #region Product
        [LuisIntent("Search for Product")]
        public async Task SearchForProduct(IDialogContext context, LuisResult result)
        {
            if (result.Entities.Any())
            {
                string criteria = string.Join(",", result.Entities.Select(e => e.Entity));
                await context.PostAsync($"Searching for '{criteria}'...");
            } else {
                await context.PostAsync("Searching like a #BOSS (not sure exactly what you want...)");
            }
            
            context.Wait(MessageReceived);
        }
        #endregion

        #region Support
        [LuisIntent("Get Support")]
        public async Task GetSupport(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Please give us a call and we can assist you: 1-877-631-2845.");

            context.Wait(MessageReceived);
        }
        #endregion
    }
}