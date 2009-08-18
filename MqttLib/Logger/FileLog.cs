using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MqttLib.Logger
{
    public enum LogFileModes : uint
    {
        // Simply append to the existing file
        NORMAL,
        // Start from the beginning if the log gets larger than the maximum log size
        ROTATE,
        // Create a new log file when the existing one gets larger than the maximum size
        SEPARATE,
    }

    public class FileLog : ILog
    {
        private string _filename;
        private string _name;
        private int _maxSize = 1024;
        private LogFileModes _mode = LogFileModes.ROTATE;
        private LogLevel _loggingLevel = LogLevel.DEV;

        /// <summary>
        /// Determins the type of file size control
        /// </summary>
        public LogFileModes Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        /// <summary>
        /// The maximum sie of the log file in Kilobytes
        /// </summary>
        public int MaxSize
        {
            get { return _maxSize; }
            set { _maxSize = value; }
        }

        /// <summary>
        /// The Name of the log
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// The filename this log is writing to.
        /// </summary>
        public string Filename
        {
            get { return _filename; }
        }

        public FileLog(string path, string name, LogFileModes mode)
        {
            // Set an initial log file
            _name = name;
            Mode = mode;

            if (mode == LogFileModes.SEPARATE)
            {
                _filename = path + "\\" + name + "_" + DateTime.Now.Ticks + ".log";
            }
            else
            {
                _filename = path + "\\" + name + ".log";
            }
            WriteHeader();
        }

        public FileLog(string path, string name) : this(path, name, LogFileModes.ROTATE) { }

        public FileLog(string name) : this(Directory.GetCurrentDirectory(), name, LogFileModes.ROTATE) { }

        private void WriteFile(string text)
        {
            Stream stm = OpenFile();
            if (stm != null)
            {
                try
                {
                    StreamWriter writer = new StreamWriter(stm);
                    writer.WriteLine
                    (
                        DateTime.Now.ToShortDateString() + " - " +
                        DateTime.Now.ToLongTimeString() + " - "  +
                        text
                    );
                    writer.Close();
                    Console.WriteLine(text);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Failed writing to log: " + e.Message);
                }
            }
        }

        /// <summary>
        /// Attempt to open the log file. If log rotation, or sice control is
        /// on this function may change the log filename
        /// </summary>
        /// <returns>A Stream that can be written to</returns>
        private Stream OpenFile()
        {
            Stream stm = null;
            bool append = true;
            
            try
            {
                if (File.Exists(_filename))
                {
                    FileInfo tempLogFile = new FileInfo(_filename);
                    // Check whether this log file is too large
                    if (tempLogFile.Length / 1024 >= MaxSize)
                    {
                        switch (Mode)
                        {
                            case LogFileModes.ROTATE:
                                append = false;
                                break;
                            case LogFileModes.SEPARATE:
                                //change the filename
                                _filename = tempLogFile.Directory.FullName +
                                            "\\" + Name + "_" + DateTime.Now.Ticks +
                                            ".log";
                                break;
                            default:
                            // Do nothing
                                break;
                        }
                    }
                }
                // Attempt to open the file
                FileInfo logFile = new FileInfo(_filename);
                FileMode m;
                if (append) 
                { 
                    m = FileMode.Append; 
                }
                else
                { 
                    m = FileMode.OpenOrCreate;
                }
                
                stm = (Stream)logFile.Open(m, FileAccess.Write);
                
                
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Failed to open log file: " + e.Message);
                stm = null;
            }
            return stm;
        }

        
        #region ILog Members

        public void Write(string message)
        {
            WriteFile
            (
                "[" + LogLevel.DEBUG.ToString() + "]" +
                message
            );
        }

        public void Write(LogLevel level, string message)
        {
            if ((uint)level >= (uint)_loggingLevel)
            {
                WriteFile
                (
                    "[" + level.ToString() + "]" +
                    message
                );
            }
        }

        public LogLevel LoggingLevel 
        {
            get
            {
                return _loggingLevel;
            }
            set
            {
                _loggingLevel = value;
            }
        }

        #endregion

        private void WriteHeader()
        {
            Stream stm = OpenFile();
            if (stm != null)
            {
                try
                {
                    StreamWriter writer = new StreamWriter(stm);
                    writer.WriteLine("#");
                    writer.WriteLine("# Creating new log");
                    writer.WriteLine("#");
                    writer.Close();
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Failed writing to log: " + e.Message);
                }
            }
        }
    }
}
