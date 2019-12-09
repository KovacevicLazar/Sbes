using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public enum AuditEventTypes
    {
        UserAuthenticationSuccess = 0,
        UserAuthenticationFailed = 1,
        ServiceValidationSuccess = 2,
        ServiceValidationFailed = 3
    }

    public class AuditEvents
    {
        private static ResourceManager resourceManager = null;
        private static object resourceLock = new object();

        private static ResourceManager ResourceMgr
        {
            get
            {
                lock (resourceLock)
                {
                    if (resourceManager == null)
                    {
                        resourceManager = new ResourceManager(typeof(AuditEventFile).FullName, Assembly.GetExecutingAssembly());
                    }
                    return resourceManager;
                }
            }
        }

        public static string UserAuthenticationSuccess
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.UserAuthenticationSuccess.ToString());
            }
        }

        public static string UserAuthenticationFailed
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.UserAuthenticationFailed.ToString());
            }
        }

        public static string ServiceValidationSuccess
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.ServiceValidationSuccess.ToString());
            }
        }

        public static string ServiceValidationFailed
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.ServiceValidationFailed.ToString());
            }
        }

    }
}
