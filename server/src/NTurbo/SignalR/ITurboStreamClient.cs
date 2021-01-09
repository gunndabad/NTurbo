using System.Threading.Tasks;

namespace NTurbo.SignalR
{
    /// <summary>
    /// Represents a hub client that can receive Turbo Stream elements.
    /// </summary>
    public interface ITurboStreamClient
    {
        /// <summary>
        /// Sends a Turbo Stream element to the client.
        /// </summary>
        /// <param name="element">
        /// The Turbo Stream element HTML to send to the client.
        /// The HTML should be wrapped in a &lt;turbo-stream&gt; element.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that will complete when the element has been sent to the client.
        /// </returns>
        Task ReceiveStreamElement(string element);
    }
}
