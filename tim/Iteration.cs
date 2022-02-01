using System.Collections;
using System.Collections.Generic;
using System;

class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("User input with loop - indefinite iteration");

        //setup
        int number;
        int maxRuns = 23;

        while(number < maxRuns)
        {
            Console.WriteLine("Enter a small int")
            int inInt = Convert.ToInt32(Console.ReadLine()); //cast string to int
            number = number + inInt;
            Console.WriteLine("...total so far is: " + number); //concatenation string + string
        }

        Console.WriteLine("HALT...combined total is now greater than " + maxRuns); //concatenation again
    }
}