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
    }
}
