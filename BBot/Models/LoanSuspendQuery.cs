using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.FormFlow;

namespace BBot.Models
{
    [Serializable]
    public class LoanSuspendQuery
    {
        [Prompt("please enter your {&}")]
        [Optional]
        public string LoanNumber { get; set; }
    }
}