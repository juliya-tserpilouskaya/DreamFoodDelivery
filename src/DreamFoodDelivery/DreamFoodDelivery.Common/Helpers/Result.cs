using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Common
{
    public class Result
    {
        protected Result(bool success, bool isError, string message)
        {
            IsSuccess = success;
            IsError = isError;
            Message = message;
        }

        public bool IsSuccess { get; }

        public bool IsError { get; }

        public string Message { get; }

        public static Result Ok()
        {
            return new Result(true, false, null);
        }

        public static Result Warning(string message)
        {
            return new Result(false, false, message);
        }

        public static Result Fail(string message)
        {
            return new Result(false, true, message);
        }
        public static Result NoContent()
        {
            return new Result(false, false, null);
        }
    }

    public class Result<T> : Result where T : class
    {
        protected Result(bool success, bool error, string message, T data) : base(success, error, message)
        {
            Data = data;
        }

        public T Data { get; }

        public static Result<T> Ok<T>(T data) where T : class
        {
            return new Result<T>(true, false, null, data);
        }

        public static Result<T> Warning<T>(T data, string message) where T : class
        {
            return new Result<T>(false, false, message, data);
        }
        public static Result<T> Warning(string message) 
        {
            return new Result<T>(false, false, message, null);
        }

        public static Result<T> Fail<T>(string message) where T : class
        {
            return new Result<T>(false, true, message, null);
        }

        public static Result<T> NoContent<T>() where T : class
        {
            return new Result<T>(false, false, null, null);
        }
    }
}
