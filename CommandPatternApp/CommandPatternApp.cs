using System;
using System.Collections.Generic;

// Define a common interface for all commands.
public interface ICommand
{
    // Execute the command, given the current result.
    // It returns the new result after execution.
    int Execute(int current);
    
    // Undo the command, restoring the previous state.
    // It returns the restored result.
    int Undo(int current);
}

// Increment command: increases the result by 1.
public class IncrementCommand : ICommand
{
    private int previousValue;

    public int Execute(int current)
    {
        // Save current value before change.
        previousValue = current;
        return current + 1;
    }

    public int Undo(int current)
    {
        // Restore the value saved before this command.
        return previousValue;
    }
}

// Decrement command: decreases the result by 1.
public class DecrementCommand : ICommand
{
    private int previousValue;

    public int Execute(int current)
    {
        previousValue = current;
        return current - 1;
    }

    public int Undo(int current)
    {
        return previousValue;
    }
}

// Double command: multiplies the result by 2.
public class DoubleCommand : ICommand
{
    private int previousValue;

    public int Execute(int current)
    {
        previousValue = current;
        return current * 2;
    }

    public int Undo(int current)
    {
        return previousValue;
    }
}

// RandAdd command: changes the result by a random number.
// The random number is generated once per command execution.
public class RandAddCommand : ICommand
{
    private int previousValue;
    private int randomNumber;

    // Constructor accepts a Random instance.
    public RandAddCommand(Random rand)
    {
        // Generate a random number between 1 and 10.
        randomNumber = rand.Next(1, 11);
    }

    public int Execute(int current)
    {
        previousValue = current;
        return current + randomNumber;
    }

    public int Undo(int current)
    {
        return previousValue;
    }
}

// Undo command: undoes the effect of a given command.
public class UndoCommand : ICommand
{
    private ICommand commandToUndo;

    public UndoCommand(ICommand command)
    {
        commandToUndo = command;
    }

    public int Execute(int current)
    {
        // Simply call the Undo method of the command being undone.
        return commandToUndo.Undo(current);
    }

    // For our purposes, we do not support undoing an undo.
    public int Undo(int current)
    {
        // Not implemented as undo of undo is not required.
        return current;
    }
}

public class CommandPatternApp
{
    public static void Main(string[] args)
    {
        if (args.Length != 1 || !int.TryParse(args[0], out int result))
        {
            Console.WriteLine("Usage: <CommandPatternApp> <initial_value>");
            return;
        }

        Stack<ICommand> commandHistory = new Stack<ICommand>();
        Random rand = new Random();
        Console.Write("Supported Commands: increment, decrement, double, randadd, undo \n" +
                        "Enter single command (e.g., increment) or command sequence (e.g., increment, decrement, undo) \n");

        while (true)
        {
            Console.Write("Enter command(s) (e.g., increment, decrement, double, randadd, undo): ");
            string inputLine = Console.ReadLine()?.Trim().ToLower();

            if (string.IsNullOrEmpty(inputLine))
            {
                continue;
            }

            // Split input by comma to support multiple commands at once.
            string[] commands = inputLine.Split(',');

            foreach (string rawCmd in commands)
            {
                string input = rawCmd.Trim(); // Trim spaces around each command
                if (string.IsNullOrEmpty(input)) continue;

                ICommand command = null;
                bool isUndoCommand = false;

                switch (input)
                {
                    case "increment":
                        command = new IncrementCommand();
                        break;
                    case "decrement":
                        command = new DecrementCommand();
                        break;
                    case "double":
                        command = new DoubleCommand();
                        break;
                    case "randadd":
                        command = new RandAddCommand(rand);
                        break;
                    case "undo":
                        if (commandHistory.Count == 0)
                        {
                            Console.WriteLine("No command to undo.");
                            continue;
                        }
                        ICommand lastCommand = commandHistory.Pop();
                        command = new UndoCommand(lastCommand);
                        isUndoCommand = true;
                        break;
                    default:
                        Console.WriteLine($"Unknown command: {input}");
                        continue;
                }

                result = command.Execute(result);
                Console.WriteLine($"After '{input}': {result}");

                if (!isUndoCommand && input != "undo")
                {
                    commandHistory.Push(command);
                }
            }
        }
    }
}
