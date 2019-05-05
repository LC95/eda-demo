# 事件驱动架构Demo

## 运行

1. F5
2. 运行Powershell命令行
    ```powershell
    Invoke-RestMethod 'http://localhost:5000/api/Customers' -Method Post  
    ```
3. 观察控制台输出

## 项目分层

### 分层定义

API是控制器, 处理请求. 引用了
Domain.Core定义了一些核心逻辑抽象
Domain定义了核心的业务逻辑, 以及这些业务逻辑所需的抽象. 引用Domain.Core
EventBus.Simple实现了一个简单的事件总线. 引用Domain
EventBus.RabbitMQ使用RBMQ实现了另一个简单的事件总线. 引用Domain
EventHandlerContext.Simple实现了一个简单的事件处理上下文. 引用Domain
EventStore.Simple实现了一个简单的事件存储. 引用Domain

### Domain.Core层定义

1. 定义了事件的相关抽象
    1. `IEvent` 事件
    1. `IEventBus` 事件处理器
    1. `IEventSubscriber` 事件处理订阅
    1. `IEventStore` 事件存储
    1. `IEventPublisher` 事件发布器
    1. `IEventBus` 事件总线, 继承了事件订阅和事件发布. 它是一个中介.
    1. `BaseEventBus` 事件总线基类
    1. `IEventHandler` 事件处理
    1. `IEventHandlerExecutionContext` 事件处理上下文, 用来实例化事件处理器
2. 定义了领域相关的抽象
    1. `IDomainEvent` 领域事件接口, 继承IEvent. 定义了一些聚合信息
    1. `DomainEvent` 领域事件抽象类实现
    1. `AggregateCreatedEvent` 聚合创建事件, 一个领域事件的创建
    1. `DomainEvent` 


## 运行过程

    1. 控制器层注入了事件处理上下文, 事件总线, 事件仓储
    2. 控制器接受一个创建聚合的Command
    3. 在Command执行完毕后创建一个"聚合已创建事件"
    4. 该事件被发布到事件总线中, 几个事件总线接收者接受该事件
    5. 每个接收者持有一个事件处理上下文, 用来实例化事件处理器
    6. 事件处理器处理事件, 并确定是否需要发布新的事件