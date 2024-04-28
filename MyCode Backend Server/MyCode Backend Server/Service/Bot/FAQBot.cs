using Microsoft.EntityFrameworkCore;
using MyCode_Backend_Server.Data;
using MyCode_Backend_Server.Models;
using System;

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
            List<string> faqData = [];

            foreach (var faq in matchingFAQs)
            {
                var matchCount = 0;
                foreach (var keyword in keywords)
                {
                    if (faq.Question!.Contains(keyword, StringComparison.CurrentCultureIgnoreCase))
                    {
                        matchCount++;
                        if (!faqData.Contains(keyword))
                        {
                            faqData.Add(keyword);
                        }
                    }
                }

                if (matchCount > maxMatchCount)
                {
                    maxMatchCount = matchCount;
                    bestMatchAnswer = faq.Answer!;
                }
            }

            var result = ScoreAndSelectAnswer(keywords, faqData);

            if (result != null)
            {
                if (bestMatchAnswer != result)
                {
                    bestMatchAnswer = result;
                }
            }

            return bestMatchAnswer;
        }

        private static string ScoreAndSelectAnswer(List<string> keywords, List<string> faqDatabase)
        {
            List<string> bestAnswers = [];
            int maxScore = 0;

            foreach (string faq in faqDatabase)
            {
                int score = 0;
                foreach (string keyword in keywords)
                {
                    if (faq.Contains(keyword, StringComparison.CurrentCultureIgnoreCase))
                    {
                        score++;
                    }
                }

                if (score > maxScore)
                {
                    maxScore = score;
                    bestAnswers.Clear();
                    bestAnswers.Add(faq);
                }
                else if (score == maxScore)
                {
                    bestAnswers.Add(faq);
                }
            }

            if (bestAnswers.Count == 1)
            {
                return bestAnswers[0];
            }
            else
            {
                return null!;
            }
        }
    }
}