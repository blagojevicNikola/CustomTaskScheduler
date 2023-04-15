# CustomTaskScheduler
A custom preemptive task scheduler
## Technologies
- WPF(.NET Framework)

## Project Description
This project represents a custom task scheduler that schedules tasks depending on defined number of cores, level of parallelism of every task, and scheduler mode (***preemptive*** and ***non-preemptive***).

In MyTaskScheduler library there is an abstract class ***UserTask*** which method ***algorithm*** needs to be implemented. In that way we define functionality of our custom task that can
be scheduled by MyTaskScheduler.

- Mechanism of locking resources(files) used by tasks
- User can define number of cores available on machine and number of concurrent tasks 
- Implemented ***Multishot*** and ***Hierarchical deadlock avoidance*** algorithms
- In WPF app, it's represented practical usage of CustomTaskSheduler, as well as implementation of custom tasks for ***serial and parallel image blurring***
- Ability to ***stop***, ***pause*** and ***resume*** tasks scheduled in MyTaskScheduler

## Demo
<img src="/WPFScheduler/Resources/TaskScheduler.png"  width="600"> 
