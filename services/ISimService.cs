using Lside_Mixture.Managers;
using Lside_Mixture.Models;

namespace Lside_Mixture.Services
{
    public interface ISimService 
    {
        bool Connected
        {
            get;
        }

        bool Crashed
        {
            get;
        }

        SampleModel SampleModel 
        { 
            get; 
        }

        /// <summary>
        /// Sets the engine Mixture, in %.
        /// </summary>
        /// <param name="heading"></param>
        void SetMixture(double mixture);

        PlaneInfoResponse MostRecentSample
        {
            get;
        }
    }
}
