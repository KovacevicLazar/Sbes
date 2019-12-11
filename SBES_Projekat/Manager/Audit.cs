using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public class Audit : IDisposable
    {
        private static EventLog customLog = null;
        const string SourceName = "Manager.Audit";
        const string LogName = "MySecTest";



        static Audit()
        {
            try
            {
                if (!EventLog.SourceExists(SourceName))
                {
                    EventLog.CreateEventSource(SourceName, LogName);
                }

                customLog = new EventLog(LogName, Environment.MachineName, SourceName);
            }
            catch (Exception e)
            {
                customLog = null;
                Console.WriteLine("Error while trying to create log handle. Error = {0}", e.Message);
            }
        }


        public static void AuthenticationSuccess(string userName)
        {
            string UserAuthenticationSuccess = AuditEvents.UserAuthenticationSuccess;
            if (customLog != null)
            {
                string message = string.Format(UserAuthenticationSuccess, userName);
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.", (int)AuditEventTypes.UserAuthenticationSuccess));
            }
        }

        public static void AuthenticationFailed(string userName)
        {
            string UserAuthenticationFailed = AuditEvents.UserAuthenticationFailed;
            if (customLog != null)
            {
                string message = string.Format(UserAuthenticationFailed, userName);
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.", (int)AuditEventTypes.UserAuthenticationFailed));
            }
        }

        public static void ValidationSuccess(string userName)
        {
            string ServiceValidationSuccess = AuditEvents.ServiceValidationSuccess;
            if (customLog != null)
            {
                string message = string.Format(ServiceValidationSuccess, userName);
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.", (int)AuditEventTypes.ServiceValidationSuccess));
            }
        }

        public static void ValidationFailed(string userName)
        {
            string ServiceValidationFailed = AuditEvents.ServiceValidationFailed;
            if (customLog != null)
            {
                string message = string.Format(ServiceValidationFailed, userName);
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.", (int)AuditEventTypes.ServiceValidationFailed));
            }
        }

        public void Dispose()
        {
            if (customLog != null)
            {
                customLog.Dispose();
                customLog = null;
            }
        }
    }
}
