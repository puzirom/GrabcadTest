using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Microsoft.AspNet.SignalR;
using MathCompetition.Models;

namespace MathCompetition.Hubs
{
    public class GameHub : Hub
    {
        #region Private Variables and Costants

        private const int MaxCount = 10;
        private const int TimeInterval = 5000;
        private static Timer _timer;
        private static readonly object Locker = new object();
        private static readonly List<Competitor> Competitors = new List<Competitor>();

        #endregion

        /// <summary>
        /// Method is used in Util.js
        /// </summary>
        /// <param name="userName"></param>
        public void Connect(string userName)
        {
            lock (Locker)
            {
                if (Competitors.Count + 1 > MaxCount)
                {
                    Clients.Caller.onMaxCount(MaxCount);
                    return;
                }

                if (Competitors.Any(x => string.Equals(x.Name, userName, StringComparison.OrdinalIgnoreCase)))
                {
                    Clients.Caller.onAlreadyPresent(userName);
                    return;
                }

                if (Competitors.Any(x => x.Id == Context.ConnectionId))
                {
                    return;
                }

                var newCompetitor = Competitor.NewCompetitor(Context.ConnectionId, userName);
                Competitors.Add(newCompetitor);
                Clients.Caller.onUserConnected(userName, Competitors);
                Clients.AllExcept(Context.ConnectionId).onUpdateUsers(Competitors);
                SendCurrentQuestion();
            }
        }

        /// <summary>
        /// Method is used in Util.js
        /// </summary>
        /// <param name="questionId"></param>
        /// <param name="answer"></param>
        public void SendResponse(string questionId, string answer)
        {
            lock (Locker)
            {
                var item = Competitors.SingleOrDefault(x => x.Id == Context.ConnectionId);
                if (item == null)
                {
                    return;
                }

                var result = Mathematics.Instance.CompareResult(questionId, answer);
                if (result == AnswerResult.Correct)
                {
                    item.PointsUp();
                    Clients.All.onRoundFinish(item.Name);
                    StartCompetition();
                }
                if (result == AnswerResult.Incorrect)
                {
                    item.PointsDown();
                    Clients.Caller.onIncorrect(answer);
                }
                if (result == AnswerResult.Late)
                {
                    return;
                }
                Clients.All.onUpdateUsers(Competitors);
            }
        }

        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            lock (Locker)
            {
                var item = Competitors.SingleOrDefault(x => x.Id == Context.ConnectionId);
                if (item != null)
                {

                    Competitors.Remove(item);
                    if (Competitors.Count == 0) StopCompetition();
                    Clients.All.onUserDisconnected(Context.ConnectionId);
                }

                return base.OnDisconnected(stopCalled);
            }
        }

        #region Private Methods

        private void StartCompetition()
        {
            // prepare and run the timer
            if (_timer == null)
            {
                _timer = new Timer { AutoReset = true, Interval = TimeInterval };
                _timer.Elapsed += SendNewQuestion;
            }
            _timer.Start();
        }

        private static void StopCompetition()
        {
            // dispose the timer
            if (_timer == null) return;
            _timer.Stop();
            _timer.Dispose();
        }

        private void SendNewQuestion(object source, ElapsedEventArgs e)
        {
            // is launched by timer 
            _timer?.Stop();
            var mathInst = Mathematics.Instance;
            mathInst.GenerateNew();
            Clients.All.onNewQuestion(mathInst.Id, mathInst.Text, mathInst.Answers);
        }

        private void SendCurrentQuestion()
        {
            // send existing question (if it is not empty) or new one
            var mathInst = Mathematics.Instance;
            if (mathInst.IsEmpty)
            {
                mathInst.GenerateNew();
            }
            Clients.Caller.onNewQuestion(mathInst.Id, mathInst.Text, mathInst.Answers);
        }

        #endregion
    }
}