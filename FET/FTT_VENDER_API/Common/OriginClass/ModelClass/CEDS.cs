using FTT_VENDER_API.Common.OriginClass.EntiityClass;
using System.ComponentModel;
using static FTT_VENDER_API.Common.OriginClass.EntiityClass.Employee;

namespace FTT_VENDER_API.Common.OriginClass.ModelClass
{
    public class CEDS : IDisposable
    {
        private bool IsDisposed = false;

        private Container components = null;

        ~CEDS()
        {
            Dispose(Disposing: false);
        }

        public void Dispose()
        {
            Dispose(Disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool Disposing)
        {
            if (!IsDisposed && Disposing && components != null)
            {
                components.Dispose();
            }

            IsDisposed = true;
        }


        public string GetEmpName(RefType Type, string Data)
        {
            //IL_0022: Unknown result type (might be due to invalid IL or missing references)
            //IL_0024: Unknown result type (might be due to invalid IL or missing references)
            //IL_002a: Expected O, but got Unknown
            string text = "";
            if (Data == "")
            {
                text = "";
            }
            else
            {
                Employee val = new Employee(Type, Data);
                text = ((!val.hasData()) ? Data : (val.EmployeeName + "(" + val.EnglishName + ")"));
                val.Dispose();
            }

            return text;
        }
    }
}
