using System;
using System.Collections;
using System.Collections.Generic;

namespace OpenRasta.OperationModel.Interceptors
{
    public class OperationWithInterceptors : IOperation
    {
        readonly IEnumerable<IOperationInterceptor> _interceptors;
        readonly IOperation _wrappedOperation;

        public OperationWithInterceptors(IOperation wrappedOperation, IEnumerable<IOperationInterceptor> systemInterceptors)
        {
            _wrappedOperation = wrappedOperation;
            _interceptors = systemInterceptors;
        }

        public IDictionary ExtendedProperties
        {
            get { return _wrappedOperation.ExtendedProperties; }
        }

        public IEnumerable<InputMember> Inputs
        {
            get { return _wrappedOperation.Inputs; }
        }

        public string Name
        {
            get { return _wrappedOperation.Name; }
        }

        public T FindAttribute<T>() where T : class
        {
            return _wrappedOperation.FindAttribute<T>();
        }

        public IEnumerable<T> FindAttributes<T>() where T : class
        {
            return _wrappedOperation.FindAttributes<T>();
        }

        public IEnumerable<OutputMember> Invoke()
        {
            ExecutePreConditions();

            Func<IEnumerable<OutputMember>> operation = () => _wrappedOperation.Invoke();
            foreach (var executingCondition in _interceptors)
            {
                operation = executingCondition.RewriteOperation(operation) ?? operation;
            }
            var results = operation();

            ExecutePostConditions(results);
            return results;
        }

        void ExecutePostConditions(IEnumerable<OutputMember> results)
        {
            foreach (var postCondition in _interceptors)
            {
                TryExecute(() => postCondition.AfterExecute(_wrappedOperation, results), "The interceptor {0} stopped execution.".With(postCondition.GetType().Name));
            }
        }

        void ExecutePreConditions()
        {
            foreach (var precondition in _interceptors)
            {
                TryExecute(() => precondition.BeforeExecute(_wrappedOperation), "The interceptor {0} stopped execution.".With(precondition.GetType().Name));
            }
        }

        void TryExecute(Func<bool> interception, string exceptionMessage)
        {
            Exception exception = null;
            try
            {
                bool isSuccessful = interception();
                if (!isSuccessful)
                {
                    exception = new InterceptorException(exceptionMessage);
                }
            }
            catch (Exception e)
            {
                exception = new InterceptorException(exceptionMessage, e);
            }
            if (exception != null)
            {
                throw exception;
            }
        }
    }
}