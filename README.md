# Maze Explorer

Maze Explorer is a simple maze game. The goal is to find the exit from the maze.
It's a sample project to demonstrate base concepts of Unity3D game development.

# Main Concepts

## [MessageBus](Assets/Modules/Events/Event.cs)
Events are used to communicate between different parts of the game. Used when there are more than one destination on it's not known in advance.
Also events can be awaited.

## [ServiceLocator](Assets/Modules/ServiceLocator/ServiceLocator.cs)
ServiceLocator is used to resolve dependencies between different game components.

## [State Machine](Assets/Modules/Fsm/FsmService.cs)
State Machine is used to hold game states and switch between them.

## [UI System](Assets/Modules/UIService/UIService.cs)
Model-View based UI system. Code related to visual appearance defined on view-s. Code related to providing data and handling user input defined on model-s. 

## Check out [WebGL Build](https://vadim-agamov.github.io/maze/index.html)
