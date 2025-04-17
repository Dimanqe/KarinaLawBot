using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using KarinaLawBot.Models;

namespace KarinaLawBot.Services
{
    public class MemoryStorage : IStorage
    {
        private readonly ConcurrentDictionary<long, Session> _sessions;

        public MemoryStorage()
        {
            _sessions = new ConcurrentDictionary<long, Session>();
        }

        public Session GetSession(long chatId)
        {
            if (_sessions.TryGetValue(chatId, out var session))
                return session;

            var newSession = new Session();
            _sessions.TryAdd(chatId, newSession);
            return newSession;
        }

        public void UpdateSession(long chatId, Action<Session> updateAction)
        {
            var session = GetSession(chatId);
            updateAction(session);
            _sessions[chatId] = session;
        }
    }
}