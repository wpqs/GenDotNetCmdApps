using MxReturnCode;

namespace KLineEdCmdApp.Utils
{
    public interface IErrorState
    {
        bool IsErrorState();
        MxReturnCode<bool> GetErrorState();
        void ResetErrorState();
        bool SetErrorState(MxReturnCode<bool> mxErr);
    }
}