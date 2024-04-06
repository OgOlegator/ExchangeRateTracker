using ExchangeRateTracker.AutoSynhronize.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRateTracker.AutoSynhronize
{
    partial class AutoSynhronizeExchRate : ServiceBase
    {
        public AutoSynhronizeExchRate()
        {
            InitializeComponent();
        }

        private System.Timers.Timer _timer;
        private StreamWriter _file;
        protected override void OnStart(string[] args)
        {
            //Файл по умолчанию будет создан в   "C:\Winnt\System32\"
            _file = new StreamWriter(new FileStream(
                $"C:\\Users\\olegp\\Documents\\AutoSynhronizeExchRatesService\\AutoSynhronizeExchRateService-{DateTime.Now.ToString("dd.MM.yyyy-hh.mm")}.log",  
                System.IO.FileMode.Append));
            
            _file.WriteLine($"{DateTime.Now} - AutoSynhronizeExchRate стартовал");
            _file.Flush();

            _timer = new System.Timers.Timer();
            _timer.Enabled = true;

            //Интервал 60000мс - 60с.
            _timer.Interval = 60000;
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
            _timer.AutoReset = true;
            _timer.Start();
        }

        protected override void OnStop()
        {
            _timer.Stop();
            _file.WriteLine($"{DateTime.Now} - AutoSynhronizeExchRate завершил работу"); 
            _file.Flush();
            _file.Close();
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var synhronizeService = new SynhronizeRateSevice();

            _file.WriteLine($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")} Запуск синхронизации");
            _file.Flush();

            try
            {
                var result = synhronizeService.ExecuteAsync().GetAwaiter();

                result.OnCompleted(() =>
                {
                    _file.WriteLine(result.GetResult().Message);
                    _file.Flush();
                });
            }
            catch (Exception ex)
            {
                _file.WriteLine($"Ошибка в процессе синхронизации. {ex.Message}");
                _file.Flush();
            }
        }
    }
}
