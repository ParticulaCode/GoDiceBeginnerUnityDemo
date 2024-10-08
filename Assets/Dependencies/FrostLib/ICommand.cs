﻿namespace FrostLib
{
    public interface ICommand
    {
        void Execute();
    }

    public interface ICommand<out T>
    {
        T Execute();
    }
}