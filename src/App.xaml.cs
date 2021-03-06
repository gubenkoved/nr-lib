﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace NRLib
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            IoC.Init();

            MainWindow mainWindow = IoC.Get<MainWindow>();

            mainWindow.Show();
        }
    }
}
