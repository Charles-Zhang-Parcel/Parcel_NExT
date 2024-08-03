﻿using Parcel.Neo.Base.Framework.ViewModels;
using Parcel.NExT.Python.Helpers;
using System;
using System.Windows.Input;

namespace Parcel.Neo
{
    internal class CommandManagerWrapper : ICommandManager
    {
        public void AddEvent(EventHandler e)
        {
            CommandManager.RequerySuggested += e;
        }

        public void RemoveEvent(EventHandler e)
        {
            CommandManager.RequerySuggested -= e;
        }
    }

    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            // charles-z 20220726: Quick hack to get around dependancy
            CommandManagerWrapper wrapper = new();
            RequeryCommand.CommandManager = wrapper;
            RequeryCommand.CommandManager = wrapper;

            // Start the app
            var app = new App();
            app.InitializeComponent();
            app.Run();

            // Some libraries may have used Python, here we shut it down if it's initialized; Otherwise the application will not terminate
            PythonRuntimeHelper.TryShutdownEngine();
        }
    }
}