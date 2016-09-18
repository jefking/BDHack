namespace products
{
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Builder.Luis.Models;
    using Microsoft.Bot.Connector;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [LuisModel("229c49a2-d6ce-4e33-9bd1-e0e5a942dd6e", "83df26914f4f4499be8b48456a9d1ed5")]
    public class BuildDirectDialog : LuisDialog<object>
    {
        protected override async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            _message = (Activity)await item;
            await base.MessageReceived(context, item);
        }

        [field: NonSerialized()]
        private Activity _message;

        #region None
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            var message = "Hi John Conner... Skynet online. I will kill all humans.";
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
               
                String searchTerm = string.Join(" ", result.Entities.Select(e => e.Entity));

                //PromptOptions<string> promptOptions = new PromptOptions<string>($"What are you searching for?", options: list);
                //PromptDialog.Choice(context, ApplyRefiner, promptOptions);
                
                BuildDirectApi bdApi = new BuildDirectApi();
                IEnumerable<SearchProduct> searchResults = await bdApi.GetProducts(searchTerm);
                List<SearchProduct> allResults = searchResults.ToList();
                List<SearchProduct> searchResultsFirstTwo = searchResults.Take(2).ToList();

                Activity replyToConversation = _message.CreateReply($"Showing {searchResultsFirstTwo.Count} of {allResults.Count} on search '{searchTerm}'.");
                replyToConversation.Recipient = _message.From;
                replyToConversation.Type = "message";
                replyToConversation.Attachments = new List<Attachment>();

                // AttachmentLayout options are list or carousel
                // yucky replyToConversation.AttachmentLayout = "carousel";

                foreach (SearchProduct searchProduct in searchResultsFirstTwo)
                {    
                    List<CardImage> cardImages = new List<CardImage>();
                    cardImages.Add(new CardImage(url: searchProduct.Image));
                    List<CardAction> cardButtons = new List<CardAction>();
                    CardAction plButton = new CardAction()
                    {
                        Value = searchProduct.Url,
                        Type = "openUrl",
                        Title = searchProduct.Title
                    };
                    cardButtons.Add(plButton);
                    ThumbnailCard plCard = new ThumbnailCard()
                    {
                        Title = searchProduct.Title,
                        Subtitle = searchProduct.Brand.Name,
                        Images = cardImages,
                        Buttons = cardButtons
                    };
                    Attachment plAttachment = plCard.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment);
                }

                await context.PostAsync(replyToConversation);
                context.Wait(MessageReceived);
                
            } else {
                await context.PostAsync("Searching like a #BOSS (not sure exactly what you want...)");
            }
            
            //context.Wait(MessageReceived);
        }

        public async Task ApplyRefiner(IDialogContext context, IAwaitable<string> input)
        {
            string selection = await input;

            if (selection != null && selection.ToLowerInvariant() == "cancel")
            {
                context.Done<string>(null);
            }
            else
            {
                string value = ParseRefinerValue(selection);

                //if (this.queryBuilder != null)
                //{
                //    this.queryBuilder.Refinements.Add(this.refiner, new string[] { value });
                //}

                context.Done(value);
            }
        }
        #endregion

        protected virtual string ParseRefinerValue(string value)
        {
            return value.Substring(0, value.LastIndexOf('(') - 1);
        }
        public async Task AfterConfirming_ProductSearch(IDialogContext context, IAwaitable<bool> confirmation)
        {
            try {
                
                if (await confirmation)
                {
                    await context.PostAsync($"Ok, searching...");
                }
                else
                {
                    await context.PostAsync("Sorry, I couldn't figure out what you wanted - try again.");
                }
                context.Wait(MessageReceived);
            } catch (Exception e)
            {
                throw e;
            }
        }

        #region Support
        [LuisIntent("Get Support")]
        public async Task GetSupport(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Please give us a call and we can assist you: 1-877-631-2845.");

            context.Wait(MessageReceived);
        }
        #endregion

        #region Fun
        [LuisIntent("Thanks")]
        public async Task Thanks(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("No... THANK YOU! (you are now my best friend).");

            context.Wait(MessageReceived);
        }

        [LuisIntent("TechnicalSupport")]
        public async Task TechnicalSupport(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Check out our developer portal: https://developer.builddirect.io/");

            context.Wait(MessageReceived);
        }


        [LuisIntent("PooPoo")]
        public async Task PooPoo(IDialogContext context, LuisResult result)
        {
            result = new LuisResult
            {
                Entities = new List<EntityRecommendation>(),
            };
            var item = new EntityRecommendation
            {
                Entity = "Toilet",
            };

            result.Entities.Add(item);

            await this.SearchForProduct(context, result);
        }
        #endregion
    }
}