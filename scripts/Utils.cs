using System;
using System.Collections.Generic;
using Godot;

public static class Utils
{
    private static Random random = new Random();

    public static float ConvergeValue(float value, float target, float increment)
    {
        // Move value towards target in steps of size increment.
        // If increment is negative can also be used to do the opposite

        float difference = value - target;
        if (Mathf.Abs(difference) < increment) return target;
        else return value + -Mathf.Sign(difference) * increment;
    }

    public static T RandomFromList<T>(List<T> data)
    {
        // Select a random item from a list
        
        return data[random.Next(data.Count)];
    }

    public static float RandomFloat(float min, float max)
    {
        // Random float between two numbers

        return (float) (random.NextDouble() * (max - min) + min);
    }
}