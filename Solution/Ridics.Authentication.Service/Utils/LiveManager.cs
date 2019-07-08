using System.Threading;

namespace Ridics.Authentication.Service.Utils
{
    public class LiveManager
    {
        private readonly CancellationTokenSource m_cancellationTokenSource;

        public LiveManager()
        {
            m_cancellationTokenSource = new CancellationTokenSource();
        }

        public CancellationToken GetCancellationToken()
        {
            return m_cancellationTokenSource.Token;
        }

        public void RebuildAndRestart()
        {
            ServiceShouldRestart = true;
            m_cancellationTokenSource.Cancel();
        }

        public bool ServiceShouldRestart { get; private set; } = false;
    }
}
