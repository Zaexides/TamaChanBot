using System;

namespace TamaChanBot.API.Responses
{
    public abstract class MenuHandlerObject<TResult> : IMenuHandlerObject
    {
        public readonly string title;
        public readonly string description;

        public object data;

        public ResponseHandler<TResult> responseHandler;

        public delegate object ResponseHandler<T>(T receivedResponse, MessageContext context, object data);

        public string Title { get => title; }
        public string Description { get => description; }

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
        bool HandleMenuOperation(out object response, MessageContext messageContext);
    }
}
