using System;
using TamaChanBot.API.Events;

namespace TamaChanBot.API.Responses
{
    public abstract class MenuHandlerObject<TResult> : IMenuHandlerObject
    {
        protected readonly string title;
        protected readonly string description;

        public object data;

        public ResponseHandler<TResult> responseHandler;

        public delegate object ResponseHandler<T>(T receivedResponse, MessageContext context, object data);

        public string Title { get => title; }
        public string Description { get => description; }
        public bool DeleteAfterResponse { get; set; }
        public ResponseSentArgs ResponseSentArgs { get; set; }

        public MenuHandlerObject(string title, ResponseHandler<TResult> responseHandler, string description = "")
        {
            this.title = title;
            this.description = description;
            this.responseHandler = responseHandler;
        }

        public abstract bool HandleMenuOperation(out object response, MessageContext messageContext);
    }

    public interface IMenuHandlerObject
    {
        string Title { get; }
        string Description { get; }
        bool DeleteAfterResponse { get; }
        ResponseSentArgs ResponseSentArgs { get; set; }
        bool HandleMenuOperation(out object response, MessageContext messageContext);
    }
}
