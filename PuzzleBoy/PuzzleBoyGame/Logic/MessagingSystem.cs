using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PuzzleBoy
{
    public enum MessageType
    {
        MSG_PLAYER_STARTED_MOVING,
        MSG_PLAYER_MOVED,
        MSG_ENEMY_KILLED,
        MSG_PLAYER_DIED,
        MSG_ITEM_COLLECTED
    }

    class Message
    {
        public MessageType _type;

        public Message(MessageType type)
        {
            _type = type;
        }
    }

    class MessageSystem
    {
        private static MessageSystem instance = null;
        public static MessageSystem _MessageSystem
        {
            get
            {
                if (instance == null)
                    instance = new MessageSystem();
                return instance;
            }
        }

        public delegate void MessageHandler(String param);

        public class SubscriberList : List<MessageHandler> { }

        Dictionary<MessageType, SubscriberList> _subscriptions = new Dictionary<MessageType, SubscriberList>();

        public void subscribe(MessageType msgType, MessageHandler handler)
        {
            if (!_subscriptions.ContainsKey(msgType))
            {
                _subscriptions.Add(msgType, new SubscriberList());
            }
            _subscriptions[msgType].Add(handler);
        }

        public void unsubscribe(MessageType msgType)
        {
            if (_subscriptions.ContainsKey(msgType))
            {
                foreach (MessageHandler handler in _subscriptions[msgType])
                {
                }
            }
        }

        public void sendMessage(MessageType msgType, String param)
        {
            if (_subscriptions.ContainsKey(msgType))
            {
                foreach (MessageHandler handler in _subscriptions[msgType])
                {
                    handler(param);
                }   
            }
        }
    }
}
