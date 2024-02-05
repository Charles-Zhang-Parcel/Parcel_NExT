using Parcel.CoreEngine.Document;

namespace Parcel.CoreEngine.Service.CoreExtensions
{
    /// <summary>
    /// Provide standalone node-as-service execution services
    /// </summary>
    public static class ParcelNodeExtension
    {
        #region Runtime
        public static ParcelPayload Execute(this ParcelNode node)
        {
            // TODO: Somehow just execute the node as is
            throw new NotImplementedException();

            // TODO: At the moment we provice a temporary solution - eventually we need to merge with GraphRuntime
        }
        #endregion
    }
}
