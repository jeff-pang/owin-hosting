﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Owin.Builder.Tests.Middlewares
{
    public class StandardBehavior
    {
        public static Task<ResultParameters> Execute(CallParameters call, AppDelegate app, string arg1, string arg2)
        {
            AddValue(call.Environment, "arg1", arg1);
            return app.Invoke(call)
                .Then(result =>
                {
                    AddValue(call.Environment, "arg2", arg2);
                    AddValue(result.Properties, "arg2", arg2);
                    return result;
                });
        }

        static void AddValue(IDictionary<string, object> dictionary, string key, string value)
        {
            object existing;
            dictionary.TryGetValue(key, out existing);
            dictionary[key] = existing + value;
        }
    }

    public class Alpha
    {
        public AppDelegate Invoke(AppDelegate app, string arg1, string arg2)
        {
            return call => StandardBehavior.Execute(call, app, arg1, arg2);
        }
    }

    public class Beta
    {
        readonly AppDelegate _app;
        readonly string _arg1;
        readonly string _arg2;

        public Beta(AppDelegate app, string arg1, string arg2)
        {
            _app = app;
            _arg1 = arg1;
            _arg2 = arg2;
        }

        public Task<ResultParameters> Invoke(CallParameters call)
        {
            return StandardBehavior.Execute(call, _app, _arg1, _arg2);
        }
    }

    public class Gamma
    {
        public Beta Middleware(AppDelegate app, string arg1, string arg2)
        {
            return new Beta(app, arg1, arg2);
        }
    }
}
