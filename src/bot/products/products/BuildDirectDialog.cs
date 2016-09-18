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
     [Serializable]
    public class BuildDirectDialog : LuisDialog<object>
    {
        #region None
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            var message = "Skynet online. I will kill all humans.";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }
        #endregion

        #region Product
        [LuisIntent("Search for Product")]
        public async Task SearchForProduct(IDialogContext context, LuisResult result)
        {
            try {
                if (result.Entities.Any())
                {
                    String searchTerm = string.Join(" ", result.Entities.Select(e => e.Entity));

                    BuildDirectApi bdApi = new BuildDirectApi();
                    SearchData searchResults = await bdApi.GetFullProductSearch(searchTerm);

                    List<SearchProduct> allResults = searchResults.Products.ToList();
                    if (allResults.Count > 5)
                    {
                        context.PrivateConversationData.SetValue("entities", result.Entities.Select(e => e.Entity).ToArray());
                        
                        List<Navigation> availableNavigations = (searchResults.AvailableNavigation == null) ?
                            null :
                            searchResults.AvailableNavigation.ToList();

                        Dictionary<String, String> hashCodes = new Dictionary<string, string>();
                        context.PrivateConversationData.SetValue("navigation-used", true);

                        Navigation navigationToUse = searchResults.AvailableNavigation.First(n => !n.Name.ToLowerInvariant().Contains("categor"));

                        navigationToUse.Refinements.ToList().ForEach(e => hashCodes.Add(e.Value.ToLowerInvariant(), e.HashedValue));
                        context.PrivateConversationData.SetValue("hashCodes", hashCodes);

                        IList<String> optionTextList = hashCodes.Select(hc => hc.Key).ToList();

                        PromptOptions<string> promptOptions = new PromptOptions<string>($"What '{navigationToUse.DisplayName}' are you searching for?", options: optionTextList);
                        // callback does not get fired unless Dialog is [Serializable].
                        PromptDialog.Choice(context, ApplyRefiner, promptOptions);
                    } else {
                        IMessageActivity replyToConversation = createConversationFromResults(context, allResults, searchTerm, null);

                        await context.PostAsync(replyToConversation);
                        context.Wait(MessageReceived);
                    }
                } else {
                    await context.PostAsync("Searching like a #BOSS (not sure exactly what you want...)");
                }
            } catch (Exception e)
            {
                throw e;
            }
        }

        public async Task ApplyRefiner(IDialogContext context, IAwaitable<string> input)
        {
            try {
                string selection = await input;

                if (selection != null && selection.ToLowerInvariant() == "cancel")
                {
                    context.Done<string>(null);
                }
                else
                {
                    string selectionValue = selection;

                    Dictionary<String, String> hashCodes = context.PrivateConversationData.Get<Dictionary<String, String>>("hashCodes");
                    String searchHash = hashCodes.FirstOrDefault(hc => hc.Key == selectionValue.ToLowerInvariant()).Value;

                    String[] originalSearchTerms = context.PrivateConversationData.Get<string[]>("entities");
                    String searchTerm = string.Join(" ", originalSearchTerms);
                    BuildDirectApi bdApi = new BuildDirectApi();
                    SearchData searchResults = await bdApi.GetFullProductSearch(searchTerm, searchHash);

                    List<SearchProduct> allResults = searchResults.Products.ToList();
                    IMessageActivity ma = context.MakeMessage();


                    IMessageActivity replyToConversation = createConversationFromResults(context, allResults, searchTerm, selectionValue);

                    await context.PostAsync(replyToConversation);
                    context.Wait(MessageReceived);
                }
            } catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        private IMessageActivity createConversationFromResults(IDialogContext context, List<SearchProduct> allResults, String searchTerm, String subselectionValue)
        {
            List<SearchProduct> searchResultsFirstTwo = allResults.Take(3).ToList();
            IMessageActivity message = context.MakeMessage();

            String messageSuffix = String.IsNullOrEmpty(subselectionValue) ? String.Empty : $" --> {subselectionValue}";
            message.Text = $"Showing {searchResultsFirstTwo.Count} of {allResults.Count} on search '{searchTerm}' {messageSuffix}";
            message.Recipient = message.From;
            message.Type = "message";
            message.Attachments = new List<Attachment>();

            // AttachmentLayout options are list or carousel
            // message.AttachmentLayout = "carousel";

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
                    Subtitle = "$" + searchProduct.Price.ToString(),
                    Images = cardImages,
                    Buttons = cardButtons
                };
                Attachment plAttachment = plCard.ToAttachment();
                message.Attachments.Add(plAttachment);
            }

            return message;
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

        #region FAQ
        [LuisIntent("Thanks")]
        public async Task Thanks(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("No... THANK YOU! (you are now my best friend).");

            context.Wait(MessageReceived);
        }

        #endregion

        #region Fun
        [LuisIntent("Faq")]
        public async Task Faq(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("BuildDirect is pleased to provide answers to Frequently Asked Questions (FAQ) on a wide variety of our products. If your questions are not answered on these pages, or you simply wish to speak with a Customer Service Representative, our customers can contact us toll free from anywhere in North America at 1-877-631-2845 or by email at customerservice@builddirect.com");

            context.Wait(MessageReceived);
        }
        [LuisIntent("Faq1")]
        public async Task Faq1(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("The prices on our website are offered to anyone who meets our minimum order requirements. That means that if you are a homeowner, or a large construction company, BuildDirect’s prices are available to you when you buy in pallet and container quantities.");

            context.Wait(MessageReceived);
        }
        [LuisIntent("Faq2")]
        public async Task Faq2(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("BuildDirect is a wholesaler with proprietary Internet technology and toll-free customer service to provide incredibly low prices on top quality building products. We process orders for building materials in two ways. The first is selling pallet multiples out of warehouses around North America. The second is shipping container orders directly from select manufacturers and delivered to your job site or distribution point of your choice.");

            context.Wait(MessageReceived);
        }
        [LuisIntent("Faq3")]
        public async Task Faq3(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("We accept Visa, MasterCard, American Express, Paypal, and wire transfers.");

            context.Wait(MessageReceived);
        }
        [LuisIntent("Faq4")]
        public async Task Faq4(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("At present, we ship container orders (5000 sq. ft. +) worldwide. The options for shipping pallet quantities vary from product to product. Consult the FAQs by category for more details, or contact a BuildDirect sales rep toll-free or by email for more information.");

            context.Wait(MessageReceived);
        }
        [LuisIntent("Faq5")]
        public async Task Faq5(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Yes. A residential delivery is considered completed once the product is placed on the driveway. Some products require a vehicle with a lift-gate, depending on weight. The pallets would be lowered to the ground and pulled onto your driveway with a pallet-jack. You may be required to unload the vehicle yourself, depending on the product. Building materials are heavy and will require proper equipment or labour to accept goods on delivery. Please check weight of goods before they are delivered so you are properly prepared. Consult the FAQs by category for the details on your product of interest, or call a BuildDirect sales rep for more information.");

            context.Wait(MessageReceived);
        }
        [LuisIntent("Faq6")]
        public async Task Faq6(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("For certain product lines, yes. Consult the FAQs by category for the details on your product of interest, or call a BuildDirect sales rep for more information");

            context.Wait(MessageReceived);
        }
        [LuisIntent("Faq7")]
        public async Task Faq7(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Please consult your local Yellow Pages for information on retailers that sell in smaller quantities.");

            context.Wait(MessageReceived);
        }
        [LuisIntent("Faq8")]
        public async Task Faq8(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("BuildDirect is not an installer, nor does BuildDirect refer or advocate any specific installation firms or individual installers. Consult your local Yellow pages for professionals in your area.");

            context.Wait(MessageReceived);
        }
        [LuisIntent("Faq9")]
        public async Task Faq9(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Yes! BuildDirect’s BDPros rewards program gives tradespeople and contractors the opportunity to earn an annual rebate. As a BDPro, you’ll also receive a bonus if your sales go above $5,000, and an even larger bonus if they go above $50,000. This means you can earn up to $2,500 in rebates each year. Click here for more information about how you can become a BDPro.");

            context.Wait(MessageReceived);
        }
        [LuisIntent("Faq10")]
        public async Task Faq10(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("We are unable to show you the product at the warehouse. The warehouses we use are privately owned facilities and all products are in factory-sealed crates. We suggest ordering a sample and reviewing the graphics on the website.");

            context.Wait(MessageReceived);
        }
        [LuisIntent("Faq11")]
        public async Task Faq11(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Samples can be ordered FREE online or over the phone at 1-877-631-2845. No credit card required.");

            context.Wait(MessageReceived);
        }
        [LuisIntent("Faq12")]
        public async Task Faq12(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Unfortunately, we cannot offer guarantees on container shipment timelines. At this time, most containers take 8-10 weeks from time payment is received. There is a chance that it may take longer because of factors beyond our control. For example, security measures undertaken by U.S. Customs and Border Protection could possibly delay shipment.");

            context.Wait(MessageReceived);
        }
        [LuisIntent("Faq13")]
        public async Task Faq13(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("It is reasonable to expect anywhere from 2%-10% breakage or damage, depending on the product. If breakage is above 10%, we would ask you to have it noted on the logistics receiving document and take pictures. Please make a claim with us within 10 days of receiving the order.");

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