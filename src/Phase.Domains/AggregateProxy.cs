﻿using System;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Threading;

namespace Phase.Domains
{
    /// <summary>
    /// Responsible for controlling the number of threads that can execute against a single aggregate root instance at any given time.
    /// </summary>
    /// <typeparam name="TAggregate">Aggregate root type</typeparam>
    public class AggregateProxy<TAggregate> : RealProxy
        where TAggregate : AggregateRoot
    {
        private readonly TAggregate _aggregate;
        private readonly SemaphoreSlim _pool;
        private readonly IAggregateSynchronizer _sync;

        protected AggregateProxy(TAggregate actor, IAggregateSynchronizer sync)
            : base(typeof(TAggregate))
        {
            _aggregate = actor;
            _sync = sync;
            _pool = new SemaphoreSlim(1, 1);
        }

        public static TAggregate Create(TAggregate actor, IAggregateSynchronizer invoker)
        {
            return (TAggregate)new AggregateProxy<TAggregate>(actor, invoker).GetTransparentProxy();
        }

        public override IMessage Invoke(IMessage msg)
        {
            try
            {
                _pool.Wait();
                var methodCall = (IMethodCallMessage)msg;
                var method = (MethodInfo)methodCall.MethodBase;
                var result = _sync.Invoke(method, _aggregate, methodCall.InArgs);

                return new ReturnMessage(result, null, 0, methodCall.LogicalCallContext, methodCall);
            }
            catch (Exception ex)
            {
                if (ex is TargetInvocationException && ex.InnerException != null)
                    return new ReturnMessage(ex.InnerException, msg as IMethodCallMessage);

                return new ReturnMessage(ex, msg as IMethodCallMessage);
            }
            finally
            {
                _pool.Release();
            }
        }
    }
}