# 事件驱动架构Demo

## 运行

1. F5
2. 运行Powershell命令行
    ```powershell
    Invoke-RestMethod 'http://localhost:5000/api/Customers' -Method Post  
    ```
3. 观察控制台输出

## 分层

1. Domain.Core 定义领域模型的抽象 包括  
    * `IEvent` 事件
    * `IEventHandler` 事件处理器
    * `IEventSubscriber` 事件处理订阅: 用来注册一个事件的事件处理器
    * `IEventStore` 事件存储
    * `IEventPublisher` 事件发布器
    * `IEventBus` 事件总线, 用来处理事件和发布事件
    * `IEventHandlerExecutionContext` 事件处理上下文, 被总线持有, 用来实例化处理器
1. Domain 定义了一个事件和两个事件处理器
1. EventBus.Simple 实现了一个简单的事件总线
1. EventStore.Simple 实现了一个简单的事件存储
1. EventHandlerContext.Simple 实现了一个简单的事件处理上下文
1. MST 是一个WebApi  提供了一个调用方法
    ```c#
    var customer = new Customer {Name = "XiaoMa"};
    await _eventBus.PublishAsync(new CustomerCreatedEvent(customer.Name));
    return new ObjectResult(true);
    ```
