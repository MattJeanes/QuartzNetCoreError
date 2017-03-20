# QuartzNetCoreError
Just a quick app to show exceptions thrown by Quartz in it's alpha dotnetcore build

## Update:

This is now resolved, you must return a Task (or Task.CompletedTask) instead of null, I have asked the developer to add some kind of indication for this failure as it is currently a silent failure without the job or scheduler listeners.

See: https://github.com/quartznet/quartznet/issues/459
