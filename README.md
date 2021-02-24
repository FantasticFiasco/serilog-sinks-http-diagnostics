# Benchmarks for Serilog.Sinks.Http

This repository contains one console application capable of generating log events and one ASP.NET Core application capable of receiving log events sent over the network using [Serilog.Sinks.Http](https://github.com/FantasticFiasco/serilog-sinks-http).

The applications have been suited to fill at least two roles.

## Role 1 - Act as benchmark

The console application accept arguments which directly controls the amount of log events, anf their size, that are generated and sent over the network using _Serilog.Sinks.Http_. This can be a great insight in whether your infrastructure and lof server is capable of handling the load of the log events.

The applications are also used to validate that the same number of log events generated in the console application are received by the log server, thus flushing out any potential bugs in the sink implementation.

## Role 2 - Provide insight into the log events produced by your application

The behavior of _Serilog.Sinks.Http_ is determined by its configuration. How often should we send a batch of log events over the network? How many buffer files should we use? Which size should the buffer files have?

to see the amount of events the sink can send ot

## Console app using the Serilog HTTP sink

The

## Log server receiving log events
