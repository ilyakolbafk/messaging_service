using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace MessageService.Models
{
    /// <summary>
    /// Class of users and interactions with them.
    /// </summary>
    public class User : IComparable
    {
        // For generating users.
        private static readonly Random Rnd = new();

        // User username.
        public string UserName { get; init; }

        // User Email.
        public string EMail { get; init; }

        /// <summary>
        /// Creating an email of user from English letters using a email mask.
        /// </summary>
        public static string GenerateEMail()
        {
            var letters = "abcdefghijklmnopqrstuvwxyz".ToList();
            var nameLen = Rnd.Next(5, 16);
            var domainLen = Rnd.Next(3, 8);
            var res = new StringBuilder();
            for (var i = 0; i < nameLen; i++)
                res.Append(letters[Rnd.Next(0, 26)]);
            res.Append('@');
            for (var i = 0; i < domainLen; i++)
                res.Append(letters[Rnd.Next(0, 26)]);
            res.Append(Math.Round(Rnd.NextDouble()) == 0 ? ".com" : ".ru");
            return res.ToString();
        }

        /// <summary>
        /// Creating a username of user from English letters.
        /// </summary>
        public static string GenerateUserName()
        {
            var letters = "abcdefghijklmnopqrstuvwxyz".ToList();
            var res = new StringBuilder();
            for (var i = 0; i < Rnd.Next(5, 16); i++)
                res.Append(letters[Rnd.Next(0, 26)]);
            return res.ToString();
        }

        /// <summary>
        /// Save information about users to a file.
        /// </summary>
        public static void SaveUsers(List<User> users)
        {
            try
            {
                users.Sort();
                File.WriteAllText("users.json", JsonSerializer.Serialize(users));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Load information about users from a file.
        /// </summary>
        public static List<User> LoadUsers()
        {
            try
            {
                return File.Exists("users.json")
                    ? JsonSerializer.Deserialize<List<User>>(File.ReadAllText("users.json"))
                    : new List<User>(200);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return new List<User>(200);
        }

        // Users are sorted by Email.
        public int CompareTo(object obj) => EMail.CompareTo(((User) obj).EMail);

        public override string ToString() => $"User: username: {UserName}, eMail: {EMail}";
    }
}