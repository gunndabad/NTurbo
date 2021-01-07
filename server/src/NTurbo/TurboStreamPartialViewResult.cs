using Microsoft.AspNetCore.Mvc;

namespace NTurbo
{
    /// <summary>
    /// Represents an <see cref="ActionResult"/> that renders a partial view to the response
    /// with the Turbo Stream content type.
    /// </summary>
    public class TurboStreamPartialViewResult : PartialViewResult
    {
        /// <summary>
        /// The content type for a Turbo Stream.
        /// </summary>
        public const string TurboStreamContentType = "text/html; turbo-stream; charset=utf-8";

        /// <summary>
        /// Initializes a new instance of the <see cref="TurboStreamPartialViewResult"/> class.
        /// </summary>
        public TurboStreamPartialViewResult()
        {
            base.ContentType = TurboStreamContentType;
        }

        /// <summary>
        /// Gets the Content-Type header for the response.
        /// </summary>
        public new string ContentType
        {
            get => TurboStreamContentType;
        }
    }
}
