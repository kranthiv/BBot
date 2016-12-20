using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using BBot.Models;
using Microsoft.Bot.Builder.FormFlow;
using BBot.QNA;

namespace BBot.Dialogs
{
    [Serializable]
    [LuisModel("95cac89b-e6a7-4c00-8ddd-93d05d36ea19", "67d4426865094c8c81abf3c927ec7814")]
    public class RootLuisDialog : LuisDialog<object>
    {
        #region Entities
        private const string EntityLoanSuspension = "loan suspension";
        private const string EntityHelp = "Help";
        private const string EntityLoanNumber = "Loan Number";
        private const string EntityService = "Service";
        #endregion

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry, I did not understand '{result.Query}'. Type 'help' if you need assistance.";

            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Contact SLS")]
        public async Task Contact(IDialogContext context, LuisResult result)
        {

        }

        [LuisIntent("FAQS")]
        public async Task FAQS(IDialogContext context, LuisResult result)
        {
            QNAService qNa = new QNAService();
            QNAResponse reponse  = await qNa.GetAnswerAsync(result.Query);
            await context.PostAsync(reponse.Answer);
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("suspend payments")]
        public async Task SLSServices(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;
            await context.PostAsync($"Welcome to sls services!! we are analyzing your message: `{message.Text}`");
            var query = new LoanSuspendQuery();
            EntityRecommendation loanEntity;
            if (result.TryFindEntity(EntityLoanSuspension, out loanEntity))
            {
                loanEntity.Type = "LoanNumber";
            }
            var formDialog = new FormDialog<LoanSuspendQuery>(query, this.BuildForm, FormOptions.PromptInStart, result.Entities);
            context.Call(formDialog, this.ResumeAfterFormDialog);
        }


        private IForm<LoanSuspendQuery> BuildForm()
        {
            OnCompletionAsyncDelegate<LoanSuspendQuery> processLoanSuspension = async (context, state) =>
            {
                var message = "Suspending payments for loan";
                if (!string.IsNullOrEmpty(state.LoanNumber))
                {
                    message += $"  {state.LoanNumber}...";
                }


                await context.PostAsync(message);
            };

            return new FormBuilder<LoanSuspendQuery>()
                .Field(nameof(LoanSuspendQuery.LoanNumber), (state) => string.IsNullOrEmpty(state.LoanNumber))
                .OnCompletion(processLoanSuspension)
                .Build();
        }


        private async Task ResumeAfterFormDialog(IDialogContext context, IAwaitable<LoanSuspendQuery> result)
        {
            try
            {
                var searchQuery = await result;

                //Database call to suspend loan payments


                await context.PostAsync($"Suspending payment to the loan initilized...");

                await context.PostAsync("payment to the loan suspended!!");
            }
            catch (FormCanceledException ex)
            {
                string reply;

                if (ex.InnerException == null)
                {
                    reply = "You have canceled the operation.";
                }
                else
                {
                    reply = $"Oops! Something went wrong :( Technical Details: {ex.InnerException.Message}";
                }

                await context.PostAsync(reply);
            }
            finally
            {
                context.Done<object>(null);
            }
        }
    }


}