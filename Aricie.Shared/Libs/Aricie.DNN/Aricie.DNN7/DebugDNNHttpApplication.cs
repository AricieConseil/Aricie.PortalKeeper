using System;
using System.Reflection;
using Aricie;
using Aricie.Services;
using Aricie.Web;
using DotNetNuke.Instrumentation;
using DotNetNuke.Web.Common.Internal;

namespace Diagnostics
{
    public class DebugDNNHttpApplication : DotNetNukeHttpApplication
    {

        public DebugDNNHttpApplication() : base()
        {
        }

        private void Application_BeginRequest(object sender, System.EventArgs e)
        {
            MethodInfo objMethod = typeof(DotNetNukeHttpApplication).GetMethod("Application_BeginRequest", BindingFlags.NonPublic | BindingFlags.Instance);
            objMethod.Invoke(this,new object[]  {sender,e});
            //HttpInternals.StopDirectoryMonitoring("App_LocalResources")

        }


        private void Application_Start(object sender, System.EventArgs e)
        {
            MethodInfo objMethod = typeof(DotNetNukeHttpApplication).GetMethod("Application_Start", BindingFlags.NonPublic | BindingFlags.Instance);
            objMethod.Invoke(this,new object[] {
                sender,
                e
            });

            HttpInternals.Instance.CombineCriticalChangeCallBack(new EventHandler(OnCriticaldirChange));
        }

        private void Application_End(object sender, System.EventArgs e)
        {
            LogEndBis();
            MethodInfo objMethod = typeof(DotNetNukeHttpApplication).GetMethod("Application_End", BindingFlags.NonPublic | BindingFlags.Instance);
            objMethod.Invoke(this, new object[]{
                sender,
                e
            });
        }



        public void LogEndBis()
        {
            try {
                ILog objLogger = typeof(DotNetNukeHttpApplication).GetField("Logger", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).GetValue(this) as ILog;
                objLogger.Info(HttpInternals.Instance.GetDump());

            } catch (Exception exc) {
                ExceptionHelper.LogException(exc);
            }

        }



        private void OnCriticaldirChange(object sender, EventArgs e)
        {
            ILog objLogger = typeof(DotNetNukeHttpApplication).GetField("Logger", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).GetValue(this) as ILog;
            object fields = ReflectionHelper.GetFields(e, 2);
            objLogger.InfoFormat(Environment.NewLine +"Critical Change: " + ReflectionHelper.Serialize(fields).Beautify() + Environment.NewLine);
        }





    }
}