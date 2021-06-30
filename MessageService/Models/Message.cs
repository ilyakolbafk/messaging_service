using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace MessageService.Models
{
    /// <summary>
    /// Class of messages and interactions with them.
    /// </summary>
    public class Msg
    {
        // For generating messages.
        private static readonly Random Rnd = new();

        // Russian nouns for filling messages.
        public static readonly List<string> RussiansWords = File.ReadAllLines("russian.txt").ToList();

        // Message subject.
        public string Subject { get; init; }

        // Message body.
        public string Message { get; init; }

        // Email address of the sender of the message.
        public string SenderId { get; init; }

        // Email address of the receiver of the message.
        public string ReceiverId { get; init; }

        /// <summary>
        /// Create a body of a letter consisting of 10 - 50 Russian nouns.
        /// </summary>
        public static string GenerateMessage()
        {
            var res = new StringBuilder();
            res.Append(RussiansWords[Rnd.Next(0, RussiansWords.Count)]);
            res[0] = Convert.ToChar(res[0].ToString().ToUpper());
            for (var i = 0; i < Rnd.Next(9, 50); i++)
                res.Append(RussiansWords[Rnd.Next(0, RussiansWords.Count)] + " ");
            return res.ToString();
        }

        /// <summary>
        /// Save information about messages to a file.
        /// </summary>
        public static void SaveMessages(List<Msg> messages)
        {
            try
            {
                File.WriteAllText("messages.json", JsonSerializer.Serialize(messages));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Load information about messages from a file.
        /// </summary>
        public static List<Msg> LoadMessages()
        {
            try
            {
                return File.Exists("messages.json")
                    ? JsonSerializer.Deserialize<List<Msg>>(File.ReadAllText("messages.json"))
                    : new List<Msg>(1000);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return new List<Msg>(1000);
        }
    }
}