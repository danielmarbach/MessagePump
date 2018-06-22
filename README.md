# MessagePump

Presentation Material for message pump presentation

Pictures are from pixabay.com

Font is Yanone Kaffeesatz

## Do it yourself. An async message pump that kicks ass

Building a message pump that consumes and produces messages from queues is astonishingly simple in theory. In practice, the picture looks a bit different. Over the last years, I've contributed and built queue adapters in .NET for RabbitMQ, Azure Service Bus, Azure Storage Queues, MSMQ, AmazonSQS, Kafka and SQL Server. And you can bet I made plenty of mistakes along the way!

In this talk, I'll show you what a robust and reliable message pump with TPL and async can look like. Furthermore, I'll teach you how you can use asynchronous synchronization primitives to throttle requests. I'll then compare and contrast different queuing technology message pumps. With this knowledge, you'll be ready to build an asynchronous message pump that does not only keep on pumping for ages but does that in a performant way. Join me and learn to create a message pump that kicks your queue's ass!
