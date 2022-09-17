using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace CgalUtilWrapper
{
    public abstract class GeoWrapperBase : IDisposable
    {
        protected const string DLL = "CgalUtil.dll";

        protected IntPtr _handle = IntPtr.Zero;

        protected string _error = "";

        protected bool _isDisposed = false;

        public bool IsDisposed => _isDisposed;

        public bool IsValid => _handle != IntPtr.Zero;

        public string Error => _error;

        #region Memory management.
        protected virtual void DropManaged()
        {
            _error = null;
        }

        protected virtual void DropUnmanaged() { }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed) return;

            if (disposing)
            {
                DropManaged();
            }

            if (_handle != IntPtr.Zero)
            {
                DropUnmanaged();
                _handle = IntPtr.Zero;
            }

            _isDisposed = true;
        }

        protected virtual void Gc()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
