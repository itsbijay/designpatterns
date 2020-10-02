using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DesignPatterns.Traditional.Command
{
    /*******************************************************************************
     * Command Pattern
     *******************************************************************************
     * What is it?
     * One of original GoF patterns to encapsulate a request into a specific command
     * 
     * Real-world Examples:
     *  - Queue processing (MDM Queue Handler)
     *  - Task execution (such as Control-M to run OS vs SQL commands)
     *  - Undoable operations (redo/undo)
     *  - Thread pools/asynchronous code (Tasks, Func/Action)
     * 
     * Demo:
     * 1. Async processing (Func/Action)
     * 2. Queue handling
     * 
     *******************************************************************************/

    /*******************************************************************************
     * 1. Async processing (Func/Action)
     *******************************************************************************/

    public class Demo
    {
        public async Task DoSomething() { await Task.FromResult(0); }

        public async Task Run()
        {
            var tasks = new List<Task>();
            for (int i = 0; i < 20; i++)
                tasks.Add(Task.Run(() => this.DoSomething())); // Action/Func as "Command"
            await Task.WhenAll(tasks);
        }
    }

    /*******************************************************************************
     * 2. Queue handling
     *******************************************************************************/

    public interface IQueueHandler { string QueueCode { get; } void Execute(Message message); }
    public class Message { public string QueueCode { get; } public string Payload { get; } }// Simple model for payload

    // Handler to create customers
    public class CreateCustomerHandler : IQueueHandler
    {
        public const string QUEUE_CODE = "customer.create";
        public string QueueCode => QUEUE_CODE;
        public void Execute(Message message) { } // Create customer
    }

    // Handler to update customers
    public class UpdateCustomerHandler : IQueueHandler
    {
        public const string QUEUE_CODE = "customer.update";
        public string QueueCode => QUEUE_CODE;
        public void Execute(Message message) { } // Update customer
    }

    // Queue manager
    public class QueueManager
    {
        private Dictionary<string, IQueueHandler> Handlers => _handlers.Value; // Flywheel
        private static Lazy<Dictionary<string, IQueueHandler>> _handlers = new Lazy<Dictionary<string, IQueueHandler>>(()
            => new Dictionary<string, IQueueHandler>
            {
                { CreateCustomerHandler.QUEUE_CODE, new CreateCustomerHandler() },
                { UpdateCustomerHandler.QUEUE_CODE, new UpdateCustomerHandler() },
            });

        public Message GetNextMessage() => null; // Usually pinging database for next message
        public bool StopRequested { get; set; }
        public void Start()
        {
            // Does not include full/complete logic for all scenarios (pre-validation, completing multi-threading, stopping/cancelling, etc)
            var _ = Task.Run(() =>
            {
                this.StopRequested = false;
                while (this.StopRequested)
                {
                    Thread.Sleep(1000); // Wait
                    var message = this.GetNextMessage();
                    if (message == null || this.Handlers.TryGetValue(message.QueueCode, out var handler))
                        continue; // Guard pattern to check message received and handler exists
                    handler.Execute(message); // Send request to handler (Command)
                }
            });
        }

    }
}
