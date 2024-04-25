﻿using Microsoft.EntityFrameworkCore;
using MyCode_Backend_Server.Data;
using MyCode_Backend_Server.Models;

namespace MyCode_Backend_Server.Service.Bot
{
    public class FAQBot(DataContext dataContext)
    {
        private readonly DataContext _dataContext = dataContext;

        public async Task<string> OnMessageActivityAsync(BotMessage message)
        {
            if (message.Text == null)
            {
                return "Sorry, I couldn't understand your message.";
            }

            var keywords = GetKeywordsFromMessage(message.Text);

            if (keywords.Count == 0 || keywords == null)
            {
                return "Please be more specific!";
            }
            var result = await SearchFAQByKeywords(keywords);

            return result;
        }

        private static List<string> GetKeywordsFromMessage(string message)
        {
            var keywords = new List<string>();

            var words = message.Split(' ');

            foreach (var word in words)
            {
                if (!string.IsNullOrWhiteSpace(word))
                {
                    var keywordWithoutPunctuation = new string(word.Where(c => !char.IsPunctuation(c) && c != '\'' && c != '-').ToArray()).ToLower();

                    keywords.Add(keywordWithoutPunctuation);
                }
            }

            return keywords;
        }

        private async Task<string> SearchFAQByKeywords(List<string> keywords)
        {
            string defaultAnswer = "Sorry, I couldn't find an answer related to your question.";
            string bestMatchAnswer = defaultAnswer;
            int maxMatchCount = 0;

            var matchingFAQs = await _dataContext.BotDb!.ToListAsync();

            foreach (var faq in matchingFAQs)
            {
                var matchCount = 0;
                foreach (var keyword in keywords)
                {
                    if (faq.Question!.Contains(keyword, StringComparison.CurrentCultureIgnoreCase))
                    {
                        matchCount++;
                    }
                }

                if (matchCount > maxMatchCount)
                {
                    maxMatchCount = matchCount;
                    bestMatchAnswer = faq.Answer!;
                }
            }

            return bestMatchAnswer;
        }
    }
}
