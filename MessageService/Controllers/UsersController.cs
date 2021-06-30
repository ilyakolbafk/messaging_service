using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MessageService.Models;

namespace MessageService.Controllers
{
    /// <summary>
    /// Controller of all requests.
    /// </summary>
    [Route("users")]
    public class UsersController : Controller
    {
        // List of users.
        private static List<User> _users;
        // List of messages.
        private static List<Msg> _messages;
        
        // For generating messages.
        private static readonly Random Rnd = new();

        // Load users and messages from files.
        static UsersController()
        {
            _users = Models.User.LoadUsers();
            _messages = Msg.LoadMessages();
        }

        /// <summary>
        /// Request with optional parameters which show a list of users.
        /// </summary>
        /// <param name="limit"> Maximum number of users to display. </param>
        /// <param name="offset"> Index of the first user. </param>
        [HttpGet]
        public ActionResult<IEnumerable<User>> Get([FromQuery] int limit = int.MaxValue, int offset = 0)
        {
            _users.Sort();
            if (limit <= 0 || offset < 0)
                return BadRequest(
                    new {Message = "Limit должен быть положительным и Offset должен быть неотрицательным"});
            return Ok(_users.Skip(offset).Take(limit));
        }

        /// <summary>
        /// Request that includes the user's email address which show that user.
        /// </summary>
        [HttpGet("{eMail}")]
        public ActionResult Get(string eMail)
        {
            var user = _users.FirstOrDefault(x => x.EMail == eMail);
            if (user == null)
                return NotFound(new {Message = $"Пользователь с eMail = {eMail} не найден"});
            return Ok(user);
        }

        /// <summary>
        /// Request with optional parameters which show a list of messages.
        /// </summary>
        /// <param name="sender"> Message sender. </param>
        /// <param name="receiver"> Message receiver. </param>
        [HttpGet("messages")]
        public ActionResult Get([FromQuery] string sender, string receiver)
        {
            if (sender == null && receiver == null)
                return Ok(_messages);
            if (sender == null)
                return Ok(_messages.Where(x => x.ReceiverId == receiver));
            if (receiver == null)
                return Ok(_messages.Where(x => x.SenderId == sender));
            return Ok(_messages.Where(x => x.SenderId == sender && x.ReceiverId == receiver));
        }

        /// <summary>
        /// Initial request that creates a list of 100 users and 1000 messages.
        /// </summary>
        [HttpPost("create")]
        public ActionResult Post()
        {
            if (_users.Count != 0)
                return BadRequest();
            Initialization();
            Msg.SaveMessages(_messages);
            Models.User.SaveUsers(_users);
            return Ok();
        }

        /// <summary>
        /// Request that creates a new user based on the data entered in json format.
        /// </summary>
        [HttpPost("create-user")]
        public IActionResult CreateUser([FromQuery] CreateUserRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.UserName) || string.IsNullOrWhiteSpace(req.EMail))
                return BadRequest(new {Message = "У пользователя должно быть и UserName, и EMail"});
            if (_users.Any(x => x.EMail == req.EMail))
                return BadRequest(new {Message = "Пользователь с таким EMail уже зарегистрирован"});
            _users.Add(new User {UserName = req.UserName, EMail = req.EMail});
            Models.User.SaveUsers(_users);
            return Ok(_users[^1]);
        }

        /// <summary>
        /// Request that creates a new message based on the data entered in json format.
        /// </summary>
        [HttpPost("create-message")]
        public IActionResult CreateMessage([FromQuery] CreateMessageRequest req)
        {
            if (req.Subject == null || req.Message == null ||
                string.IsNullOrWhiteSpace(req.ReceiverId) || string.IsNullOrWhiteSpace(req.SenderId))
                return BadRequest(new
                    {Message = "Сообщение не имеет темы, или тела, или отправитель или получатель не указан"});
            if (_users.All(x => x.EMail != req.SenderId))
                return BadRequest(new {Message = "Отправитель не зарегистрирован."});
            if (_users.All(x => x.EMail != req.ReceiverId))
                return BadRequest(new {Message = "Получатель не зарегистрирован."});
            _messages.Add(new Msg
                {Message = req.Message, Subject = req.Subject, SenderId = req.SenderId, ReceiverId = req.ReceiverId});
            Msg.SaveMessages(_messages);
            return Ok(_messages[^1]);
        }

        /// <summary>
        /// Create a list of 100 users and 1000 messages.
        /// </summary>
        private static void Initialization()
        {
            for (var i = 0; i < 100; i++)
            {
                string eMail;
                do eMail = Models.User.GenerateEMail();
                while (_users.Any(x => x.EMail == eMail));
                _users.Add(new User {UserName = Models.User.GenerateUserName(), EMail = eMail});
            }

            for (var i = 0; i < 1000; i++)
                _messages.Add(new Msg
                {
                    SenderId = _users[Rnd.Next(0, _users.Count)].EMail,
                    ReceiverId = _users[Rnd.Next(0, _users.Count)].EMail,
                    Subject = Msg.RussiansWords[Rnd.Next(0, Msg.RussiansWords.Count)],
                    Message = Msg.GenerateMessage()
                });
        }
    }
}